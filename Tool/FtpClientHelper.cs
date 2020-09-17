using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// FTP客户端帮助类
    /// </summary>
    public static class FtpClientHelper
    {
        /// <summary>
        /// 上传文件,返回 StatusDescription
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fileName"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static string UploadFile(Uri target, string fileName, NetworkCredential credentials = null)
        {
            var uri = target.AbsoluteUri;
            var index = uri.LastIndexOf('/');
            var makeDir = MakeDirectoryRecursion(new Uri(uri.Substring(0, index + 1)), credentials);

            // Create a Uri instance with the specified URI string.
            // If the URI is not correctly formed, the Uri constructor
            // will throw an exception.
            ManualResetEvent waitObject;

            FtpState state = new FtpState();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(target);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example uses anonymous logon.
            // The request is anonymous by default; the credential does not have to be specified. 
            // The example specifies the credential only to
            // control how actions are logged on the server.
            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            // Store the request in the object that we pass into the
            // asynchronous operations.
            state.Request = request;
            state.FileName = fileName;

            // Get the event to wait on.
            waitObject = state.OperationComplete;

            // Asynchronously get the stream for the file contents.
            request.BeginGetRequestStream(
                new AsyncCallback(EndGetStreamCallback),
                state
            );

            // Block the current thread until all operations are complete.
            waitObject.WaitOne();

            // The operations either completed or threw an exception.
            if (state.OperationException != null)
            {
                throw state.OperationException;
            }

            return string.Format("{0}, UploadFile {2} completed - {1}", makeDir, state.StatusDescription, target.AbsoluteUri);
        }
        #region+    private method
        private static void EndGetStreamCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;

            Stream requestStream = null;
            // End the asynchronous call to get the request stream.
            try
            {
                requestStream = state.Request.EndGetRequestStream(ar);
                // Copy the file contents to the request stream.
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                using (FileStream stream = File.OpenRead(state.FileName))
                {
                    do
                    {
                        readBytes = stream.Read(buffer, 0, bufferLength);
                        requestStream.Write(buffer, 0, readBytes);
                        count += readBytes;
                    }
                    while (readBytes != 0);
                }
                //Console.WriteLine("Writing {0} bytes to the stream.", count);
                // IMPORTANT: Close the request stream before sending the request.
                requestStream.Close();
                // Asynchronously get the response to the upload request.
                state.Request.BeginGetResponse(
                    new AsyncCallback(EndGetResponseCallback),
                    state
                );
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {
                //Console.WriteLine("Could not get the request stream.");
                state.OperationException = e;
                state.OperationComplete.Set();
                return;
            }

        }

        // The EndGetResponseCallback method  
        // completes a call to BeginGetResponse.
        private static void EndGetResponseCallback(IAsyncResult ar)
        {
            FtpState state = (FtpState)ar.AsyncState;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)state.Request.EndGetResponse(ar);
                response.Close();
                state.StatusDescription = response.StatusDescription;
                // Signal the main application thread that 
                // the operation is complete.
                state.OperationComplete.Set();
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {
                //Console.WriteLine("Error getting response.");
                state.OperationException = e;
                state.OperationComplete.Set();
                return;
            }
        }
        #endregion

        /// <summary>
        /// 下载文件,返回文件流
        /// </summary>
        /// <param name="serverUri"></param>
        /// <returns></returns>
        public static byte[] DownLoadFile(Uri serverUri)
        {
            // Get the object used to communicate with the server.
            WebClient request = new WebClient();

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", "abc@xyz.com");
            byte[] newFileData = request.DownloadData(serverUri.ToString());

            return newFileData;
        }

        /// <summary>
        /// 删除文件，返回 StatusDescription
        /// </summary>
        /// <param name="fileUri"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static string DeleteFile(Uri fileUri, NetworkCredential credentials = null)
        {
            var exsitFile = IsExistFile(fileUri, credentials);
            if (!exsitFile)
            {
                return string.Format("{0} file not exist!", fileUri.AbsoluteUri);
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileUri);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            var result = string.Format("DeleteFile {0} - {1}", fileUri.AbsoluteUri, response.StatusDescription);
            response.Close();

            return result;
        }

        /// <summary>
        /// 删除文件夹，返回 StatusDescription
        /// </summary>
        /// <param name="directoryUri"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static string RemoveDirectoryRecursion(Uri directoryUri, NetworkCredential credentials = null)
        {
            var result = string.Empty;
            var listDirectory = ListDirectoryDetails(directoryUri);
            for (int i = 0; i < listDirectory.Count; i++)
            {
                if (listDirectory[i].IsDirectory)
                {
                    var subDirectoryUri = new Uri(string.Format("{0}{1}/", directoryUri.AbsoluteUri, listDirectory[i].Name));
                    var msg = RemoveDirectoryRecursion(subDirectoryUri, credentials);
                    result = string.Format("{0}{1},", result, msg);
                }
                else
                {
                    var subFileUri = new Uri(string.Format("{0}{1}", directoryUri.AbsoluteUri, listDirectory[i].Name));
                    var msg = DeleteFile(subFileUri, credentials);
                    result = string.Format("{0}{1},", result, msg);
                }
            }

            var msgRemoveDirectory = RemoveDirectory(directoryUri, credentials);
            result = string.Format("{0}{1},", result, msgRemoveDirectory);

            return result;
        }

        public static string RemoveDirectory(Uri directoryUri, NetworkCredential credentials = null)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUri);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            var result = string.Format("RemoveDirectory {0} - {1}", directoryUri.AbsoluteUri, response.StatusDescription);
            response.Close();

            return result;
        }

        private static string MakeDirectoryRecursion(Uri directoryUri, NetworkCredential credentials = null)
        {
            List<Uri> uriList = new List<Uri> { directoryUri };
            while (true)
            {
                var last = uriList.Last();
                if (string.IsNullOrEmpty(last.AbsolutePath) || last.AbsolutePath.Trim('/') == "")
                {
                    uriList.Remove(last);
                    break;
                }

                var uri = last.AbsoluteUri.TrimEnd('/');
                var index = uri.LastIndexOf('/');
                last = new Uri(uri.Substring(0, index + 1));
                uriList.Add(last);
            }

            var result = string.Empty;
            uriList.Reverse();
            uriList.ForEach(uri =>
            {
                var msg = MakeDirectory(uri, credentials);
                result = string.Format("{0}{1}{2}", result, msg, string.IsNullOrEmpty(msg) ? "" : "\n");
            });

            return result;
        }

        private static string MakeDirectory(Uri directoryUri, NetworkCredential credentials = null)
        {
            bool existDir = IsExistDirectory(directoryUri, credentials);
            if (existDir)
            {
                return null;
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUri);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            var result = string.Format("MakeDirectory {1} {0}", response.StatusDescription, directoryUri.AbsoluteUri);
            response.Close();

            return result;
        }

        private static bool IsExistDirectory(Uri directoryUri, NetworkCredential credentials = null)
        {
            var uri = directoryUri.AbsoluteUri.TrimEnd('/');
            var index = uri.LastIndexOf('/');
            var parentUri = new Uri(uri.Substring(0, index + 1));
            var theDirName = uri.Substring(index + 1);
            var ls = ListDirectoryDetails(parentUri, credentials);
            return ls.Where(x => x.IsDirectory && x.Name == theDirName).Any();
        }

        private static bool IsExistFile(Uri fileUri, NetworkCredential credentials = null)
        {
            var uri = fileUri.AbsoluteUri;
            var index = uri.LastIndexOf('/');
            var dirUri = new Uri(uri.Substring(0, index + 1));
            var theFileName = uri.Substring(index + 1);
            var ls = ListDirectoryDetails(dirUri, credentials);
            return ls.Where(x => !x.IsDirectory && x.Name == theFileName).Any();
        }

        private static List<FileStruct> ListDirectoryDetails(Uri directoryUri, NetworkCredential credentials = null)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUri);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            if (credentials != null)
            {
                request.Credentials = credentials;
            }

            var fileList = new List<FileStruct>();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] arrs = line.Split(' ');
                    var model = new FileStruct()
                    {
                        IsDirectory = line.IndexOf("<DIR>") > 0 || arrs[0].IndexOf("d") == 0 ? true : false,
                        Name = arrs[arrs.Length - 1],
                    };
                    fileList.Add(model);
                }
            }

            var result = string.Format("ListDirectoryDetails '{1}' status: {0}", response.StatusDescription, directoryUri.AbsoluteUri);
            response.Close();

            return fileList;
        }

        /// <summary>
        /// 获取目录下的文件名列表
        /// </summary>
        /// <param name="directoryUri"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static List<string> ListDirFileNames(Uri directoryUri, NetworkCredential credentials = null)
        {
            var ls = ListDirectoryDetails(directoryUri, credentials);
            return ls.Where(x => !x.IsDirectory).Select(x => x.Name).ToList();
        }

        private class FileStruct
        {
            /// <summary>  
            /// 是否为目录  
            /// </summary>  
            public bool IsDirectory { get; set; }
            /// <summary>  
            /// 文件或目录名称  
            /// </summary>  
            public string Name { get; set; }
        }
        private class FtpState
        {
            private ManualResetEvent wait;
            private FtpWebRequest request;
            private string fileName;
            private Exception operationException = null;
            string status;

            public FtpState()
            {
                wait = new ManualResetEvent(false);
            }

            public ManualResetEvent OperationComplete
            {
                get { return wait; }
            }

            public FtpWebRequest Request
            {
                get { return request; }
                set { request = value; }
            }

            public string FileName
            {
                get { return fileName; }
                set { fileName = value; }
            }
            public Exception OperationException
            {
                get { return operationException; }
                set { operationException = value; }
            }
            public string StatusDescription
            {
                get { return status; }
                set { status = value; }
            }
        }
    }
}
