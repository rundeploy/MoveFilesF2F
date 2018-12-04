using System;
using System.IO;
using System.Timers;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// Just move pdf files from folder in path1 to folder in path2
        /// @author rundeploy
        /// @date 4/12/2018
        /// </summary>
        static void Main()
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 30000;
            aTimer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit the sample."); // Mod this in order to run always
            while (Console.Read() != 'q') ;

        }

        private static void OnTimedEvent(object source, ElapsedEventArgs a)
        {
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

                    // Ensure that the target does not exist.
                    if (File.Exists(Path.Combine(path2, name)))
                    {
                        File.Delete(Path.Combine(path2, name));
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
