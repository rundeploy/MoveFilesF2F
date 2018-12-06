using System;
using System.IO;
using System.Timers;

namespace WindowsService1
{
    static class Program
    {

        private const string EDP = "edp";
        private const string BCP = "bcp";

        //[TODO] ask user the FROM folder and the TO folder files to move
        private static string path1 = @"D:\Downloads\Pasta1"; // Specify folder from you want to move files
        private static string path2 = @"D:\Downloads\Pasta2"; // Specify folder where you want the files be moved to
        private static string path3 = @"D:\Downloads\Pasta3"; // Specify folder where you want the files be moved to

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


                    //if filename contains edp
                    //move to folder 2
                    //else if filename contains bcp
                    // move to folder3

                    if(IsSubstring(name, EDP)){
                        if (File.Exists(Path.Combine(path2, name)))
                        {
                            string newName = GetNotDuplicatedFileName(path2, name); //Alter the duplicated name then move it
                            File.Move(Path.Combine(path1, name), Path.Combine(path2, newName));
                            Console.WriteLine("{0} was moved to {1}.", path1, path2);
                        }
                        else
                        {
                            File.Move(Path.Combine(path1, name), Path.Combine(path2, name));
                            Console.WriteLine("{0} was moved to {1}.", path1, path2);
                        }
                    }else if (IsSubstring(name, BCP))
                    {
                        if (File.Exists(Path.Combine(path3, name)))
                        {
                            string newName = GetNotDuplicatedFileName(path3, name); //Alter the duplicated name then move it
                            File.Move(Path.Combine(path1, name), Path.Combine(path3, newName));
                            Console.WriteLine("{0} was moved to {1}.", path1, path3);
                        }
                        else
                        {
                            File.Move(Path.Combine(path1, name), Path.Combine(path3, name));
                            Console.WriteLine("{0} was moved to {1}.", path1, path3);
                        }

                    }else
                    {
                        Console.WriteLine("Nothing to move");
                    }

                    
                    
                    


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
        /// Get not duplicated filename
        /// </summary>
        /// <param name="destPath">destination folder</param>
        /// <param name="name">file name</param>
        /// <returns></returns>
        private static string GetNotDuplicatedFileName(string destPath, string name)
        {
            //bool alterNameSuccessed = false;

            string alteredName = actuallyAlterDupName(name);

            while(File.Exists(Path.Combine(destPath, alteredName)))
            {
                GetNotDuplicatedFileName(destPath, alteredName );
            }
            
            return alteredName;
        }

        /// <summary>
        /// Alter the names of files if they are the same on destination folder as in the origin
        /// </summary>
        /// <param name="name">file name to be changed if duplicated on the destination folder</param>
        /// <returns></returns>
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
