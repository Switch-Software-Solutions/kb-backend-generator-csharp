using CodeGenerator.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL2EF6.Projects
{
    public static class MyMusic
    {
        static CodeGenerator.Core.Engine e;

        public static void Generate()
        {
            string CONNECTION_STRING = @"
                Password=Nc123123;
                Persist Security Info=True;
                User ID=sa;
                Initial Catalog=MyMusic;
                Data Source=localhost\SQLEXPRESS";

            string ROOT_PATH = @"D:\Temp\PRUEBASMSSQLTOKNEX\MyMusic";

            string proyectName = "MyMusic";


            CodeGenerator.Core.Database.MSSQL _MSSQL = new CodeGenerator.Core.Database.MSSQL(CONNECTION_STRING);

            
            //e = new CodeGenerator.Core.Engine(@"./Templates/Testing/Test.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            //e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            //Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\Test2\\Program.cs"));

            //e = new CodeGenerator.Core.Engine(@"./Templates/Testing/Handler.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            //e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            //Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\Testing\\Handlers", string.Empty, "Handler"));


            //e = new CodeGenerator.Core.Engine(@"./Templates/Back/Core.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            //e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            //Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\Core", string.Empty, string.Empty));


            //e = new CodeGenerator.Core.Engine(@"./Templates/Back/DataAccessService.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            //e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            //Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\DataAccess\\Services", string.Empty, string.Empty));


            e = new CodeGenerator.Core.Engine(@"./Templates/Back/DTO.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            e.CustomAction = CustomAction;
            e.ExcludedTablesNames.Add("__EFMigrationsHistory");
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\DTO", string.Empty, string.Empty));

            Console.ReadKey();
        }

        private static string CustomAction(CodeGenerator.Core.Wildcards.Wildcard wildcard, string tableName, string currentValue)
        {
            string result = string.Empty;
            
            InformationSchemaColumns isc;
            
            switch (wildcard.Name)
            {
                

                case "VALIDATORS.RULES":
                    isc = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += isc.IS_NULLABLE ? string.Empty : $".NotEmpty().WithMessage(\"'{currentValue} is required.\");";
                    result += isc.CHARACTER_MAXIMUM_LENGTH == 0 ? string.Empty : $".MaximumLength({isc.CHARACTER_MAXIMUM_LENGTH});";

                    break;
                case "DATA.CONFIGURATIONS_FIELDS":
                    isc  = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += isc.PRIMARY_KEY ? string.Empty : $"builder.HasKey({tableName.ToLower()} => {tableName.ToLower()}.{currentValue}); ";

                    result += $"builder.Property({tableName.ToLower()} => {tableName.ToLower()}.{currentValue})";
                    result += isc.IS_NULLABLE ? string.Empty : $".IsRequired()";
                    result += isc.CHARACTER_MAXIMUM_LENGTH == 0 ? string.Empty : $".HasMaxLength({isc.CHARACTER_MAXIMUM_LENGTH});";
                    result += isc.IS_IDENTITY ? ".UseIdentityColumn()" : string.Empty;

                    break;
                case "DATA.DEPENDENT_CONFIGURATIONS":
                    isc = e.GetMetadataByTableNameAndColumnName(tableName, currentValue);

                    result += $"builder" + Environment.NewLine;
                    result += $"    .HasOne({tableName.ToLower()} => {tableName.ToLower()}.Artist)" + Environment.NewLine;
                    result += $"    .WithMany({isc.REFERENCED_TABLE_NAME.ToLower()} => {isc.REFERENCED_TABLE_NAME.ToLower()}.{tableName})" + Environment.NewLine;
                    result += $"    .HasForeignKey({tableName.ToLower()} => {tableName.ToLower()}.{currentValue});";

                    break;

                    /*
                                 builder
                .HasOne(m => m.Artist)
                .WithMany(a => a.Musics)
                .HasForeignKey(m => m.ArtistId);
                     */
                    //result += isc.IS_NULLABLE ? string.Empty : $".NotEmpty().WithMessage(\"'{currentValue} is required.\");";
                    //result += isc.CHARACTER_MAXIMUM_LENGTH == 0 ? string.Empty : $".MaximumLength({isc.CHARACTER_MAXIMUM_LENGTH});";

                    break;
                default:
                    result = currentValue;
                    break;
            }

            return result;
        }

    }
}
