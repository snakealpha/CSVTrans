using System;
using System.IO;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 该类用于控制控制台及文件的日志记录
    /// Author: Snake.L
    /// Date:   Nov.6, 2012
    /// </summary>
    public class LogRecorder
    {
        private bool recordInFile = false;
        private string logFilePath;
        private FileInfo logFile;
        private StreamWriter logFileWriter;

        private static LogRecorder recorder;

        private LogRecorder()
        {
        }

        public static LogRecorder Instance()
        {
            if (recorder == null)
            {
                recorder = new LogRecorder();
            }

            return recorder;
        }

        /// <summary>
        /// 独立日志文件的保存路径
        /// </summary>
        public string LogFilePath
        {
            get
            {
                return logFilePath;
            }
            set
            {
                if (value != logFilePath || logFile == null)
                {
                    logFile = new FileInfo(logFilePath);
                    logFileWriter = logFile.AppendText();
                }

                logFilePath = value;
            }
        }

        /// <summary>
        /// 是否记录独立的日志文件
        /// </summary>
        public bool RecordInFile
        {
            get
            {
                return recordInFile;
            }
            set
            {
                recordInFile = value;

                if (logFilePath != null && logFilePath != "")
                {
                    logFile = new FileInfo(logFilePath);
                    logFileWriter = logFile.AppendText();
                }
            }
        }

        /// <summary>
        /// 在日志中记录一串字符串
        /// </summary>
        /// <param name="content">字符串内容</param>
        public void RecordString(string content)
        {
            Console.WriteLine(content);

            if (recordInFile && logFileWriter != null)
            {
                logFileWriter.WriteLine(content);
            }
        }

        /// <summary>
        /// 在日志中记录一个异常
        /// </summary>
        /// <param name="exception">异常对象</param>
        public void RecordException(Exception exception)
        {
            string express = exception.GetType().ToString() + ": " + exception.Message;

            RecordString(express);
        }
    }
}
