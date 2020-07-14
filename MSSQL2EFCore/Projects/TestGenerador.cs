using CodeGenerator.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL2EFCore.Projects
{
    public static class TestGenerador
    {
        static CodeGenerator.Core.Engine e;

        public static void Generate()
        {
            string CONNECTION_STRING = @"
                Password=test123123;
                Persist Security Info=True;
                User ID=usr_testgenerador;
                Initial Catalog=TestGenerador;
                Data Source=localhost\SQLEXPRESS";

            string ROOT_PATH = @"D:\Temp\PRUEBASMSSQLTODOTNETCORE";

            string proyectName = "TestGenerador";

            new Start(e).CreateFiles(CONNECTION_STRING, ROOT_PATH, proyectName);

            Console.ReadKey();
        }
    }
}
