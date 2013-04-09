using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

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
                ConvertTree treeEntity;
                if (singleTree.Attribute(@"namespace") != null)
                {
                    treeEntity = new ConvertTree(singleTree.Attribute(@"Name").Value, singleTree.Attribute(@"namespace").Value);
                }
                else
                {
                    treeEntity = new ConvertTree(singleTree.Attribute(@"Name").Value);
                }
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

        public void GenerateASConsts()
        {
            foreach (var tree in convertTrees)
            {
                GenerateSingleASConsts(tree);
            }
        }

        public void GenerateCConsts()
        {
            foreach (var tree in convertTrees)
            {
                GenerateSingleCConsts(tree);
            }
        }

        private void GenerateSingleASConsts(ConvertTree tree)
        {
            string consts = "package " + tree.NameSpace + System.Environment.NewLine +
                            "{" + System.Environment.NewLine +
                            "    public class Macros" + System.Environment.NewLine +
                            "    {" + System.Environment.NewLine;

            string collection = "";
            foreach (ConvertNode node in tree)
            {
                foreach (ResourceInformation info in node)
                {
                    collection += info.GenerateAS3Consts();
                }
            }
            consts += collection;

            consts += "    }" + System.Environment.NewLine +
                      "}";

            StreamWriter output = File.CreateText(environment.AsPath + "\\Macros.as");
            try
            {
                output.Write(consts);
            }
            finally
            {
                output.Close();
            }
        }

        private void GenerateSingleCConsts(ConvertTree tree)
        {
            string collection = "#define Macros_Load" + System.Environment.NewLine +
                                System.Environment.NewLine;
            foreach (ConvertNode node in tree)
            {
                foreach (ResourceInformation info in node)
                {
                    collection += info.GenerateHeadConsts();
                }
            }

            StreamWriter output = File.CreateText(environment.HeadOutputPath + "\\Macros.h");
            try
            {
                output.Write(collection);
            }
            finally
            {
                output.Close();
            }
        }
    }
}
