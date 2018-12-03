using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// Just move pdf files from folder in path1 to folder in path2
        /// @author rundeploy
        /// </summary>
        static void Main()
        {
            string path1 = @"D:\Downloads\Pasta1";
            string path2 = @"D:\Downloads\Pasta2";


            var pdfFileNames = GetFileNames(path1, "*.pdf");


            try
            {
                foreach (string name in pdfFileNames)
                {
                    if (!Directory.Exists(path2))
                    {
                        Directory.CreateDirectory(path2);
                    }

                    // Ensure that the target does not exist.
                    if (File.Exists(Path.Combine(path2, name)))
                    {
                        File.Delete(Path.Combine(path2, name));
                    }

                    // Move the file.
                    File.Move(Path.Combine(path1, name), Path.Combine(path2, name));
                    Console.WriteLine("{0} was moved to {1}.", path1, path2);


                    // See if the original exists now.
                    if (File.Exists(path1 + pdfFileNames))
                    {
                        Console.WriteLine("The original file still exists, which is unexpected.");
                    }
                    else
                    {
                        Console.WriteLine("The original file no longer exists, which is expected.");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

        }

        private static string[] GetFileNames(string path, string filtro)
        {
            var pdfFiles = new DirectoryInfo(path).GetFiles(filtro);
            string[] fileNames = new string[pdfFiles.Length];
            for (int i = 0; i < pdfFiles.Length; i++)
                fileNames[i] = pdfFiles[i].Name;

            return fileNames;
        }

    }
}
