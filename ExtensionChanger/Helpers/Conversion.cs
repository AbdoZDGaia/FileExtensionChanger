using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionChanger.Helpers
{
    public class Conversion
    {
        private readonly string _path;
        private readonly string _appName;
        private bool _validSelection;
        private int _selection;
        private List<DirectoryItem> _myList;
        Common common;

        public Conversion(string path, string appName)
        {
            int selection = 0;
            bool validSelection = false;
            _path = path;
            _appName = appName;
            _validSelection = validSelection;
            _selection = selection;
            _myList = new List<DirectoryItem>();
            common = new Common(_path, _myList, _validSelection, _selection, _appName);
        }

        private void ReInitialize(Dictionary<string, object> newValuesDictionary)
        {
            _myList = (List<DirectoryItem>)newValuesDictionary["myList"];
            _selection = (int)newValuesDictionary["selection"];
            _validSelection = (bool)newValuesDictionary["validSelection"];
        }

        internal void ConvertToExe()
        {
            while (!_validSelection)
                ReInitialize(common.IterateThroughPath());
            ConvertItems(_selection, GeneralEnums.Transformations.exe);
        }

        internal void ConvertToMp4()
        {
            while (!_validSelection)
                ReInitialize(common.IterateThroughPath());
            ConvertItems(_selection, GeneralEnums.Transformations.mp4);
        }

        internal void ConvertItems(int ItemNumber, GeneralEnums.Transformations convertType)
        {
            if (ItemNumber == 0)
                return;

            //All files and subdirectories' files are to be renamed
            if (ItemNumber - _myList.Count == 1)
            {
                int fileItemNumber = 1;
                foreach (var fileItem in _myList)
                {
                    ConvertItem(fileItemNumber, convertType);
                    fileItemNumber++;
                }
            }

            //A single file or directory selected
            if (ItemNumber <= _myList.Count)
            {
                ConvertItem(ItemNumber, convertType);
            }

            //Invalid selection
            if (ItemNumber - _myList.Count > 1)
            {
                return;
            }

        }

        internal void ConvertItem(int ItemNumber, GeneralEnums.Transformations convertType)
        {
            Console.WriteLine($"Converting to {convertType}\n");

            var item = _myList[ItemNumber - 1];
            var path = item.ItemPath;

            if (Directory.Exists(path) && !item.isFile)
            {
                ConvertDirectory(convertType, path);
            }

            if (File.Exists(path) && item.isFile)
            {
                ConvertFile(convertType, item);
            }
        }

        internal void ConvertDirectory(GeneralEnums.Transformations convertType, string path)
        {
            var directoryFiles = Directory.GetFiles(path);
            if (directoryFiles.Count() == 0)
                Console.WriteLine("No files or sub-directories were present...Nothing's changed");
            foreach (var file in directoryFiles)
            {
                var itemFile = ItemsConverter.ConvertToDirectoryItem(file);
                ConvertFile(convertType, itemFile);
            }
        }

        internal void ConvertFile(GeneralEnums.Transformations convertType, DirectoryItem fileItem)
        {
            var oldName = fileItem.ItemName + fileItem.ItemExtension;
            var newName = fileItem.ItemName + "." + convertType;
            if (fileItem.ItemPath != _path + "\\" + _appName && !HasExtension(fileItem.ItemPath, convertType))
            {
                Console.WriteLine($"Old File Name: {fileItem.ItemName}{fileItem.ItemExtension}\nNew File Name: {fileItem.ItemName}.{convertType}");
                File.Move(fileItem.ItemPath, fileItem.ItemPath.Replace(oldName, newName));
            }
        }

        internal bool HasExtension(string file, GeneralEnums.Transformations conv)
        {
            return file.Contains("." + conv.ToString());
        }
    }
}
