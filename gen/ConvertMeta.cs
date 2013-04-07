using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ConvertSupport
{
    /// <summary>
    /// 该类被用于存储由各XML文件中读取的Metadata基本信息
    /// Author: Snake.L
    /// Date:   Oct.29, 2012
    /// </summary>
    public class ConvertMeta
    {
        private string name;
        private IEnumerable<MetadataField> fields;
        private Dictionary<string, MetadataField> fieldDictionary = new Dictionary<string,MetadataField>();

        public ConvertMeta(XElement rawMetadata)
        {
            name = rawMetadata.Attribute(@"name").Value;
            fields = from fieldInfo in rawMetadata.Elements(@"entry")
                     select new MetadataField(fieldInfo);

            foreach (MetadataField meta in fields)
            {
                fieldDictionary.Add(meta.Name, meta);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public IEnumerable<MetadataField> Fields
        {
            get
            {
                return fields;
            }
        }

        public Dictionary<string, MetadataField> FieldDictionary
        {
            get
            {
                return fieldDictionary;
            }
        }
    }
}
