using System.Collections.Generic;

namespace ConvertSupport
{
    /// <summary>
    /// 转换功能分节点集合
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ConvertTree:List<ConvertNode>
    {
        private string treeName;

        public ConvertTree(string name)
        {
            treeName = name;
        }

        public string TreeName
        {
            get
            {
                return treeName;
            }
        }
    }
}
