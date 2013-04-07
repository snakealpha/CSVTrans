using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace BlackCatWorkshop.Merge
{
    public static class XmlWrapper
    {
        /// <summary>
        /// 根据当前收集的上下文解析元数据和CSV并生成输出文件
        /// </summary>
        public static void Generate(ResourceInformation generateInfo)
        {
            CsvFile csvInfo = new CsvFile(generateInfo.CsvPath);

            ExportData(generateInfo, csvInfo);
        }

        private static void ExportData(ResourceInformation generateInfo, CsvFile csvInfo)
        {
            XElement rootElement = WrapGlobal(generateInfo, csvInfo);

            XDocument doc = new XDocument();
            doc.Add(rootElement);
            doc.Save(generateInfo.OutputPath);
        }

        private static XElement WrapGlobal(ResourceInformation generateInfo, CsvFile csvInfo)
        {
            XElement root = new XElement(generateInfo.MetadataName + "_Tab");
            for (int i = 0; i != csvInfo.CsvElements.Count; i++)
            {
                root.Add(WrapElement(generateInfo, csvInfo, csvInfo.CsvElements[i]));
            }

            return root;
        }

        private static XElement WrapElement(
            ResourceInformation generateInfo,
            CsvFile csvInfo,
            List<string> value)
        {
            ConvertMeta meta = generateInfo.Metadata;
            XElement root = new XElement(meta.Name);
            string[] nameList = meta.FieldDictionary.Keys.ToArray<string>();
            for (int i = 0; i != nameList.Length; i++)
            {
                MetadataField field = meta.FieldDictionary[nameList[i]];

                try
                {
                    if (field.IsCollection && (generateInfo.Metadatas.ContainsKey(field.Type) || field.Type == "string"))
                    {
                        for (int j = 0; j != Convert.ToInt32(field.CountString); j++)
                        {
                            root.Add(WrapType(generateInfo, csvInfo, nameList[i], meta, value, true, j, field));
                        }
                    }
                    else
                    {
                        root.Add(WrapType(generateInfo, csvInfo, nameList[i], meta, value));
                    }
                }
                catch (Exception)
                {
                    LogRecorder.Instance().RecordString(string.Format("    尝试转换类型{0}的第{1}行元素，字段{2}(类型{3})时出错，请检查元数据文件及配置表",
                                                                              meta.Name, i + 1, field.CountString, field.Type));
                    return null;
                }
            }

            return root;
        }

        /// <summary>
        /// 这个方法搞不好有点复杂，因为需要根据原语类型，复合类型和数组进行分别处理
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="meta"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static XElement WrapType(
            ResourceInformation generateInfo,
            CsvFile csvInfo,
            string fieldName,
            ConvertMeta meta,
            List<string> value,
            bool isComplexCollection = false,
            int complexCollectionIndex = 0,
            MetadataField baseField = null)
        {
            MetadataField field = meta.FieldDictionary[fieldName];
            XElement root = new XElement(fieldName);

            if (generateInfo.Metadatas.ContainsKey(field.Type))
            {
                WrapComplexType(generateInfo, csvInfo, value, isComplexCollection, complexCollectionIndex, field, root);
            }
            else if (isComplexCollection)
            {
                WrapComplexCollectionType(csvInfo, value, complexCollectionIndex, field, root, baseField);
            }
            else if (field.IsCollection)
            {
                WrapCollectionType(generateInfo, csvInfo, value, field, root);
            }
            else
            {
                WrapPrimitiveType(csvInfo, value, field, root);
            }

            return root;
        }

        private static void WrapPrimitiveType(
            CsvFile csvInfo, 
            List<string> value, 
            MetadataField field, 
            XElement root)
        {
            if (csvInfo.CsvTitleIndex.ContainsKey(field.ColumnName) &&
                value[csvInfo.CsvTitleIndex[field.ColumnName]] != "" &&
                (
                    field.Type == "string" ||
                    Regex.IsMatch(value[csvInfo.CsvTitleIndex[field.ColumnName]], "^-?[0-9]+(\\.[0-9]+)?$")
                )
               )
            {
                root.Value = value[csvInfo.CsvTitleIndex[field.ColumnName]];
            }
            else if (field.Type == "string")
            {
                root.Value = "";
            }
            else
            {
                root.Value = "0";
            }
        }

        private static void WrapCollectionType(
            ResourceInformation generateInfo, 
            CsvFile csvInfo, List<string> value, 
            MetadataField field, 
            XElement root)
        {
            int count = 0;
            if (generateInfo.Macros.ContainsKey(field.CountString))
            {
                count = Convert.ToInt32(generateInfo.Macros[field.CountString]);
            }
            else
            {
                count = Convert.ToInt32(field.CountString);
            }
            for (int i = 0; i != count; i++)
            {
                string column;
                if (csvInfo.CsvTitleIndex.ContainsKey(field.ColumnName + (i + 1).ToString()) &&
                    value[csvInfo.CsvTitleIndex[field.ColumnName + (i + 1).ToString()]] != "")
                {
                    column = value[csvInfo.CsvTitleIndex[field.ColumnName + (i + 1).ToString()]];
                }
                else
                {
                    column = "0";
                }

                root.Value += column + " ";
            }
        }

        private static void WrapComplexCollectionType(
            CsvFile csvInfo, 
            List<string> value, 
            int complexCollectionIndex, 
            MetadataField field, 
            XElement root, 
            MetadataField baseField = null)
        {
            string column;

            if (field.Type == "string" && csvInfo.CsvTitleIndex.ContainsKey(field.ColumnName + (complexCollectionIndex + 1).ToString()))
            {
                column = value[csvInfo.CsvTitleIndex[field.ColumnName + (complexCollectionIndex + 1).ToString()]];
            }
            else if (baseField != null && csvInfo.CsvTitleIndex.ContainsKey(baseField.ColumnName + (complexCollectionIndex + 1).ToString() + field.ColumnName))
            {
                column = value[csvInfo.CsvTitleIndex[baseField.ColumnName + (complexCollectionIndex + 1).ToString() + field.ColumnName]];
                if (column == "")
                    column = "0";
            }
            else
            {
                column = "0";
            }

            root.Value += column;
        }

        private static void WrapComplexType(
            ResourceInformation generateInfo, 
            CsvFile csvInfo, 
            List<string> value, 
            bool isComplexCollection, 
            int complexCollectionIndex, 
            MetadataField field, 
            XElement root)
        {
            root.SetAttributeValue("name", field.Type);
            ConvertMeta subElementMeta = generateInfo.Metadatas[field.Type];

            if (isComplexCollection)
            {
                foreach (MetadataField subField in subElementMeta.Fields)
                {
                    root.Add(WrapType(generateInfo, csvInfo, subField.Name, subElementMeta, value, true, complexCollectionIndex, field));
                }
            }
            else
            {
                foreach (MetadataField subField in subElementMeta.Fields)
                {
                    root.Add(WrapType(generateInfo, csvInfo, subField.Name, subElementMeta, value));
                }
            }
        }
    }
}
