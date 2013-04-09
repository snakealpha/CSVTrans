using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.IO;

namespace BlackCatWorkshop.Merge
{
    /// <summary>
    /// 该类被用于存储由各XML文件中读取的Metadata基本信息
    /// Author: Snake.L
    /// Date:   Oct.29, 2012
    /// </summary>
    public class ConvertMeta
    {
        private string name;
        private string package;
        private IEnumerable<MetadataField> fields;
        private Dictionary<string, MetadataField> fieldDictionary = new Dictionary<string, MetadataField>();

        private Dictionary<string, string> asTypeMap = new Dictionary<string, string>();

        public ConvertMeta(XElement rawMetadata)
        {
            if (asTypeMap.Keys.Count == 0)
            {
                asTypeMap.Add("string", "String");
                asTypeMap.Add("int", "int");
                asTypeMap.Add("uint", "uint");
                asTypeMap.Add("double", "Number");
                asTypeMap.Add("float", "Number");
                asTypeMap.Add("number", "Number");
                asTypeMap.Add("collection", "Array");
            }

            package = rawMetadata.Attribute(@"namespace").Value;
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

        public void GenerateAS3Type(string outputPath)
        {
            string codeString = "package " + package + Environment.NewLine +
                                "{" + Environment.NewLine +
                                "    public class " + name + Environment.NewLine +
                                "    {" + Environment.NewLine;

            foreach (MetadataField meta in fieldDictionary.Values)
            {
                string fieldString = "";
                if (meta.Description != null && meta.Description != "")
                {
                    fieldString += "        //" + meta.Description + Environment.NewLine;
                }
                fieldString += "        public var " + meta.Name + ":";
                if (meta.IsCollection)
                {
                    fieldString += asTypeMap["collection"] + " = [];" + Environment.NewLine;
                }
                else
                {
                    if (asTypeMap.ContainsKey(meta.Type))
                    {
                        fieldString += asTypeMap[meta.Type] + ";" + Environment.NewLine;
                    }
                }
                codeString += fieldString + Environment.NewLine;
            }

            codeString += "    }" + Environment.NewLine +
                          "}";

            StreamWriter output = File.CreateText(outputPath + "\\" + Name + ".as");
            try
            {
                output.Write(codeString);
            }
            finally
            {
                output.Close();
            }
        }

        public void GenerateHeadType(string outputPath)
        {
            string codeString = "#ifndef Macros_Load" + Environment.NewLine +
                                "#include Macros.h" + Environment.NewLine +
                                "#endif" + Environment.NewLine +
                                Environment.NewLine+
                                "struct " + name + Environment.NewLine +
                                "{" + Environment.NewLine;
            foreach (MetadataField field in fieldDictionary.Values)
            {
                if (field.Type == "string")
                {
                    codeString += "    char[" + field.StringSize + "] ";
                }
                else
                {
                    codeString += "    " + field.Type + " ";
                }

                if (field.IsCollection)
                {
                    codeString += "[" + field.CountString + "] ";
                }
                codeString += field.Name + ";";
                if (field.Description != null && field.Description != "")
                {
                    codeString += "        //" + field.Description;
                }
                codeString += Environment.NewLine;
            }
            codeString += "};";

            StreamWriter output = File.CreateText(outputPath + "\\" + Name + ".h");
            try
            {
                output.Write(codeString);
            }
            finally
            {
                output.Close();
            }
        }
    }
}
