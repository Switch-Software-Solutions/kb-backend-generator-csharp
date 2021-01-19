using System.IO;
using T4CodeGenerator.MSSQL2CSHARP;
using T4CodeGenerator.MSSQL2CSHARP.Templates.SQLSERVER.NETCORE.CSHARP;
using T4CodeGenerator.Main.Helpers;

namespace T4CodeGenerator.Main.Generators
{
    public static class CoreGenerator
    {
        public static void Generate()
        {
             HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Core", "Repositories"), "IRepository.cs", new CoreIRepositoryCS().TransformText());

             HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Core"), "IUnitOfWork.cs", new CoreIUnitOfWorkCS().TransformText());

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                 HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Core", "Models"), $@"{tableName}.cs", new CoreModel().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                 HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Core", "Repositories"), $@"I{tableName}Repository.cs", new CoreIRepository().TransformText());
            }

            foreach (string tableName in Configuration.PurgeExcludedClasses(Configuration.MSSQL.GetRepeatersTables()))
            {
                Configuration.TableName = tableName;
                 HelperFile.CreateFile(Path.Combine(Configuration.BasePath, "Business", "Core", "Services"), $@"I{tableName}Service.cs", new CoreIService().TransformText());
            }

        }
    }
}
