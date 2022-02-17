using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ftp_File_Share
{
    class My_Event_Class
    {
        public string filename;
        public delegate void myUpEventsHandler(string _filename);
        public event myUpEventsHandler startUpEvent;

        public My_Event_Class()
        { }

        public My_Event_Class(string _filename)
        {
            filename = _filename;
        }

        public void mythreadStart()
        {
            Thread thr = new Thread(new ThreadStart(this.start_Event));
            thr.Start();
        }

        public void start_Event()
        {
            startUpEvent(filename);
        }



    }
}
