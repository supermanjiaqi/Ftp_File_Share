using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Ftp_File_Share
{
    class FileWatcher
    {

            private System.IO.FileSystemWatcher _watcher = null;
            private string _path = string.Empty;
            private string _filter = string.Empty;
            private bool _isWatch = false;
            // private CustomQueue<FileChangeInformation> _queue = null;

            //private Ftp_Class ftp;
        
            /// <summary>
            /// 监控是否正在运行
            /// </summary>
            public bool IsWatch
            {
                get
                {
                    return _isWatch;
                }
            }

            /// <summary>
            /// 文件变更信息队列
            /// </summary>
            //public CustomQueue<FileChangeInformation> FileChangeQueue
            //{
            //    get
            //    {
            //        return _queue;
            //    }
            //}

            /// <summary>
            /// 初始化FileWatcher类
            /// </summary>
            /// <param name="path">监控路径</param>
            public FileWatcher(string path)
            {
                _path = path;
                //_queue = new CustomQueue<FileChangeInformation>();



            }
            /// <summary>
            /// 初始化FileWatcher类，并指定是否持久化文件变更消息
            /// </summary>
            /// <param name="path">监控路径</param>
            /// <param name="isPersistence">是否持久化变更消息</param>
            /// <param name="persistenceFilePath">持久化保存路径</param>
            public FileWatcher(string path, bool isPersistence, string persistenceFilePath)
            {
                _path = path;
                // _queue = new CustomQueue<FileChangeInformation>(isPersistence, persistenceFilePath);
            }

            /// <summary>
            /// 初始化FileWatcher类，并指定是否监控指定类型文件
            /// </summary>
            /// <param name="path">监控路径</param>
            /// <param name="filter">指定类型文件，格式如:*.txt,*.doc,*.rar</param>
            public FileWatcher(string path, string filter)
            {
                _path = path;
                _filter = filter;
                // _queue = new CustomQueue<FileChangeInformation>();
               // ftp = _ftp;
                
            }

            /// <summary>
            /// 初始化FileWatcher类，并指定是否监控指定类型文件，是否持久化文件变更消息
            /// </summary>
            /// <param name="path">监控路径</param>
            /// <param name="filter">指定类型文件，格式如:*.txt,*.doc,*.rar</param>
            /// <param name="isPersistence">是否持久化变更消息</param>
            /// <param name="persistenceFilePath">持久化保存路径</param>
            public FileWatcher(string path, string filter, bool isPersistence, string persistenceFilePath)
            {
                _path = path;
                _filter = filter;
                // _queue = new CustomQueue<FileChangeInformation>(isPersistence, persistenceFilePath);
            }

            /// <summary>
            /// 打开文件监听器
            /// </summary>
            public void Open()
            {
                if (!System.IO.Directory.Exists(_path))
                {
                    System.IO.Directory.CreateDirectory(_path);
                }

                if (string.IsNullOrEmpty(_filter))
                {
                    _watcher = new System.IO.FileSystemWatcher(_path);
                }
                else
                {
                    _watcher = new FileSystemWatcher(_path, _filter);
                }
                _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
                //注册监听事件
                _watcher.Created += new FileSystemEventHandler(OnProcess);
                //_watcher.Changed += new FileSystemEventHandler(OnProcess);

               // _watcher.Deleted += new FileSystemEventHandler(OnProcess);
               // _watcher.Renamed += new RenamedEventHandler(OnFileRenamed);
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = true;
                _isWatch = true;
            }

            /// <summary>
            /// 关闭监听器
            /// </summary>
            public void Close()
            {
                _isWatch = false;
                _watcher.Created -= new System.IO.FileSystemEventHandler(OnProcess);
                //_watcher.Changed -= new FileSystemEventHandler(OnProcess);

                //_watcher.Deleted -= new FileSystemEventHandler(OnProcess);
                //_watcher.Renamed -= new RenamedEventHandler(OnFileRenamed);
                _watcher.EnableRaisingEvents = false;
                _watcher = null;
            }

            /// <summary>
            /// 获取一条文件变更消息
            /// </summary>
            /// <returns></returns>
            //public FileChangeInformation Get()
            //{
            //    FileChangeInformation info = null;
            //    if (_queue.Count > 0)
            //    {
            //        lock (_queue)
            //        {
            //            info = _queue.Dequeue();
            //        }
            //    }
            //    return info;
            //}

            /// <summary>
            /// 监听事件触发的方法
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnProcess(object sender, FileSystemEventArgs e)
            {
                try
                {
                    // FileChangeType changeType = FileChangeType.Unknow;
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        Console.WriteLine("Create_File！" + e.FullPath);
                        if (File.GetAttributes(e.FullPath) == FileAttributes.Directory)
                        {
                            // changeType = FileChangeType.NewFolder;
                            
                        }
                        else
                        {
                            // changeType = FileChangeType.NewFile;
                        }


                        //bool Upload_status = false;
                        string filepath = e.FullPath;
                        // FileInfo fi = new FileInfo(filepath);


                        //if ((fi.Exists) && (fi.Length > 0))
                        //{
                            //My_Event_Class my_e = new My_Event_Class(e.FullPath);
                            //my_e.startUpEvent += new My_Event_Class.myUpEventsHandler(ftp.FileUpload);
                            //my_e.mythreadStart();

                            Program.TaskQueue.Enqueue(e.FullPath);//入列
                            string name = Path.GetFileNameWithoutExtension(e.FullPath) + ".mp4"; //文件名 没后缀
                            string sql = string.Format("insert into dbo.Video_info(Name,isFTPUploadFinish,isFTPUploadFinishDate) values('{0}','{1}',CONVERT(varchar,GETDATE()))", name, 0);
                            int changed = DbHelperSQL.ExecuteSql(sql);

                        //}



                            //Upload_status = ftp.FileUpload(e.FullPath);
                            //if (Upload_status == true)
                            //{
                            //    Console.WriteLine("Upload Success！");
                            //}
                            //else
                            //{
                            //    Console.WriteLine("Upload Failure！");
                            //}


                    }
                    else if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        //_watcher.EnableRaisingEvents = false;

                        string temp = e.Name.Substring(11, 14);


                        Console.WriteLine("Change_File！" + e.FullPath);
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        Console.WriteLine("Delete_File！");
                        // changeType = FileChangeType.Delete;
                    }

                }
                catch
                {
                    Close();
                }
            
            }


            /// <summary>
            /// 文件或目录重命名时触发的事件
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnFileRenamed(object sender, RenamedEventArgs e)
            {
                try
                {
                    //创建消息，并压入队列中
                    //FileChangeInformation info = new FileChangeInformation(Guid.NewGuid().ToString(), FileChangeType.Rename, e.OldFullPath, e.FullPath, e.OldName, e.Name);
                    //_queue.Enqueue(info);
                }
                catch
                {
                   // Console.WriteLine("File_Watcher_Start:");
                    Close();
                }
            }



    }
}
