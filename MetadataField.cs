using System.Xml.Linq;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 元数据类中的单个字段
    /// Author: Snake.L
    /// Date:   Nov.11 2012
    /// </summary>
    public class MetadataField
    {
        private string name;
        private string type;
        private string column;
        private bool isCollection;
        private string countString;

        private string description;

        public MetadataField(string name, string type, string column, bool isCollection, string description = "")
        {
            this.name = name;
            this.type = type;
            this.column = column;
            this.isCollection = isCollection;
            this.description = description;
        }

        public MetadataField(XElement rawFieldData)
        {
            name = rawFieldData.Attribute(@"name").Value;
            type = rawFieldData.Attribute(@"type").Value;
            if (rawFieldData.Attribute("cname") != null)
            {
                column = rawFieldData.Attribute(@"cname").Value;
            }
            else
            {
                column = "";
            }
            isCollection = rawFieldData.Attribute(@"count") != null;
            if (isCollection)
            {
                countString = rawFieldData.Attribute(@"count").Value;
            }

            if (rawFieldData.Attribute(@"desc") != null)
            {
                description = rawFieldData.Attribute(@"desc").Value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public string ColumnName
        {
            get
            {
                return column;
            }
        }

        public bool IsCollection
        {
            get
            {
                return isCollection;
            }
        }

        public string CountString
        {
            get
            {
                return countString;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }
    }
}
