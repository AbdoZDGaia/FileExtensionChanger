using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionChanger.Helpers
{
    public class Common
    {
        private readonly string _path;
        private List<DirectoryItem> _myList;
        private bool _validSelection;
        private int _selection;
        private readonly string _appName;

        public Common(string path, List<DirectoryItem> myList, bool validSelection, int selection, string appName)
        {
            _path = path;
            _myList = myList;
            _validSelection = validSelection;
            _selection = selection;
            _appName = appName;
        }

        public Dictionary<string, object> IterateThroughPath()
        {
            int counter = 0;
            if (Directory.Exists(_path))
            {
                PopulateItemsList();

                _myList.ForEach(item =>
                {
                    counter++;
                    OutputItemsToConsole(item, counter);
                });

                AddTheAllOption();
            }

            var prompt = Console.ReadLine();

            int.TryParse(prompt, out int promptAsNumber);
            if (prompt.Trim().ToUpper() == "EXIT")
            {
                Environment.Exit(0);
            }
            else if (promptAsNumber == 0)
            {
                Console.WriteLine("Invalid selection\nTry again, or type \"Exit\" to exit\n\n");
                IterateThroughPath();
            }
            else
            {
                _validSelection = true;
                _selection = promptAsNumber;
            }

            return new Dictionary<string, object> {
                { "validSelection", _validSelection },
                { "selection", _selection },
                { "myList", _myList }
            };
        }

        public void AddTheAllOption()
        {
            if (_myList.Count > 0)
                Console.WriteLine($"{_myList.Count + 1}) All the files and sub-directories");
        }

        public void OutputItemsToConsole(DirectoryItem item, int counter)
        {
            Console.WriteLine(item.isFile ?
                                   $"{counter}) FileName:{item.ItemName}, FileExtension:{item.ItemExtension}" :
                                   $"{counter}) **Directory Name:{item.ItemName}");
        }

        public void PopulateItemsList()
        {
            var directoryFiles = Directory.GetFileSystemEntries(_path);
            if (directoryFiles.Count() == 1)
                Console.WriteLine("No files or sub-directories are present...");
            foreach (var file in directoryFiles)
            {
                if (file == _path + "\\" + _appName)
                    continue;
                var item = ItemsConverter.ConvertToDirectoryItem(file);
                _myList.Add(item);
            }

            if (_myList.Count > 0)
            {
                _myList = _myList.OrderBy(il => il.isFile).ToList();
            }
        }
    }
}
