using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL2EF6.Projects
{
    public static class MNGR
    {
        public static void Generate() 
        {
            string CONNECTION_STRING = @"
                Password=Switch09#mr;
                Persist Security Info=True;
                User ID=usr_mngr;
                Initial Catalog=MNGR;
                Data Source=localhost\SQLEXPRESS";

            string ROOT_PATH = @"D:\Temp\PRUEBASMSSQLTOKNEX\MNGR";

            string proyectName = "MNGR";

            CodeGenerator.Core.Database.MSSQL _MSSQL = new CodeGenerator.Core.Database.MSSQL(CONNECTION_STRING);


            CodeGenerator.Core.Engine e;

            e = new CodeGenerator.Core.Engine(@"./Templates/Testing/Test.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata, $"{ROOT_PATH}\\Test2\\Program.cs"));
            e = new CodeGenerator.Core.Engine(@"./Templates/Testing/Handler.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\Testing\\Handlers", string.Empty, "Handler"));


            e = new CodeGenerator.Core.Engine(@"./Templates/Back/Core.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\Core", string.Empty, string.Empty));


            e = new CodeGenerator.Core.Engine(@"./Templates/Back/DataAccessService.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\DataAccess\\Services", string.Empty, string.Empty));


            e = new CodeGenerator.Core.Engine(@"./Templates/Back/DTO.xml", @"./Types/MSSQL2CSHARP.xml", proyectName);
            Console.WriteLine(e.Generate(_MSSQL.Metadata, $"{ROOT_PATH}\\DTO", string.Empty, string.Empty));


            Console.WriteLine(e.GenerateInOneClass(_MSSQL.Metadata));

            Console.ReadKey();
        }
    }
}
