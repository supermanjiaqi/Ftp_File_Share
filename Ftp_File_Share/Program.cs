/*////////版本说明
1.0  具备单个文件上传的能力
2.0  多个线程上传
3.0  增加上传进度显示
4.0  增加回调功能
5.0  增加上传完成后更新数据库状态isFTPUploadFinish

作者:superman
*/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;



namespace Ftp_File_Share
{
    class Program
    {
        

       
        public static Queue<string> TaskQueue = new Queue<string>();
        private static int Last_Status =0;
        public delegate int Upload_Task(string path) ;

        static void Main(string[] args)
        {
            try
            {
                Console.Title = "Ftp_File_Share5.0";

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("File_Watcher_Start:");


                string ftpUriString = "ftp://10.3.86.126:22";
                string user = "administrator";
                string password = "lgzybxtcx";

                //test
                //string ftpUriString = "ftp://192.168.2.173:22";
                //string user = "superman";
                //string password = "007";

                Ftp_Class ftp = new Ftp_Class(ftpUriString, user, password); //ftp建立


                FileWatcher watcher_bof1 = new FileWatcher(@"D:\", "*.avi"); //文件监控开启

                //FileWatcher watcher_bof1 = new FileWatcher(@"D:\BOF1\Video_COL", "*.avi", ftp);
                //watcher_bof1.Open();
                //FileWatcher watcher_bof2 = new FileWatcher(@"D:\BOF2\Video_COL", "*.avi", ftp);
                //watcher_bof2.Open();
                //FileWatcher watcher_bof3 = new FileWatcher(@"D:\BOF3\Video_COL", "*.avi", ftp);
                //watcher_bof3.Open();
                



                //Thread thr = new Thread(new ParameterizedThreadStart(Task_Run));
                //thr.Start(ftp);



                while (true)
                {
                    int result = 0;
                    string path = "";
                    //如果IsWatch为False，则可能监控内部发生异常终止了监控，需要重新开启监控
                    if (!watcher_bof1.IsWatch)
                    {
                        watcher_bof1.Open();
                    }
                    if (TaskQueue.Count > 0)
                    {
                        Upload_Task uptask = new Upload_Task(ftp.FileUpload);
                        AsyncCallback callback = new AsyncCallback(_ar =>
                        {
                         
                        });

                       path = TaskQueue.Dequeue();
                       IAsyncResult iaresult = uptask.BeginInvoke(path, callback, null);

                       result = uptask.EndInvoke(iaresult);

                       if (result == 100)
                       {
                           string name = Path.GetFileNameWithoutExtension(path) + ".mp4"; //文件名 没后缀
                           string sql = string.Format("update dbo.Video_info set isFTPUploadFinish=1,isFTPUploadFinishDate=CONVERT(varchar,GETDATE()) where name = '{0}'", name);
                           int changed = DbHelperSQL.ExecuteSql(sql);
                           Console.WriteLine("Call_Back_Upload Success!");
                       }
                       else
                       {
                           Console.WriteLine("Call_Back_Upload Failure!");
                       }
                    }


                    Thread.Sleep(2000);

                }




        



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        private static void Task_Run( object obj)
        {
            Ftp_Class ftp = (Ftp_Class)obj;
            int status = 0;
            Console.WriteLine("Upload Task Start:");
            while (true)
            {
                Console.WriteLine(TaskQueue.Count);
                if (TaskQueue.Count > 0)  ///出列
                {

                    string path_temp = "";
                    path_temp = TaskQueue.Dequeue();
                    
                    status =ftp.FileUpload(path_temp);
                    if (status == 100)
                    {
                        continue;
                    }
                    if (status == 0)
                    {
                        Console.WriteLine("系统异常！请重新启动");
                        break;
                    }

                }
                Thread.Sleep(2000);
            }
        }

      


    }
}
