﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        /// <summary>
        /// 传导获取的全局环境文件
        /// 用于拼合各输入及输出文件的完全限定路径
        /// </summary>
        private ConvertEnvironment environment;

        private string nodeName;

        private string csvFileFullPath;
        private string outputFileFullPath;
        private string metadataName;
        private IEnumerable<string> metadataFileFullPathes;

        public ResourceInformation(XElement resInfoElement, ConvertEnvironment resEnvironment)
        {
            environment = resEnvironment;

            nodeName = resInfoElement.Attribute(@"Name").Value;
            csvFileFullPath = environment.CsvPath + @"\" + resInfoElement.Attribute(@"ExcelFile").Value;
            outputFileFullPath = Regex.Replace(environment.XmlOutputPath + @"\" + resInfoElement.Attribute(@"BinFile").Value, @"bin$", "xml");
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
                return outputFileFullPath;
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
        /// 从Google Protobuff构建元数据库
        /// </summary>
        private void ConstructMetadataLibraryFromProto()
        {
            metadataDictionary = new Dictionary<string, ConvertMeta>();
            macros = new Dictionary<string, int>();
            foreach (string path in metadataFileFullPathes)
            {
                ReadMetadataFromProto(path);
            }
        }

        /// <summary>
        /// 从TDR风格的Xml元数据文件中读取元数据
        /// </summary>
        /// <param name="metadataFilePath"></param>
        public void ReadMetadataFromXml(string metadataFilePath)
        {
            XElement metadataElement = XElement.Load(metadataFilePath);

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

        /// <summary>
        /// 从Google Protobuff风格重的元数据文件中读取元数据
        /// </summary>
        /// <param name="metadataFilePath"></param>
        public void ReadMetadataFromProto(string metadataFilePath)
        {
            // todo：编写protobuff文件分析方法
        }

        public string Name
        {
            get
            {
                return nodeName;
            }
        }
    }
}