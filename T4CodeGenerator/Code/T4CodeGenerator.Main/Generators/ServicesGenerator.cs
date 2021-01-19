using System.IO;
using T4CodeGenerator.MSSQL2CSHARP;
using T4CodeGenerator.MSSQL2CSHARP.Templates.SQLSERVER.NETCORE.CSHARP;
using T4CodeGenerator.Main.Helpers;

namespace T4CodeGenerator.Main.Generators
{
    public static class ServicesGenerator
    {
        public static void Generate()
        {
            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Services"), $@"{tableName}Service.cs", new ServiceService().TransformText());
            }
        }
    }
}
