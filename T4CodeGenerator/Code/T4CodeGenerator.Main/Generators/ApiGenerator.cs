using System.Collections.Generic;
using System.IO;
using T4CodeGenerator.MSSQL2CSHARP;
using T4CodeGenerator.MSSQL2CSHARP.Database;
using T4CodeGenerator.MSSQL2CSHARP.Templates.SQLSERVER.NETCORE.CSHARP;
using T4CodeGenerator.Main.Helpers;

namespace T4CodeGenerator.Main.Generators
{
    public static class ApiGenerator
    {
        public static void Generate()
        {
            HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api"), "appsettings.Development.json", new ApiAppSettingsJSON().TransformText());

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}"), $"{tableName}Controller.cs", new ApiEntityController().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Request", "Create"), $"{tableName}RequestCreate.cs", new ApiEntityRequestCreate().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Request", "Create"), $"{tableName}RequestCreateValidator.cs", new ApiEntityRequestCreateValidator().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Request", "Update"), $"{tableName}RequestUpdate.cs", new ApiEntityRequestUpdate().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Request", "Update"), $"{tableName}RequestUpdateValidator.cs", new ApiEntityRequestUpdateValidator().TransformText());
            }

            Configuration.RelatedEntities = new Dictionary<string, List<string>>();

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Response", "Read"), $"{tableName}Item.cs", new ApiEntityResponseItem().TransformText());

                Configuration.RelatedEntities.Add(tableName, new List<string>());

                // Se repite por cada ForeingKey de la tabla
                foreach (InformationSchemaColumns informationSchemaColumn in Configuration.MSSQL.GetRepeatersReferencedTablesInformationFromThis(tableName, Configuration.ExcludedClasses))
                {
                    Configuration.TableName = informationSchemaColumn.REFERENCED_TABLE_NAME;
                    HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Response", "Read", "RelatedEntities"), $"{tableName}Response{informationSchemaColumn.REFERENCED_TABLE_NAME}.cs", new ApiEntityResponseItemRelatedEntity().TransformText());
                    Configuration.RelatedEntities[tableName].Add(Configuration.TableName);
                }
                // Se repite por cada tabla que hace referencia a esta tabla 
                foreach (InformationSchemaColumns informationSchemaColumn in Configuration.MSSQL.GetRepeatersReferencedTablesNamesToThis(tableName, Configuration.ExcludedClasses))
                {
                    Configuration.TableName = informationSchemaColumn.TABLE_NAME;
                    HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $"{tableName}", "Response", "Read", "RelatedEntities"), $"{tableName}Response{informationSchemaColumn.TABLE_NAME}.cs", new ApiEntityResponseItemRelatedEntity().TransformText());
                    Configuration.RelatedEntities[tableName].Add(Configuration.TableName);
                }
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Endpoints", $@"{tableName}", "Response", "List"), $@"{tableName}ItemList.cs", new ApiEntityResponseItemList().TransformText());
            }

            HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api", "Mapping"), "MappingProfile.cs", new ApiMappingCS().TransformText());

            HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Api"), "Startup.cs", new ApiStartupCS().TransformText());
        }
    }
}
