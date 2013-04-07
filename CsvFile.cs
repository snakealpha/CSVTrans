using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 该类用于集成生成CSV数据所使用的方法,以及驻留CSV数据
    /// </summary>
    class CsvFile
    {
        private Dictionary<string, int> csvTitleIndex = new Dictionary<string, int>();
        private List<List<string>> csvElements;

        public CsvFile(string path)
        {
            ParseCsv(path);
        }

        /// <summary>
        /// 经过解析的CSV各行数据列表
        /// </summary>
        public List<List<string>> CsvElements
        {
            get
            {
                return csvElements;
            }
        }

        /// <summary>
        /// CSV文件首行生成的表头/列索引映射表
        /// </summary>
        public Dictionary<string, int> CsvTitleIndex
        {
            get
            {
                return csvTitleIndex;
            }
        }

        /// <summary>
        /// 产生CSV表头数据和内容数据
        /// </summary>
        private void ParseCsv(string path)
        {
            LogRecorder.Instance().RecordString("开始读取CSV文件：" + path);
            List<List<string>> csvElements = new List<List<string>>();

            try
            {
                string[] csvLines = File.ReadAllLines(path, Encoding.Default);
                int titleNum = csvLines[0].Split(',').Length;

                for (int i = 0; i != csvLines.Length; i++)
                {
                    string concatString = csvLines[i];
                    string[] lineElements = concatString.Split(',');

                    while (lineElements.Length < titleNum)
                    {
                        concatString += "\n" + csvLines[++i];
                        lineElements = concatString.Split(',');
                    }

                    //csvElements.Add(ProcessCsvLine(csvLines[i].Split(',')));
                    csvElements.Add(ProcessCsvLine(lineElements));
                }
            }
            catch (IOException e)
            {
                LogRecorder.Instance().RecordException(e);
            }

            List<string> title = csvElements.ElementAt(0);
            csvElements.Remove(title);

            csvTitleIndex.Clear();
            for (int i = 0; i != title.Count; i++)
            {
                if (csvTitleIndex.ContainsKey(title[i]))
                {
                    Random rand = new Random();
                    int newKey = rand.Next();
                    while (csvTitleIndex.ContainsKey(newKey.ToString()))
                    {
                        newKey = rand.Next();
                    }
                    csvTitleIndex.Add(newKey.ToString(), i);
                }
                else
                {
                    csvTitleIndex.Add(title[i], i);
                }
            }

            this.csvElements = csvElements;

            LogRecorder.Instance().RecordString("解析完成" + Environment.NewLine);
        }

        /// <summary>
        /// 处理CSV各行，使其成为可以直接使用的字符串数据
        /// </summary>
        /// <param name="lineContentCollection"></param>
        /// <returns></returns>
        private List<string> ProcessCsvLine(IList<string> lineContentCollection)
        {
            List<string> resultList = new List<string>();

            bool isLinkMode = false;
            string linkString = null;

            for (int i = 0; i != lineContentCollection.Count; i++)
            {
                string curClip = lineContentCollection[i];

                if (isLinkMode)
                {
                    linkString += "," + curClip;

                    if (/*curClip.EndsWith("\"") && !curClip.EndsWith("\"\"")*/IsClipEnd(curClip))
                    {
                        linkString = linkString.Remove(linkString.Length - 1);
                        linkString = Regex.Replace(linkString, "\"\"", "\"");
                        resultList.Add(linkString);
                        isLinkMode = false;
                    }

                    continue;
                }

                if (curClip.StartsWith("\""))
                {
                    if (/*curClip.EndsWith("\"") && !curClip.EndsWith("\"\"")*/IsClipEnd(curClip))
                    {
                        string raw = curClip.Clone() as string;

                        curClip = curClip.Remove(0, 1);
                        curClip = curClip.Remove(curClip.Length - 1);

                        curClip = Regex.Replace(curClip, "\"\"", "\"");
                        resultList.Add(curClip);
                    }
                    else
                    {
                        linkString = curClip.Remove(0, 1);
                        isLinkMode = true;
                    }
                    continue;
                }

                resultList.Add(curClip);
            }

            return resultList;
        }

        private bool IsClipEnd(string clip)
        {
            string ques = Regex.Match(clip, "(\"*)$").Groups[1].Value;
            return ques.Length % 2 != 0;
        }
    }
}
