using System.Text;

namespace ConvertSupport
{
    /// <summary>
    /// 单个转换进行时所需的环境参数信息
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ConvertEnvironment
    {
        private string csvPath;
        private string outputPath;
        private string metaPath;
        private Encoding convertEncoding;

        public ConvertEnvironment(string csvPath, string outputPath, string metaPath, Encoding encoding)
        {
            this.csvPath = csvPath;
            this.outputPath = outputPath;
            this.metaPath = metaPath;
            this.convertEncoding = encoding;
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
        public string OutputPath
        {
            get
            {
                return outputPath;
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
    }
}
