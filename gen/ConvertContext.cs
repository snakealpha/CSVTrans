using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConvertSupport
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
                from tree in convertTree.Elements(@"ConvTree")
                select tree;
            foreach (XElement singleTree in trees)
            {
                ConvertTree treeEntity = new ConvertTree(singleTree.Attribute(@"Name").Value);
                convertTrees.Add(treeEntity);

                IEnumerable<XElement> nodes =
                    from node in singleTree.Elements(@"CommNode")
                    select node;
                foreach (XElement singleNode in nodes)
                {
                    ConvertNode nodeEntity = new ConvertNode(singleNode.Attribute(@"Name").Value);
                    treeEntity.Add(nodeEntity);

                    IEnumerable<XElement> resNodes =
                        from res in singleNode.Elements(@"ResNode")
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
            XAttribute encodingName = contextXml.Attribute(@"ResEncoding");
            Encoding encoding = Encoding.GetEncoding(encodingName.Value);
            if (encodingName.Value == "GBK")
                encoding = Encoding.GetEncoding(936);
            string baseCvsPath = contextXml.Element(@"ExcelFilesPath").Attribute(@"Path").Value;
            string baseOutputPath = contextXml.Element(@"BinFilesPath").Attribute(@"Path").Value;
            string baseMetaPath = contextXml.Element(@"EntryMapFilesPath").Attribute(@"Path").Value;

            ConvertEnvironment appxEnvironment = new ConvertEnvironment(baseCvsPath, baseOutputPath, baseMetaPath, encoding);

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
