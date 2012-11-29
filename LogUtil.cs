using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace syxc.helper
{
    public partial class LogUtil
    {
        public static bool logoff = true;
        public static LogType level = LogType.INFO; // Write file level

        public static void Trace(LogType type, string tag, string msg)
        {
            // Output
            if (logoff)
            {
                switch (type)
                {
                    case LogType.VERBOSE:
                        Log.V(tag, msg);
                        break;
                    case LogType.DEBUG:
                        Log.D(tag, msg);
                        break;
                    case LogType.INFO:
                        Log.I(tag, msg);
                        break;
                    case LogType.WARN:
                        Log.W(tag, msg);
                        break;
                    case LogType.ERROR:
                        Log.E(tag, msg);
                        break;
                }
            }
            // Write to file
            if (type >= level)
            {
                WriteLog(type, tag, msg);
            }
        }

        private static void WriteLog(LogType type, string tag, string msg)
        {
            string logPath = GeneralHelper.CurrentPath() + @"\Log\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            try
            {
                IDictionary<LogType, string> logMap = new Dictionary<LogType, string>();
                logMap.Add(LogType.VERBOSE, " VERBOSE ");
                logMap.Add(LogType.DEBUG, " DEBUG ");
                logMap.Add(LogType.INFO, " INFO ");
                logMap.Add(LogType.WARN, " WARN ");
                logMap.Add(LogType.ERROR, " ERROR ");

                msg = new StringBuilder().Append("\r\n")
                        .Append(GetDateformat(Dateformat.yyyyMMddHHmmss))
                        .Append(logMap[type]).Append(tag)
                        .Append(msg).ToString();

                string fileName = new StringBuilder()
                        .Append(GetDateformat(Dateformat.yyyyMMdd))
                        .Append(".log").ToString();

                RecordLog(logPath, fileName, msg);
            }
            catch (Exception e)
            {
                LogUtil.Trace(LogType.ERROR, "LogUtil: ", e.Message);
            }
        }

        private static void RecordLog(string logPath, string fileName, string msg)
        {
            try
            {
                string filePath = logPath + fileName;
                FileExist(filePath);
                string logTemp = string.Empty;
                logTemp += msg;
                FileStream fs = new FileStream(filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.WriteLine(logTemp);
                sw.Close();
                fs.Close();
            }
            catch (IOException)
            {
                RecordLog(logPath, fileName, msg);
            }
        }

        private static void FileExist(string path)
        {
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Close();
            }
        }

        public static string GetEnumValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        private static string GetDateformat(Dateformat pattern)
        {
            return DateTime.Now.ToString(GetEnumValue(pattern));
        }
    }

    public enum LogType
    {
        VERBOSE,
        DEBUG,
        INFO,
        WARN,
        ERROR
    }

    public enum Dateformat
    {
        [Description("yyyy-MM-dd")]
        yyyyMMdd,
        [Description("yyyy-MM-dd-HH")]
        yyyyMMddHH,
        [Description("yyyy-MM-dd-HH:mm")]
        yyyyMMddHHmm,
        [Description("yyyy-MM-dd-HH:mm:ss")]
        yyyyMMddHHmmss
    }

    public class Log
    {
        public static void V(string tag, string msg)
        {
            Console.WriteLine(" {0} {1} {2}", "VERBOSE", tag, msg);
        }

        public static void D(string tag, string msg)
        {
            Console.WriteLine(" {0} {1} {2}", "DEBUG", tag, msg);
        }

        public static void I(string tag, string msg)
        {
            Console.WriteLine(" {0} {1} {2}", "INFO", tag, msg);
        }

        public static void W(string tag, string msg)
        {
            Console.WriteLine(" {0} {1} {2}", "WARN", tag, msg);
        }

        public static void E(string tag, string msg)
        {
            Console.WriteLine(" {0} {1} {2}", "ERROR", tag, msg);
        }
    }
}
