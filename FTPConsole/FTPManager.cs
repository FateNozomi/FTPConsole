using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTPConsole
{
    public class FTPManager
    {
        private readonly IPAddress _ipAddress;
        private readonly ushort _port;
        private readonly string _userName;
        private readonly string _password;

        public FTPManager(IPAddress ipAddress, ushort port, string userName, string password)
        {
            _ipAddress = ipAddress;
            _port = port;
            _userName = userName;
            _password = password;

            URI = new Uri("ftp://" + _ipAddress + ":" + _port);
        }

        public Uri URI { get; private set; }

        public void Download(string remotePath, string localPath)
        {
            FtpWebRequest request = CreateRequest(remotePath, WebRequestMethods.Ftp.DownloadFile);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream localFileStream = new FileStream(localPath, FileMode.CreateNew))
                    {
                        responseStream.CopyTo(localFileStream);
                    }
                }
            }
        }

        public void Upload(string localPath, string remotePath)
        {
            FtpWebRequest request = CreateRequest(remotePath, WebRequestMethods.Ftp.UploadFile);

            StreamReader sourceStream = new StreamReader(localPath);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
            }
        }

        public string ListDirectory(string path)
        {
            FtpWebRequest request = CreateRequest(path, WebRequestMethods.Ftp.ListDirectory);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public string ListDirectoryDetails(string path)
        {
            FtpWebRequest request = CreateRequest(path, WebRequestMethods.Ftp.ListDirectoryDetails);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public bool DirectoryExists(string path)
        {
            try
            {
                FtpWebRequest request = CreateRequest(path, WebRequestMethods.Ftp.ListDirectory);
                return request.GetResponse() != null;
            }
            catch (WebException)
            {
                return false;
            }
        }

        public void CreateDirectory(string path)
        {
            FtpWebRequest request = CreateRequest(path, WebRequestMethods.Ftp.MakeDirectory);

            using (FtpWebResponse responseStream = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine(responseStream.StatusCode);
            }
        }

        private FtpWebRequest CreateRequest(string remotePath, string ftpWebRequestMethod)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(
                new Uri(URI, remotePath));
            request.Credentials = new NetworkCredential(_userName, _password);
            request.Method = ftpWebRequestMethod;

            return request;
        }
    }
}
