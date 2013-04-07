using System.Collections.Generic;

namespace ConvertSupport
{
    /// <summary>
    /// 转换节点...经历了白痴般的重重嵌套之后,终于快抵达有实际信息的节点了...
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ConvertNode:List<ResourceInformation>
    {
        private string nodeName;

        public ConvertNode(string name)
        {
            nodeName = name;
        }

        public string NodeName
        {
            get
            {
                return nodeName;
            }
        }
    }
}
