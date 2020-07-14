using CodeGenerator.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQL2EFCore.Projects
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

            string ROOT_PATH = @"D:\Temp\PRUEBASMSSQLTODOTNETCORE\MyMusic";

            string proyectName = "MyMusic";

            new Start(e).CreateFiles(CONNECTION_STRING, ROOT_PATH, proyectName);
          
            Console.ReadKey();
        }
    }
}
