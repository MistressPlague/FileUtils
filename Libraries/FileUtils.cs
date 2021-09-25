using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Libraries
{
    public static class FileUtils
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;

        private static bool IsFileLocked(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }

        private static bool CanReadFile(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
            catch (IOException ex)
            {
                if (IsFileLocked(ex))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsReading = false;
        internal static async Task<string> SafelyReadAllText(string DirToFile)
        {
            if (!CanReadFile(DirToFile) || IsReading)
            {
                while (!CanReadFile(DirToFile) || IsReading)
                {
                    //Wait
                    await Task.Delay(150);
                }
            }

            IsReading = true;
            
            var Result = File.ReadAllText(DirToFile);
            
            IsReading = false;

            return Result;
        }

        private static bool IsWriting = false;
        public static async Task SafelyWriteAllText(string path, string text)
        {
            if (!CanReadFile(path) || IsWriting)
            {
                while (!CanReadFile(path) || IsWriting)
                {
                    //Wait
                    await Task.Delay(150);
                }
            }

            IsWriting = true;
            
            File.WriteAllText(path, text);
            
            IsWriting = false;
        }
    }
}
