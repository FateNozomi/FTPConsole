using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTPConsole
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        private static void DoWork()
        {
            Console.WriteLine("Please enter the XML file path to upload:");
            string localPath = Console.ReadLine();
            UploadXML(localPath);
        }

        private static void UploadXML(string path)
        {
            // Example File Name: AA.1.123456789.01.xml
            //                    [StripMarking].[StripNumber].[AmkorId].[DCC].xml

            IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            FTPManager ftpManager = new FTPManager(ipAddress, 21, "admin", "admin");

            string fileName = Path.GetFileName(path);
            List<string> stripMarkingFolders = GetStripMarkingFolders(fileName);

            Console.WriteLine("Creating Parent Folder...");
            string parentFolder = string.Empty;
            foreach (string folder in stripMarkingFolders)
            {
                parentFolder += "/" + folder;

                if (!ftpManager.DirectoryExists(parentFolder))
                {
                    ftpManager.CreateDirectory(parentFolder);
                }
                else
                {
                    Console.WriteLine("{0} folder already exist! Skipping folder creation...", parentFolder);
                }
            }

            string remotePath = parentFolder + "/" + fileName;

            Console.WriteLine("Uploading XML file to {0}", remotePath);
            ftpManager.Upload(path, remotePath);
        }

        private static List<string> GetStripMarkingFolders(string fileName)
        {
            int startIndex = 0;
            int lastIndex = fileName.IndexOf(".");
            int length = lastIndex - startIndex;

            string stripMarking = fileName.Substring(startIndex, length);

            List<string> folders = new List<string>();

            foreach (char folderName in stripMarking)
            {

                folders.Add(folderName.ToString());
            }

            return folders;
        }
    }
}
