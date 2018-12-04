using System;
using System.IO;
using System.Timers;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// Just move pdf files from folder in path1 to folder in path2 timing 30s to 30s
        /// @author rundeploy
        /// @date 4/12/2018
        /// </summary>
        static void Main()
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample."); 
            while (Console.Read() != 'q') ; // Mod this in order to run forever
        }

        /// <summary>
        /// Actual method that moves the files
        /// </summary>
        /// <param name="source"></param>
        /// <param name="a"></param>
        private static void OnTimedEvent(object source, ElapsedEventArgs a)
        {
            //[TODO] ask user the FROM folder and the TO folder files to move
            string path1 = @"D:\Downloads\Pasta1"; // Specify folder from you want to move files
            string path2 = @"D:\Downloads\Pasta2"; // Specify folder where you want the files be moved to

            var pdfFileNames = GetFileNames(path1, "*.pdf");
            

            try
            {
                foreach (string name in pdfFileNames)
                {
                    if (!Directory.Exists(path2))
                    {
                        Directory.CreateDirectory(path2);
                    }
                    var concNumber = 0;
                    // Ensure that the target does not exist.
                    if (File.Exists(Path.Combine(path2, name)))
                    {
                        //File.Delete(Path.Combine(path2, name));
                        //File.Create(Path.Combine(path2, name + concNumber++));
                        //File.Copy(name, name + concNumber++);
                        string[] newName = name.Split('.'); //Criar um metodo auxiliar e recursivo para verificar o nome
                        File.Move(Path.Combine(path1, name), Path.Combine(path2, newName[0] + concNumber++ + ".pdf"));
                        Console.WriteLine("{0} was moved to {1}.", path1, path2);
                    }

                    //[TODO] check if there are arleady files with the same name and same format then to do something about it before moving

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

        /// <summary>
        /// Check file names from the folder indicated in the path and return an array of file names
        /// </summary>
        /// <param name="path">The path to the origin folder where files are and should be moved</param>
        /// <param name="filtro">files format</param>
        /// <returns>return a list of file names in a form of an array</returns>
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
