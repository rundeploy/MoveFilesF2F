using System;
using System.IO;
using System.Timers;

namespace WindowsService1
{
    static class Program
    {

        private const string EDP = "edp";
        private const string BCP = "bcp";

        private static string path1 = @"D:\Downloads\Pasta1"; // Specify folder from you want to move files
        private static string path2 = @"D:\Downloads\Pasta2"; // Specify folder where you want the files be moved to
        private static string path3 = @"D:\Downloads\Pasta3"; // Specify folder where you want the files be moved to
        //private static string path4 = @"D:\Downloads\Pasta4"; // Specify folder where you want the files be moved to

        /// <summary>
        /// Just move pdf files from folder in path1 to destination folder
        /// @author rundeploy
        /// @date 4/12/2018
        /// </summary>
        static void Main()
        {

            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000; // 5000 = From 5 seconds to 5 seconds 
            aTimer.Enabled = true;
            

            Console.WriteLine("Press \'q\' to quit the sample."); 
            while (Console.Read() != 'q') ; // Mod this in order to run forever
        }

        /// <summary>
        /// While there are pdf files in the path1 folder check if on destination exist same name file
        /// if exist then modify name until there does not exist and then move, 
        /// otherwise just move the file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="a"></param>
        private static void OnTimedEvent(object source, ElapsedEventArgs a)
        {
            //Guarantee that folders do exist
            if (!Directory.Exists(path1))
                Directory.CreateDirectory(path1);

            var pdfFileNames = GetFileNames(path1, "*.pdf"); //Seleciono apenas os ficheiros PDF numa lista (array)
            
            try
            {
                foreach (string name in pdfFileNames)
                {
                    //Guarantee that folders do exist
                    if (!Directory.Exists(path2))
                        Directory.CreateDirectory(path2);
                    if (!Directory.Exists(path3))
                        Directory.CreateDirectory(path3);
                    //if (!Directory.Exists(path4))
                    //    Directory.CreateDirectory(path4);

                    if (IsSubstring(name, EDP)) {
                        if (File.Exists(Path.Combine(path2, name)))
                        {
                            string newName = GetNotDuplicatedFileName(path2, name); //file with same name exist so lets Alter the duplicated name 
                            File.Move(Path.Combine(path1, name), Path.Combine(path2, newName));
                            Console.WriteLine("{0} was moved to {1}.", path1, path2);
                        }
                        else
                        {
                            File.Move(Path.Combine(path1, name), Path.Combine(path2, name));
                            Console.WriteLine("{0} was moved to {1}.", path1, path2);
                        }
                    } else if (IsSubstring(name, BCP))
                    {
                        if (File.Exists(Path.Combine(path3, name)))
                        {
                            string newName = GetNotDuplicatedFileName(path3, name); //file with same name exist so lets Alter the duplicated name 
                            File.Move(Path.Combine(path1, name), Path.Combine(path3, newName));
                            Console.WriteLine("{0} was moved to {1}.", path1, path3);
                        }
                        else
                        {
                            File.Move(Path.Combine(path1, name), Path.Combine(path3, name));
                            Console.WriteLine("{0} was moved to {1}.", path1, path3);
                        }
                    //}else if (IsSubstring(name, QWERT))
                    //{
                    //    if (File.Exists(Path.Combine(path4, name)))
                    //    {
                    //        string newName = GetNotDuplicatedFileName(path4, name); //file with same name exist so lets Alter the duplicated name 
                    //        File.Move(Path.Combine(path1, name), Path.Combine(path4, newName));
                    //        Console.WriteLine("{0} was moved to {1}.", path1, path3);
                    //    }
                    //    else
                    //    {
                    //        File.Move(Path.Combine(path1, name), Path.Combine(path4, name));
                    //        Console.WriteLine("{0} was moved to {1}.", path1, path4);
                    //    }

                    }else
                    {
                        Console.WriteLine("No files to move");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process of moving files failed: {0}", e.ToString());
            }
        }
       

        /// <summary>
        /// Get different name than one that exist on destination folder in order to not to overwrite and loose it
        /// </summary>
        /// <param name="destPath">destination folder</param>
        /// <param name="name">file name</param>
        /// <returns>Returns a new name guaranteeing that it doesnt exist in destination folder</returns>
        private static string GetNotDuplicatedFileName(string destPath, string name)
        {
            string alteredName = actuallyAlterDupName(name);

            if (File.Exists(Path.Combine(destPath, alteredName)))
            {
                return GetNotDuplicatedFileName(destPath, alteredName );
            }
            
            return alteredName;
        }

        /// <summary>
        /// Alter the names of files if they are the same on destination folder as in the origin and returns a string of the altered name
        /// </summary>
        /// <param name="name">file name to be changed if duplicated on the destination folder</param>
        /// <returns>Get the new name of the file</returns>
        private static string actuallyAlterDupName(String name)
        {
            string alteredName = "";
            int valueIsNumeric = 0;

            string[] splitName = name.Split('.', '(', ')');
            //[nome] [ ] [pdf]
            //[0]    [1] [2] length = 3

            // Garantir que no array esta xpto(1).pdf -> [xpto][1][""][pdf] -> 4
            // Se xpto.pdf -> [xpto][pdf] -> 2 então não entra
            if (splitName.Length >= 3)
            {
                // Garantir que [xpto][1][""][pdf] -> [1] é um int
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
                    alteredName = alteredName + splitName[i];
                }
            }

            alteredName = alteredName + "(" + valueIsNumeric + ")" + ".pdf";

            return alteredName;
        }

        /// <summary>
        /// Check file names from the folder indicated in the path and return an array of file names
        /// </summary>
        /// <param name="path">The path to the origin folder where files are and should be moved</param>
        /// <param name="filtro">files format</param>
        /// <returns>Returns a list of file names (and their format) in a form of an array</returns>
        private static string[] GetFileNames(string path, string filtro)
        {
            var pdfFiles = new DirectoryInfo(path).GetFiles(filtro);
            string[] fileNames = new string[pdfFiles.Length];
            for (int i = 0; i < pdfFiles.Length; i++)
                fileNames[i] = pdfFiles[i].Name;

            return fileNames;
        }


        /// <summary>
        /// Check if a parameter is a number
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Return true if the parameter o is a number, return false otherwise</returns>
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

        /// <summary>
        /// Verify if in the name exist a substring substr
        /// </summary>
        /// <param name="name">String to verify if substr is in</param>
        /// <param name="substr">Substring to verify if it is in name string</param>
        /// <returns>Return true if substring substr is a part of the string name</returns>
        private static bool IsSubstring(string name, string substr)
        {
            if (name.ToLower().Contains(substr))
            {
                return true;
            }

            return false;
        }

    }
}
