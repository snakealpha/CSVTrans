using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ConvertSupport
{
    /// <summary>
    /// 该类为从转换配置文件中直接衍生的转换上下文集合
    /// Author: Snake.L
    /// Date:   Oct.29, 2012
    /// </summary>
    public class ConvertContextCollection:List<ConvertContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertConfigFileFullPath">转换设置文件</param>
        public ConvertContextCollection(string convertConfigFileFullPath)
        {
            XElement convertFile = XElement.Load(convertConfigFileFullPath);
            IEnumerable<XElement> contexts =
                from node in convertFile.Elements(@"ConvList")
                select node;

            foreach (XElement node in contexts)
            {
                Add(ConvertContext.Parse(node));
            }
        }
    }
}
