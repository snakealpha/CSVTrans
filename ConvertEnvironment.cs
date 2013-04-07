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
        private string configOutputPath;
        private Encoding convertEncoding;

        public ConvertEnvironment(string csvPath, string xmlOutputPath, string configOutputPath, string metaPath, Encoding encoding)
        {
            this.csvPath = csvPath;
            this.xmlOutputPath = xmlOutputPath;
            this.metaPath = metaPath;
            this.convertEncoding = encoding;
            this.configOutputPath = configOutputPath;
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
        /// 复合配置文件输出路径
        /// </summary>
        public string ConfigOutputPath
        {
            get
            {
                return configOutputPath;
            }
        }
    }
}
