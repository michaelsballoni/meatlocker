using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace MeetLocker
{
    public delegate Stream FileRetrieveHandler(string filename);

    public static class Mgr
    {
        /// <summary>
        /// Give a file and a password, generate a key, and stuff the file bytes into password-protected ZIP file
        /// </summary>
        /// <returns>The key</returns>
        public static async Task<string> Store(string pwd, string filename, Stream file)
        {
            byte[] uniq_part = Guid.NewGuid().ToByteArray();
            byte[] rand_part = RandomNumberGenerator.GetBytes(uniq_part.Length / 4); // don't get too long!
            string key = Convert.ToHexString(uniq_part) + Convert.ToHexString(rand_part);

            string zip_file_path = GetFilePath(key, pwd);
            using (Stream zip_file = File.Create(zip_file_path))
            {
                using (ZipOutputStream zip_file_stream = new ZipOutputStream(zip_file))
                {
                    var entry = new ZipEntry(filename);
                    zip_file_stream.PutNextEntry(entry);
                    zip_file_stream.Password = pwd;
                    await file.CopyToAsync(zip_file_stream);
                }
            }
            return key;
        }

        /// <summary>
        /// Given key and password, call a handler with the filename (for setting HTTP Content-Disposition).
        /// The handler returns the Stream to write the file contents to.
        /// </summary>
        public static async Task Retrieve(string key, string pwd, FileRetrieveHandler handler)
        {
            string file_path = GetFilePath(key, pwd);
            using (Stream file_stream = File.OpenRead(file_path))
            {
                using (ZipFile zip_file = new ZipFile(file_stream))
                {
                    zip_file.Password = pwd;

                    string filename = "";
                    foreach (ZipEntry entry in zip_file)
                    {
                        filename = entry.Name;
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(filename))
                        throw new Exception("Filename not found");

                    using (var zip_stream = zip_file.GetInputStream(0))
                    {
                        var output_stream = handler(filename);
                        await zip_stream.CopyToAsync(output_stream);
                        output_stream.Dispose();
                    }
                }
            }
            File.Delete(file_path);
        }

        /// <summary>
        /// Given key and password return where the file might be found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GetFilePath(string key, string pwd)
        {
            string filename = Convert.ToHexString(Encoding.Unicode.GetBytes(key + pwd)) + ".dat";
            string file_path = Path.Combine(FilesDirPath, filename);
            return file_path;
        }

        /// <summary>
        /// Get the path to the dircectory that holds all the files
        /// </summary>
        public static string FilesDirPath
        {
            get
            {
                if (sm_strFilesDirPath != null) // cheap check
                    return sm_strFilesDirPath;

                lock (sm_filesDirPathLock) // rare path to head down
                {
                    if (sm_strFilesDirPath != null) // cheap check again
                        return sm_strFilesDirPath;

                    string files_dir_path =
                        Path.Combine
                        (
                            Path.GetDirectoryName
                            (
                                // not something you want to do all the time, hence all this code
                                Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName
                                ??
                                ""
                            )
                            ??
                            "",
                            "files"
                        );
                    if (!Directory.Exists(files_dir_path))
                        Directory.CreateDirectory(files_dir_path);

                    sm_strFilesDirPath = files_dir_path;
                    return sm_strFilesDirPath;
                }
            }
        }
        private static string? sm_strFilesDirPath = null;
        private static object sm_filesDirPathLock = new object();
    }
}
