using System;
using System.IO;
using T4CodeGenerator.MSSQL2CSHARP;

namespace T4CodeGenerator.Main.Helpers
{
    internal class HelperFile
    {
        internal static void CreateFile(string directory, string file, string text)
        {
            if (Configuration.SaveOnDisk)
            {

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string outputFilePath = Path.Combine(directory, file);

                File.WriteAllText(outputFilePath, text);
            }
            Console.WriteLine("--------------------------------");
            Console.WriteLine(directory);
            Console.WriteLine(file);
            Console.WriteLine(text);
        }

        internal static void CopyFolder(string sourceDirectory, string targetDirectory)
        {
            if (Configuration.SaveOnDisk)
            {
                if (!Directory.Exists(sourceDirectory))
                {
                    throw new Exception("Source does not exist.");
                }
                if (Directory.Exists(targetDirectory))
                {
                    throw new Exception("Target already exist.");
                }

                var diSource = new DirectoryInfo(sourceDirectory);
                var diTarget = new DirectoryInfo(targetDirectory);

                CopyAll(diSource, diTarget);
            }
            else
            {
                Console.WriteLine("--------------------------------");
            }
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
