using System;
using T4CodeGenerator.MSSQL2CSHARP;
using System.IO;
using T4CodeGenerator.Main.Generators;
using T4CodeGenerator.Main.Helpers;

namespace T4CodeGenerator.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConfigGenerator(args);

                CreateProject();

                ApiGenerator.Generate();

                CoreGenerator.Generate();

                DataGenerator.Generate();

                ServicesGenerator.Generate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("Press any key for continue...");
            Console.ReadKey();
        }

        private static void ConfigGenerator(string[] args)
        {
            if (args.Length != 14)
            {
                throw new Exception($"Argument amount error.  {args.Length}");
            }

            for (int i = 0; i < 14; i += 2)
            {
                switch (args[i].ToLower())
                {
                    case "-sd":
                    case "--saveondisk":
                        Configuration.SaveOnDisk = bool.Parse(args[i + 1]);
                        break;
                    case "-pn":
                    case "--projectname":
                        Configuration.ProjectName = args[i + 1];
                        break;
                    case "-bp":
                    case "--basepath":
                        Configuration.BasePath = args[i + 1];
                        break;
                    case "-dp":
                    case "--databasepassword":
                        Configuration.DatabasePassword = args[i + 1];
                        break;
                    case "-du":
                    case "--databaseusername":
                        Configuration.DatabaseUserName = args[i + 1];
                        break;
                    case "-dn":
                    case "--databasename":
                        Configuration.DatabaseName = args[i + 1];
                        break;
                    case "-dsp":
                    case "--databaseserver":
                        Configuration.DatabaseServer = args[i + 1];
                        break;
                    default:
                        throw new Exception($"Argument {args[i]} not allowed");
                }
            }

            Configuration.BasePath = Path.Combine(Configuration.BasePath, Configuration.ProjectName);
            Configuration.DataTypeMapping = @"Resources\DataTypeMapping.xml";
        }

        private static void CreateProject()
        {
            if (Configuration.SaveOnDisk)
            {
                string emptyProjectDirectory = @"D:\Proyectos\T4CodeGenerator\T4CodeGenerator.MSSQL2CSHARP\Templates\SQLSERVER\NETCORE\CSHARP\EMPTY_PROJECT";

                HelperFile.CopyFolder(emptyProjectDirectory, Configuration.BasePath);

                File.Move(Path.Combine(Configuration.BasePath, "TemplateEmpty.sln"), Path.Combine(Configuration.BasePath, $"{Configuration.ProjectName}.sln"));
            }
        }
    }
}
