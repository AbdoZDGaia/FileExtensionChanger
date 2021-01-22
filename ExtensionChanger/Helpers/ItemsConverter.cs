using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionChanger.Helpers
{
    public static class ItemsConverter
    {
        public static DirectoryItem ConvertToDirectoryItem(string path)
        {
            string name;
            string extension = "";
            var attributes = File.GetAttributes(path);
            var lastDot = path.LastIndexOf(".");
            var lastSlash = path.LastIndexOf("\\");
            if (lastDot - lastSlash - 1 > 1)
                name = path.Substring(lastSlash + 1, Math.Abs(lastDot - lastSlash - 1));
            else
                name = path.Substring(lastSlash + 1, path.Length - 1 - lastSlash);
            var isFile = !(attributes.HasFlag(FileAttributes.Directory));
            if (isFile)
                extension = path.Substring(lastDot, path.Length - lastDot);
            var result = new DirectoryItem()
            {
                Id = Guid.NewGuid(),
                ItemPath = path,
                isFile = isFile,
                ItemName = name,
                ItemExtension = isFile ? extension : "",
            };
            return result;
        }
    }
}
