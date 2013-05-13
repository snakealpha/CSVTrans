using System.Text;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 单个转换进行时所需的环境参数信息
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ConvertEnvironment
    {
        private string csvPath;
        private string xmlOutputPath;
        private string metaPath;
        private string headPath;
        private string binPath;
        private string asPath;

        private bool genXml = true;
        private bool genBin = true;
        private bool genAS = true;
        private bool genHead = true;

        private Encoding convertEncoding;

        public ConvertEnvironment(string csvPath, 
                                  string metaPath, 
                                  string xmlOutputPath, 
                                  string headPath,
                                  string binPath,
                                  string asPath,
                                  Encoding encoding)
        {
            this.csvPath = csvPath;
            this.xmlOutputPath = xmlOutputPath;
            this.metaPath = metaPath;
            this.convertEncoding = encoding;
            this.headPath = headPath;
            this.binPath = binPath;
            this.asPath = asPath;

            if (xmlOutputPath == null)
            {
                genXml = false;
            }
            if (binPath == null)
            {
                genBin = false;
            }
            if (asPath == null)
            {
                genHead = false;
            }
            if (headPath == null)
            {
                genHead = false;
            }
        }

        /// <summary>
        /// CSV文件路径
        /// </summary>
        public string CsvPath
        {
            get
            {
                return csvPath;
            }
        }

        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string XmlOutputPath
        {
            get
            {
                return xmlOutputPath;
            }
        }

        /// <summary>
        /// 元文件基路径
        /// </summary>
        public string MetaPath
        {
            get
            {
                return metaPath;
            }
        }

        /// <summary>
        /// 转换编码
        /// </summary>
        public Encoding ConvertEncoding
        {
            get
            {
                return convertEncoding;
            }
        }

        /// <summary>
        /// 头文件的输出路径
        /// </summary>
        public string HeadOutputPath
        {
            get
            {
                return headPath;
            }
        }

        /// <summary>
        /// 二进制文件的输出路径
        /// </summary>
        public string BinPath
        {
            get
            {
                return binPath;
            }
        }

        /// <summary>
        /// AS文件的输出路径
        /// </summary>
        public string AsPath
        {
            get
            {
                return asPath;
            }
        }

        public bool GenXml
        {
            get
            {
                return genXml;
            }
        }

        public bool GenBin
        {
            get
            {
                return genBin;
            }
        }

        public bool GenAs
        {
            get
            {
                return genAS;
            }
        }

        public bool GenHead
        {
            get
            {
                return genHead;
            }
        }
    }
}
