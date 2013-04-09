using System.Collections.Generic;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 转换功能分节点集合
    /// Author: Snake.L
    /// Date:   Oct.30, 2012
    /// </summary>
    public class ConvertTree:List<ConvertNode>
    {
        private string treeName;
        private string nameSpace;

        public ConvertTree(string name, string nameSpace = "")
        {
            treeName = name;
            this.nameSpace = nameSpace;
        }

        public string TreeName
        {
            get
            {
                return treeName;
            }
        }

        public string NameSpace
        {
            get
            {
                return nameSpace;
            }
        }
    }
}
