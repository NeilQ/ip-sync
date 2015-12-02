using System;
using System.IO;
using System.Net.Sockets;
using Ipsync.Util;

namespace Ipsync.Controller
{
    public class Logging
    {
        public static string LogFile;

        public static bool OpenLogFile()
        {
            try
            {
                var temppath = Utils.GetTempPath();
                LogFile = Path.Combine(temppath, "ipsync.log");
                var fs = new FileStream(LogFile, FileMode.Append);
                var sw = new StreamWriterWithTimestamp(fs) { AutoFlush = true };
                Console.SetOut(sw);
                Console.SetError(sw);

                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public static void Log(string text)
        {
            Console.WriteLine(text);
        }

        public static void Debug(object o)
        {

#if DEBUG
            Console.WriteLine(o);
#endif
        }

        public static void LogUsefulException(Exception e)
        {
            // just log useful exceptions, not all of them
            if (e is ObjectDisposedException)
            {
            }
            else
            {
                Console.WriteLine(e);
            }
        }
    }

    // Simply extended System.IO.StreamWriter for adding timestamp workaround
    public class StreamWriterWithTimestamp : StreamWriter
    {
        public StreamWriterWithTimestamp(Stream stream) : base(stream)
        {
        }

        private string GetTimestamp()
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(GetTimestamp() + value);
        }

        public override void Write(string value)
        {
            base.Write(GetTimestamp() + value);
        }
    }
}