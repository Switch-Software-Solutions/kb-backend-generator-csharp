using System.Collections.Generic;
using System.IO;
using T4CodeGenerator.MSSQL2CSHARP;
using T4CodeGenerator.MSSQL2CSHARP.Database;
using T4CodeGenerator.MSSQL2CSHARP.Templates.SQLSERVER.NETCORE.CSHARP;
using T4CodeGenerator.Main.Helpers;

namespace T4CodeGenerator.Main.Generators
{
    public static class DataGenerator
    {
        public static void Generate()
        {
            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Data", "Configurations"), $@"{tableName}Configuration.cs", new DataConfiguration().TransformText());
            }

            HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Data"), "DbContextInstance.cs", new DataDbContextCS().TransformText());

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Data", "Repositories"), $@"{tableName}Repository.cs", new DataRepository().TransformText());
            }

            HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Data"), "UnitOfWork.cs", new DataUnitOfWork().TransformText());
        }
    }
}
