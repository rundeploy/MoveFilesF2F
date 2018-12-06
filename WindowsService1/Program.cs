﻿using System;
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
        

        //[TODO] ask user the FROM folder and the TO folder files to move
        private static string path1 = @"D:\Downloads\Pasta1"; // Specify folder from you want to move files
        private static string path2 = @"D:\Downloads\Pasta2"; // Specify folder where you want the files be moved to

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
            

            var pdfFileNames = GetFileNames(path1, "*.pdf");
            

            try
            {
                foreach (string name in pdfFileNames)
                {
                    if (!Directory.Exists(path2))
                    {
                        Directory.CreateDirectory(path2);
                    }
                    //[TODO] check if there are arleady files with the same name and same format then to do something about it before moving
                    if (File.Exists(Path.Combine(path2, name)))
                    {
                        if (alterDupFileName(path2, name))
                        {
                            Console.WriteLine("{0} was moved to {1}.", path1, path2);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Something went wrong altering the already existing file with the same name in destination folder");
                            return;
                        }
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
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static bool alterDupFileName(string path2, string name)
        {
            
            bool alterNameSuccessed = false;
            string alteredName = "";
            int valueIsNumeric = 0;

            string[] splitName = name.Split('.', '(', ')');
            //[nome] [ ] [pdf]
            //[0]    [1] [2] length = 3
            //se antes do ponto onde se define o tipo de ficheiro estiver entre parentesis um valor do tipo int entao incrementar esse valor
            //int.TryParse(splitName[splitName.Length - 3], out int result);
            if (splitName.Length >= 3)
            {
                if (int.TryParse(splitName[splitName.Length - 3], out int result) && IsNumericType(result))
                {
                    //Obter o que esta na posicao array.length-3 do nome do fich xpto(0).pdf -> [xpto] [0] [""] [pdf]
                    var valInParentesis = splitName[splitName.Length - 3];
                    valueIsNumeric = Int32.Parse(valInParentesis); //converter para inteiro esse valor encontrado na posicao array.length-3


                }
            }
            
            valueIsNumeric++;
            alteredName = splitName[0]; // alteredName = [xpto]

            // se houver mais alguma coisa entre [xpto] e [0] entao concatena no alteredName
            if (splitName.Length - 4 > 1) 
            {
                for (int i = 1; i < splitName.Length - 3; i++)
                {
                    alteredName = alteredName + splitName[i] ;
                }
            }

            alteredName = alteredName + "(" + valueIsNumeric + ")" + ".pdf";
            


            if (File.Exists(Path.Combine(path2, alteredName)))
            {
                alterDupFileName(path2, alteredName );
            }
            else
            {
                File.Move(Path.Combine(path1, name), Path.Combine(path2, alteredName));
                alterNameSuccessed = true;
            }
            return alterNameSuccessed;
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
