using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fotos
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string dirPath = ConfigurationManager.AppSettings["rootPath"];
                string newPath = ConfigurationManager.AppSettings["fotosPath"];
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                var filesSkipped = moveFiles(dirPath);

                if (filesSkipped?.Count() > 0)
                {
                    Console.WriteLine("Estos archivos ya existían, no se movieron");
                    filesSkipped.ForEach(f => { Console.WriteLine(f); });
                }

                Console.WriteLine("... complete");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
#if DEBUG
                throw new Exception();
#endif
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        static List<string> moveFiles(string dirPath)
        {
            DirectoryInfo d = new DirectoryInfo(dirPath);
            List<string> filesSkipped = new List<string>();

            FileInfo[] files = d.GetFiles();
            foreach (var file in files)
            {
                string new_path = checkFolder(file.LastWriteTime);
                if (!File.Exists(new_path + "\\" + file.Name))
                {
                    file.MoveTo(new_path + "\\" + file.Name);
                }
                else
                {
                    filesSkipped.Add(file.Name);
                }
            }

            DirectoryInfo[] directories = d.GetDirectories();
            foreach (var directory in directories)
            {
                filesSkipped.AddRange(moveFiles(directory.FullName));
            }

            return filesSkipped;
        }

        static string checkFolder(DateTime creation_date)
        {
            string newPath = ConfigurationManager.AppSettings["fotosPath"];

            newPath += $"{creation_date.Year}\\{ creation_date.Month}-{creation_date.ToString("MMMM")}\\";

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            return newPath;
        }
    }
}
