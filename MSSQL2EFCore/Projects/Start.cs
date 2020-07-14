using CodeGenerator.Core.Database;
using System;
using System.Text.RegularExpressions;

namespace MSSQL2EFCore.Projects
{
    public class Start
    {
        CodeGenerator.Core.Engine e;

        public Start(CodeGenerator.Core.Engine e)
        {
            this.e = e;
        }

        public void CreateFiles(string CONNECTION_STRING, string ROOT_PATH, string proyectName)
        {
            CodeGenerator.Core.Database.MSSQL _MSSQL = new CodeGenerator.Core.Database.MSSQL(CONNECTION_STRING);

            // Api/Controllers
            e = new CodeGenerator.Core.Engine(@"./Templates/Api/Controllers/ControllerTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Api\\Controllers\\", string.Empty, "Controller"));

            // Api/Mapping/MappingProfile.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Api/Mapping/MappingTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Api\\Mapping\\MappingProfile.cs"));

            // Api/Resources
            e = new CodeGenerator.Core.Engine(@"./Templates/Api/Resources/ResourceTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Api\\Resources\\", string.Empty, "Resource"));

            // Api/Validators
            e = new CodeGenerator.Core.Engine(@"./Templates/Api/Validators/ValidatorTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            e.CustomAction = GetCustomValue;
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Api\\Validators\\", string.Empty, "Validator"));

            // Api/Startup.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Api/StartupTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Api\\Startup.cs"));

            // Core/Models
            e = new CodeGenerator.Core.Engine(@"./Templates/Core/Models/ModelTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Core\\Models\\"));

            // Core/Repositories
            e = new CodeGenerator.Core.Engine(@"./Templates/Core/Repositories/RepositoryTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Core\\Repositories\\", "I", "Repository"));

            // Core/IRepository.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Core/Repositories/IRepositoryTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Core\\Repositories\\IRepository.cs"));

            // Core/Services
            e = new CodeGenerator.Core.Engine(@"./Templates/Core/Services/ServiceTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Core\\Services\\", "I", "Service"));

            // Core/IUnitOfWork.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Core/IUnitOfWorkTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Core\\IUnitOfWork.cs"));

            // Data/Configurations
            e = new CodeGenerator.Core.Engine(@"./Templates/Data/Configurations/ConfigurationTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            e.CustomAction = GetCustomValue;
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Data\\Configurations\\", string.Empty, "Configuration"));

            // Data/Repositories
            e = new CodeGenerator.Core.Engine(@"./Templates/Data/Repositories/EntityRepositoryTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Data\\Repositories\\", string.Empty, "Repository"));

            // Data/Repositories/Repository.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Data/Repositories/RepositoryTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Data\\Repositories\\Repository.cs"));

            // Data/DbContext.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Data/DbContextTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Data\\DbContext.cs"));

            // Data/UnitOfWork.cs
            e = new CodeGenerator.Core.Engine(@"./Templates/Data/UnitOfWorkTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Data\\UnitOfWork.cs"));

            // Services
            e = new CodeGenerator.Core.Engine(@"./Templates/Services/ServiceTemplate.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\{proyectName}\\Services\\", string.Empty, "Service"));
        }

        public string GetCustomValue(CodeGenerator.Core.Wildcards.Wildcard wildcard, string tableName, string currentValue)
        {
            string result = string.Empty;

            InformationSchemaColumns isc;

            switch (wildcard.Name)
            {
                case "VALIDATORS.RULES":
                    isc = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += isc.IS_NULLABLE ? string.Empty : $".NotEmpty().WithMessage(\"'{currentValue} is required.\")";
                    result += isc.CHARACTER_MAXIMUM_LENGTH == 0 ? string.Empty : $".MaximumLength({isc.CHARACTER_MAXIMUM_LENGTH})";

                    break;
                case "DATA.CONFIGURATIONS_FIELDS":
                    isc = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += isc.PRIMARY_KEY ? $"builder.HasKey({tableName.ToLower()} => {tableName.ToLower()}.{currentValue}); " : string.Empty;

                    result += $"builder.Property({tableName.ToLower()} => {tableName.ToLower()}.{currentValue})";
                    result += isc.IS_NULLABLE ? string.Empty : $".IsRequired()";
                    result += isc.CHARACTER_MAXIMUM_LENGTH > 0 ? $".HasMaxLength({isc.CHARACTER_MAXIMUM_LENGTH})": string.Empty;
                    result += isc.IS_IDENTITY ? ".UseIdentityColumn()" : string.Empty;

                    break;
                case "DATA.DEPENDENT_CONFIGURATIONS":
                    isc = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += $"builder" + Environment.NewLine;
                    result += $"    .HasOne({tableName.ToLower()} => {tableName.ToLower()}.{Regex.Match(isc.COLUMN_NAME, "ID(.+)$").Groups[1]})" + Environment.NewLine;
                    result += $"    .WithMany({isc.REFERENCED_TABLE_NAME.ToLower()} => {isc.REFERENCED_TABLE_NAME.ToLower()}.{tableName}Collection)" + Environment.NewLine;
                    result += $"    .HasForeignKey({tableName.ToLower()} => {tableName.ToLower()}.{currentValue})";

                    break;
                default:
                    result = currentValue;
                    break;
            }

            return result;
        }
    }
}
