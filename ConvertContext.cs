using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 转换上下文主类，保存一个转换的基础信息
    /// Author: Snake.L
    /// Date:   Oct.29, 2012
    /// </summary>
    public class ConvertContext
    {
        private List<ConvertTree> convertTrees = new List<ConvertTree>();
        private ConvertEnvironment environment;

        protected ConvertContext(ConvertEnvironment environment, XElement convertTree)
        {
            this.environment = environment;

            IEnumerable<XElement> trees =
                from tree in convertTree.Elements(@"ConvertConfigs")
                select tree;
            foreach (XElement singleTree in trees)
            {
                ConvertTree treeEntity = new ConvertTree(singleTree.Attribute(@"Name").Value);
                convertTrees.Add(treeEntity);

                IEnumerable<XElement> nodes =
                    from node in singleTree.Elements(@"ConvertNode")
                    select node;
                foreach (XElement singleNode in nodes)
                {
                    ConvertNode nodeEntity = new ConvertNode(singleNode.Attribute(@"Name").Value);
                    treeEntity.Add(nodeEntity);

                    IEnumerable<XElement> resNodes =
                        from res in singleNode.Elements(@"ResourceConfig")
                        select res;
                    foreach (XElement resInfo in resNodes)
                    {
                        ResourceInformation resEntity = new ResourceInformation(resInfo, this.environment);
                        nodeEntity.Add(resEntity);
                    }
                }
            }
        }

        /// <summary>
        /// 由特定XML节点生成转换上下文的工厂方法
        /// </summary>
        /// <param name="contextXml">用于生成转换上下文的XML节点</param>
        /// <returns>经生成的转换上下文</returns>
        public static ConvertContext Parse(XElement contextXml)
        {
            //取得编码-来自综合配置文件的根节点
            XAttribute encodingName = contextXml.Attribute(@"ResEncoding");
            Encoding encoding = Encoding.GetEncoding(encodingName.Value);

            //CSV文件路径
            string baseCvsPath = contextXml.Element(@"CSVFilePath").Attribute(@"Path").Value;
            //元文件路径
            string baseMetaPath = contextXml.Element(@"MetalibPath").Attribute(@"Path").Value;

            //输出的XML文件路径
            string baseOutputPath = contextXml.Element(@"XmlFilesPath").Attribute(@"Path").Value;
            //输出的Bin文件路径
            string baseBinOutputPath = contextXml.Element(@"BinFilesPath").Attribute(@"Path").Value;
            //原文件输出的.h文件路径
            string baseHeadOutputPath = contextXml.Element(@"HeadFilesPath").Attribute(@"Path").Value;
            //原文件输出的As文件路径
            string baseASOutputPath = contextXml.Element(@"ASPath").Attribute(@"Path").Value;

            ConvertEnvironment appxEnvironment = new ConvertEnvironment(baseCvsPath, baseMetaPath, baseOutputPath, baseHeadOutputPath, baseBinOutputPath, baseASOutputPath, encoding);

            return new ConvertContext(appxEnvironment, contextXml);
        }

        /// <summary>
        /// 转换树集合
        /// </summary>
        public List<ConvertTree> ConvertTrees
        {
            get
            {
                return convertTrees;
            }
        }

        /// <summary>
        /// 转换所需的上下文信息
        /// </summary>
        public ConvertEnvironment Environment
        {
            get
            {
                return environment;
            }
        }
    }
}
