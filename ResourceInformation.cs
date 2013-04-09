using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 资源转换信息节点
    /// 该节点拥有进行资源转换所需要的全部环境信息
    /// 经过重重嵌套之后，九死一生的阁下终于抵达了拥有实际信息的元素
    /// 可喜可贺，可喜可贺
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ResourceInformation
    {
        /// <summary>
        /// 转换所需的元数据存储于此,在该节点首次进行转换时构建
        /// </summary>
        private Dictionary<string, ConvertMeta> metadataDictionary = null;

        private Dictionary<string, int> macros = null;

        private string package;

        /// <summary>
        /// 传导获取的全局环境文件
        /// 用于拼合各输入及输出文件的完全限定路径
        /// </summary>
        private ConvertEnvironment environment;

        private string nodeName;

        private string csvFileFullPath;
        private string xmlOutputFileFullPath;
        private string binOutputFileFullPath;
        private string metadataName;


        private IEnumerable<string> metadataFileFullPathes;

        public ResourceInformation(XElement resInfoElement, ConvertEnvironment resEnvironment)
        {
            environment = resEnvironment;

            nodeName = resInfoElement.Attribute(@"Name").Value;
            csvFileFullPath = environment.CsvPath + @"\" + resInfoElement.Attribute(@"CSVFile").Value;
            xmlOutputFileFullPath = Regex.Replace(environment.XmlOutputPath + @"\" + resInfoElement.Attribute(@"BinFile").Value, @"bin$", "xml");
            binOutputFileFullPath = Regex.Replace(environment.BinPath + @"\" + resInfoElement.Attribute(@"BinFile").Value, @"bin$", "bin");
            metadataName = resInfoElement.Attribute(@"Meta").Value;

            IEnumerable<string> rawMetaSource = resInfoElement.Attribute(@"EntryMapFile").Value.Split(' ');
            metadataFileFullPathes =
                from rawMeta in rawMetaSource
                select environment.MetaPath + @"\" + rawMeta;
        }

        /// <summary>
        /// 获得元数据库
        /// </summary>
        public Dictionary<string, ConvertMeta> Metadatas
        {
            get
            {
                if (metadataDictionary == null)
                {
                    ConstructMetadataLibraryFromXml();
                }

                return metadataDictionary;
            }
        }

        /// <summary>
        /// CSV数据文件路径
        /// </summary>
        public string CsvPath
        {
            get
            {
                return csvFileFullPath;
            }
        }

        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string OutputPath
        {
            get
            {
                return xmlOutputFileFullPath;
            }
        }

        /// <summary>
        /// Bin文件输出路径
        /// </summary>
        public string BinOutputPath
        {
            get
            {
                return binOutputFileFullPath;
            }
        }

        /// <summary>
        /// 用于解析的元数据名称
        /// </summary>
        public string MetadataName
        {
            get
            {
                return metadataName;
            }
        }

        /// <summary>
        /// 用于解析的元数据对象
        /// </summary>
        public ConvertMeta Metadata
        {
            get
            {
                return Metadatas[MetadataName];
            }
        }

        /// <summary>
        /// 获得解析所得的所有宏
        /// </summary>
        public Dictionary<string, int> Macros
        {
            get
            {
                return macros;
            }
        }

        /// <summary>
        /// 构建元数据库
        /// </summary>
        private void ConstructMetadataLibraryFromXml()
        {
            metadataDictionary = new Dictionary<string, ConvertMeta>();
            macros = new Dictionary<string, int>();
            foreach (string path in metadataFileFullPathes)
            {
                ReadMetadataFromXml(path);
            }
        }

        /// <summary>
        /// 从TDR风格的Xml元数据文件中读取元数据
        /// </summary>
        /// <param name="metadataFilePath"></param>
        public void ReadMetadataFromXml(string metadataFilePath)
        {
            XElement metadataElement = XElement.Load(metadataFilePath);

            if (metadataElement.Attribute(@"namespace") != null)
            {
                package = metadataElement.Attribute(@"namespace").Value;
            }

            foreach (XElement ele in metadataElement.Elements(@"struct"))
            {
                ConvertMeta meta = new ConvertMeta(ele);
                metadataDictionary.Add(meta.Name, meta);
            }
            foreach (XElement ele in metadataElement.Elements(@"macro"))
            {
                macros.Add(ele.Attribute("name").Value, Convert.ToInt32(ele.Attribute("value").Value));
            }
        }

        public string GenerateAS3Consts()
        {
            string consts = "";
            foreach (string macro in macros.Keys)
            {
                string constString = "        public static const " + macro + ":int = " + macros[macro] + ";" + Environment.NewLine;
                consts += constString;
            }

            return consts;
        }

        public string GenerateHeadConsts()
        {
            string consts = "";
            foreach (string macro in macros.Keys)
            {
                string constString = "#define " + macro + " " + macros[macro] + Environment.NewLine;
                consts += constString;
            }

            return consts;
        }

        /// <summary>
        /// 生成AS3使用的类型文件
        /// </summary>
        public void GenerateAS3Type()
        {
            foreach (ConvertMeta info in metadataDictionary.Values)
            {
                info.GenerateAS3Type(environment.AsPath);
            }
        }

        /// <summary>
        /// 生成C/C++头文件
        /// </summary>
        public void GenerateHeadType()
        {
            foreach (ConvertMeta info in metadataDictionary.Values)
            {
                info.GenerateHeadType(environment.HeadOutputPath);
            }
        }
    }
}
