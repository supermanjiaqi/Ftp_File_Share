using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Ftp_File_Share
{
    class Ftp_Class
    {
        private string ftpUriString = "";
        private string username = "";
        private string password = "";



        private NetworkCredential networkCredential;


        private static long flag = 0;


        public Ftp_Class (string _ftpUriString,string _username,string _password)
        {
           ftpUriString =_ftpUriString;
           username =_username;
           password =_password;

           networkCredential = new NetworkCredential(username, password);

           if (ShowFtpFileAndDirectory(ftpUriString) == true)
           {
               Console.WriteLine("Connect Server OK!");
           }
           
        }


        private  bool ShowFtpFileAndDirectory(string ftpUriString)
        {
            string uri = string.Empty;

            uri = ftpUriString;

            FtpWebRequest request;
            request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.ListDirectoryDetails);

            //获取服务器端响应
            FtpWebResponse response = GetFtpResponse(request);
            if (response == null)
                return false;
            //读取网络流信息
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string s = sr.ReadToEnd();
            string[] ftpDir = s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //在listBoxInfo中显示服务器响应的原信息
            Console.WriteLine("Server_FeedBack:" + response.StatusDescription);
            return true;
        }


        private  FtpWebRequest CreateFtpWebRequest(string uri, string requestMethod)
        {
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Credentials = networkCredential;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = requestMethod;
            return request;
        }


        private FtpWebResponse GetFtpResponse(FtpWebRequest request)
        {
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                return response;
            }
            catch (WebException err)
            {
                FtpWebResponse e = (FtpWebResponse)err.Response;
                return null;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public int FileUpload (string fullfileName)
        {
            int status = 0;
            int count = 0;
            while (true)
            {

                Thread.Sleep(10000);
               
                count++;
                if (count > 3)
                {
                    break;
                }
                Console.WriteLine(count +"  "+ fullfileName);
            }
                
           
                shareRes.mutex.WaitOne();
                Interlocked.Increment(ref flag);     // 状态值+1

                FileInfo fileInfo = new FileInfo(fullfileName);
             try
                {
                string uri = ftpUriString + "/" + fileInfo.Name;
                System.Net.ServicePointManager.DefaultConnectionLimit = 10;
                FtpWebRequest request = CreateFtpWebRequest(uri, WebRequestMethods.Ftp.UploadFile);
                request.ContentLength = fileInfo.Length;
                int buffLength = 8196*4;
                byte[] buff = new byte[buffLength];
               // FileStream fs = fileInfo.OpenRead();
                FileStream fs = new FileStream(fullfileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                int allbye = (int)fileInfo.Length;
                int startbye = 0;
                float current_byte = 0;
                float all =  allbye/1024;
                Stream responseStream = request.GetRequestStream();
                int contentLen = fs.Read(buff, 0, buffLength);

                ConsoleColor colorBack = Console.BackgroundColor;
                ConsoleColor colorFore = Console.ForegroundColor;

                Console.WriteLine("UpLoading Start..." + fullfileName);
                int row = Console.CursorTop+1;
                


                while (contentLen != 0)
                {
                    responseStream.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                   // Console.WriteLine("已上传:" + (int)((startbye /1024)) + "/" + "总长度:" + (int)(allbye / 1024) + "KB" + " " + " 文件名:" + fileInfo.Name);
                    current_byte = startbye / 1024;
                    int current_per = (int)(Math.Ceiling(current_byte) / all * 100.0);
                    if (current_byte == all)
                    {
                        current_per = 100;
                    }
                    Console.BackgroundColor = ConsoleColor.Gray;//设置进度条颜色  
                    Console.SetCursorPosition(current_per / 2, row);
                    Console.Write(" ");//移动进度条                
                    Console.BackgroundColor = colorBack;//恢复输出颜色 
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(0, row+1);
                    Console.WriteLine("{0}%", current_per);
                    Console.ForegroundColor = colorFore;

                }
                responseStream.Close();
                fs.Close();
                FtpWebResponse response = GetFtpResponse(request);
                if (response == null)
                {
                    Console.WriteLine("Upload Failure！" + fullfileName);
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("Upload Success！" + fullfileName);
                    Console.WriteLine("");
                }
                return 100;


            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return 1;

            }

            finally
            {
                Interlocked.Decrement(ref  flag);
                shareRes.mutex.ReleaseMutex(); // 释放线程
                //Console.WriteLine("last!");
            }
            
           
        }

        




    }


    class shareRes
    {
        public static int count = 0;
        public static Mutex mutex = new Mutex();
    }


}
