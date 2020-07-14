using CodeGenerator.Core.Database;
using CodeGenerator.Core.Wildcards;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace CodeGenerator.Core
{
    public class Engine
    {
        private XmlDocument _XMLDocTempplateFile { get; set; }
        private XmlDocument _XMLDocMappingTypesFile { get; set; }
        private string _Main { get; set; }
        private List<InformationSchemaColumns> _InformationSchemaColumns { get; set; }

        private string proyectName;

        public Func<Wildcard, string, string, string> CustomAction;

        public List<string> ExcludedTablesNames { get; set; }

        public Engine(string xmlTemplateFilePath, string xmlMappingTypesFilePath, string proyectName)
        {
            _XMLDocTempplateFile = new XmlDocument();
            _XMLDocTempplateFile.Load(xmlTemplateFilePath);
            _Main = _XMLDocTempplateFile.SelectNodes(@"/TEMPLATE_FILE/MAIN")[0].InnerText;

            _XMLDocMappingTypesFile = new XmlDocument();
            _XMLDocMappingTypesFile.Load(xmlMappingTypesFilePath);

            this.proyectName = proyectName;

            ExcludedTablesNames = new List<string>();
        }

        public string Generate(List<InformationSchemaColumns> informationSchemaColumns, string folderPath = null, string filePrefix = null, string filePostfix = null)
        {
            string result = string.Empty;
            string template = string.Empty;

            _InformationSchemaColumns = informationSchemaColumns;

            List<string> wildcards = Wildcard.GetWildcards(_Main);

            List<string> repeatTableList = GetRepeatersTables();

            foreach (string repeaterTable in repeatTableList)
            {
                Console.WriteLine(repeaterTable);
                template = _Main;

                foreach (string wildcard in wildcards)
                {
                    template = Wildcard.ReplaceWildcard(template, wildcard, WildcardHandler(JsonConvert.DeserializeObject<Wildcard>(wildcard), repeaterTable, string.Empty));
                }

                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    File.WriteAllText($"{folderPath}\\{filePrefix}{repeaterTable}{filePostfix}.cs", template);
                }

                result += template;
            }

            return result;
        }

        //public string Generate(List<InformationSchemaColumns> informationSchemaColumns)
        //{
        //    //string result = string.Empty;
        //    //string template = string.Empty;

        //    //_InformationSchemaColumns = informationSchemaColumns;

        //    //List<string> wildcards = Wildcard.GetWildcards(_Main);

        //    //List<string> repeatTableList = GetRepeatersTables();

        //    //foreach (string repeaterTable in repeatTableList)
        //    //{
        //    //    Console.WriteLine(repeaterTable);
        //    //    template = _Main;

        //    //    foreach (string wildcard in wildcards)
        //    //    {
        //    //        template = Wildcard.ReplaceWildcard(template, wildcard, WildcardHandler(JsonConvert.DeserializeObject<Wildcard>(wildcard), repeaterTable, string.Empty));
        //    //    }

        //    //    result += template;
        //    //}

        //    //return result;
        //    return Generate(informationSchemaColumns, string.Empty)
        //}

        public string GenerateInOneClass(List<InformationSchemaColumns> informationSchemaColumns, string fileFullPath = null)
        {
            string result = string.Empty;
            string template = string.Empty;

            _InformationSchemaColumns = informationSchemaColumns;

            List<string> wildcards = Wildcard.GetWildcards(_Main);

            List<string> repeatTableList = GetRepeatersTables();


                template = _Main;

                foreach (string wildcard in wildcards)
                {
                    template = Wildcard.ReplaceWildcard(template, wildcard, WildcardHandlerOneClass(JsonConvert.DeserializeObject<Wildcard>(wildcard), "", string.Empty));
                }

                result += template;

            if (!string.IsNullOrEmpty(fileFullPath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileFullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath));
                }

                File.WriteAllText(fileFullPath, result);
            }

            return result;
        }

        private string WildcardHandler(Wildcard wildcard, string tableName, string currentValue)
        {
            string result = string.Empty;

            string template = string.Empty;

            if (wildcard.Type == "TEMPLATE")
            {
                IEnumerable<string> repeatByList = GetRepeaters(tableName, wildcard.RepeatBy);
                if (wildcard.Exclude != null) 
                {
                    repeatByList = repeatByList.Except(wildcard.Exclude);
                }

                foreach (string repeaterValue in repeatByList)
                {
                    template = GetTemplate(wildcard.Name);
                    List<string> wildcards = Wildcard.GetWildcards(template);
                    foreach (string w in wildcards)
                    {
                        template = Wildcard.ReplaceWildcard(template, w, WildcardHandler(JsonConvert.DeserializeObject<Wildcard>(w), tableName, repeaterValue));
                    }

                    result += template;
                }
            }
            else
            {
                result = GetValue(wildcard, tableName, currentValue);
            }

            return result;
        }

        private string GetValue(Wildcard wildcard, string tableName, string currentValue)
        {
            string result;
            switch (wildcard.Name)
            {
                case "PROYECT":
                    currentValue = proyectName;
                    break;
                case "TABLE_NAME":
                    currentValue = tableName;
                    break;
                case "DATA_TYPE":
                    InformationSchemaColumns isc_dataType = GetMetadataByTableName(tableName).SingleOrDefault(m => m.COLUMN_NAME == currentValue);
                    if (isc_dataType != null)
                    {
                        currentValue = _XMLDocMappingTypesFile.SelectSingleNode($"TYPES/TYPE[MSSQL_TYPE='{isc_dataType.DATA_TYPE}']").SelectSingleNode("MAPPING").InnerText;
                    }

                    break;
                case "DATA_TYPE_DEPENDENCIES":
                    InformationSchemaColumns isc_dataType_dependencies = GetMetadataByTableName(tableName).SingleOrDefault(m => m.COLUMN_NAME == currentValue);

                    if (isc_dataType_dependencies != null)
                    {
                        currentValue = isc_dataType_dependencies.REFERENCED_TABLE_NAME;
                    }

                    break;
                default:
                    if (CustomAction != null)
                    {
                        currentValue = CustomAction(wildcard, tableName, currentValue);
                    }
                    break;
            }

            result = ApplyWildcarFormat(wildcard, currentValue);
            return result;
        }

        private string WildcardHandlerOneClass(Wildcard wildcard, string tableName, string currentValue)
        {
            string result = string.Empty;

            string template = string.Empty;

            if (wildcard.Type == "TEMPLATE")
            {
                List<string> repeatByList = GetRepeatersTables();

                foreach (string repeaterValue in repeatByList)
                {
                    template = GetTemplate(wildcard.Name);
                    List<string> wildcards = Wildcard.GetWildcards(template);
                    foreach (string w in wildcards)
                    {
                        template = Wildcard.ReplaceWildcard(template, w, WildcardHandler(JsonConvert.DeserializeObject<Wildcard>(w), repeaterValue, repeaterValue));
                    }

                    result += template;
                }
            }
            else
            {
                result = GetValue(wildcard, tableName, currentValue);
            }

            return result;
        }
        
        private List<string> GetRepeaters(string tableName, string repeatBy)
        {

            List<string> result = new List<string>();

            switch (repeatBy)
            {
                case "DEPENDENT_TABLE_NAME":
                    var queryREFERENCED_TABLE_NAME = from isc in _InformationSchemaColumns
                                                     where isc.REFERENCED_TABLE_NAME != string.Empty && isc.REFERENCED_TABLE_NAME == tableName
                                                     group isc.TABLE_NAME by isc.TABLE_NAME;

                    foreach (var values in queryREFERENCED_TABLE_NAME.ToList())
                    {
                        result.Add(values.Key);
                    }
                    break;

                case "DEPENDENCIES_TABLE_NAME":
                    var queryREFERENCED_TABLE_NAME1 = from isc in _InformationSchemaColumns
                                                      where isc.REFERENCED_TABLE_NAME != string.Empty && isc.TABLE_NAME == tableName
                                                      select isc;

                    foreach (var values in queryREFERENCED_TABLE_NAME1)
                    {
                        result.Add(values.COLUMN_NAME);
                    }
                    break;

                case "COLUMN_NAME":
                    var queryCOLUMN_NAME = from isc in _InformationSchemaColumns
                                           where isc.COLUMN_NAME != string.Empty && isc.REFERENCED_TABLE_NAME == string.Empty && isc.TABLE_NAME == tableName
                                           group isc.COLUMN_NAME by isc.COLUMN_NAME;

                    foreach (var values in queryCOLUMN_NAME.ToList())
                    {
                        result.Add(values.Key);
                    }
                    break;
                case "TABLE_NAME":
                    result = GetRepeatersTables(); 
                    break;
                case "JUST_ONE":
                    result.Add(tableName);
                    break;
                default:
                    break;
            }

            return result;
        }
           

        public List<InformationSchemaColumns> GetMetadataByTableName(string tableName)
        {
            List<InformationSchemaColumns> result = new List<InformationSchemaColumns>();

            result = (
                from isc in _InformationSchemaColumns
                where isc.TABLE_NAME == tableName
                select isc
                ).ToList();

            return result;
        }

        public InformationSchemaColumns GetMetadataByTableNameAndColumnName(string tableName, string columnName)
        {
            InformationSchemaColumns result = new InformationSchemaColumns();

            result = (
                from isc in _InformationSchemaColumns
                where isc.TABLE_NAME == tableName && isc.COLUMN_NAME == columnName
                select isc
                ).SingleOrDefault();

            return result;
        }

        private List<string> GetRepeatersTables()
        {
            List<string> result = new List<string>();

            var queryTABLE_NAME = from isc in _InformationSchemaColumns
                                  where isc.TABLE_NAME != string.Empty 
                                  group isc.TABLE_NAME by isc.TABLE_NAME;

            foreach (var values in queryTABLE_NAME.ToList())
            {
                if (!ExcludedTablesNames.Contains(values.Key)) 
                {
                    result.Add(values.Key);
                }
            }

            return result;
        }

        private string ApplyWildcarFormat(Wildcard wildcard, string result)
        {
            if (!string.IsNullOrEmpty(wildcard.Transform)) 
            {
                result = Regex.Match(result, wildcard.Transform).Groups[wildcard.TransformGroup].Value;
            }

            switch (wildcard.Format)
            {
                case "FIRST_LETTER_TO_LOWER":
                    result = result.ToLower().First() + result.Substring(1, result.Length - 1);
                    break;
                case "TO_LOWER":
                    result = result.ToLower();
                    break;
                case "TO_UPPER":
                    result = result.ToUpper();
                    break;
                case "CAPITAL_LETTER":
                    result = result.ToUpper().First() + result.Substring(1, result.Length - 1);
                    break;
                default:
                    break;
            }
            
            return result;
        }

        private string GetTemplate(string name)
        {
            string result = string.Empty;

            result = _XMLDocTempplateFile.SelectNodes(@"/TEMPLATE_FILE/TEMPLATES/" + name)[0].InnerText;

            return result;
        }
    }
}

