using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Libraries
{
    public static class FileUtils
    {
        private static Dictionary<string, bool> FileReadStatus = new Dictionary<string, bool>();
        private static Dictionary<string, bool> FileWriteStatus = new Dictionary<string, bool>();

        internal static async Task<string> SafelyReadAllText(string DirToFile)
        {
            if (FileReadStatus.ContainsKey(DirToFile) && FileReadStatus[DirToFile])
            {
                while (FileReadStatus[DirToFile])
                {
                    //Wait
                    await Task.Delay(100);
                }
            }

            FileReadStatus[DirToFile] = true;

            var Result = File.ReadAllText(DirToFile);

            FileReadStatus[DirToFile] = false;

            return Result;
        }

        public static async Task SafelyWriteAllText(string path, string text)
        {
            if (FileWriteStatus.ContainsKey(path) && FileWriteStatus[path])
            {
                while (FileWriteStatus[path])
                {
                    //Wait
                    await Task.Delay(100);
                }
            }

            FileWriteStatus[path] = true;

            File.WriteAllText(path, text);

            FileWriteStatus[path] = false;
        }
    }
}
