using ExtensionChanger.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ExtensionChanger
{
    public static class Program
    {
        public static List<DirectoryItem> MyList;
        public static int selection;
        public static bool validSelection;
        public static readonly string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        public static readonly string appName = Path.GetFileName(codeBase);
        public static string path = Directory.GetCurrentDirectory();


        static void Main(string[] args)
        {
            bool Exit = false;
            while (!Exit)
            {
                InitiateItemsList();
                PrintBeginningText();
                var pressedKey = Console.ReadKey();
                Console.WriteLine("\n\n");

                if (pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.NumPad1)
                {
                    ConvertToExe();
                }
                else if (pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.NumPad2)
                {
                    ConvertToMp4();
                }
                else
                {
                    Exit = true;
                }
                Console.Clear();
            }
        }

        private static void InitiateItemsList()
        {
            MyList = new List<DirectoryItem>();
            validSelection = false;
            selection = 0;
        }

        private static void PrintBeginningText()
        {
            Console.WriteLine("Hello...Welcome to extension changer!");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Current Directory ({path})");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Please Choose an extension to change the files to: \n1) Change to .Exe\n2) Change to .Mp4\n3) Any other key to EXIT");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
        }

        private static void ConvertToExe()
        {
            while (!validSelection)
                IterateThroughPath();
            ConvertItems(selection, GeneralEnums.ConvertFilesTo.exe);
        }

        private static void ConvertToMp4()
        {
            while (!validSelection)
                IterateThroughPath();
            ConvertItems(selection, GeneralEnums.ConvertFilesTo.mp4);
        }

        private static void IterateThroughPath()
        {
            int counter = 0;
            if (Directory.Exists(path))
            {
                PopulateItemsList();

                MyList.ForEach(item =>
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
                validSelection = true;
                selection = promptAsNumber;
            }
        }

        private static void AddTheAllOption()
        {
            if (MyList.Count > 0)
                Console.WriteLine($"{MyList.Count + 1}) All the files and sub-directories");
        }

        private static void OutputItemsToConsole(DirectoryItem item, int counter)
        {
            Console.WriteLine(item.isFile ?
                                   $"{counter}) FileName:{item.ItemName}, FileExtension:{item.ItemExtension}" :
                                   $"{counter}) **Directory Name:{item.ItemName}");
        }

        private static void PopulateItemsList()
        {
            var directoryFiles = Directory.GetFileSystemEntries(path);
            if (directoryFiles.Count() == 1)
                Console.WriteLine("No files or sub-directories are present...");
            foreach (var file in directoryFiles)
            {
                if (file == path + "\\" + appName)
                    continue;
                var item = ItemsConverter.ConvertToDirectoryItem(file);
                MyList.Add(item);
            }

            if (MyList.Count > 0)
            {
                MyList = MyList.OrderBy(il => il.isFile).ToList();
            }
        }

        private static void ConvertItems(int ItemNumber, GeneralEnums.ConvertFilesTo convertType)
        {
            if (ItemNumber == 0)
                return;

            //All files and subdirectories' files are to be renamed
            if (ItemNumber - MyList.Count == 1)
            {
                int fileItemNumber = 1;
                foreach (var fileItem in MyList)
                {
                    ConvertItem(fileItemNumber, convertType);
                    fileItemNumber++;
                }
            }

            //A single file or directory selected
            if (ItemNumber <= MyList.Count)
            {
                ConvertItem(ItemNumber, convertType);
            }

            //Invalid selection
            if (ItemNumber - MyList.Count > 1)
            {
                return;
            }

        }

        private static void ConvertItem(int ItemNumber, GeneralEnums.ConvertFilesTo convertType)
        {
            Console.WriteLine($"Converting to {convertType}\n");

            var item = MyList[ItemNumber - 1];
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

        private static void ConvertFile(GeneralEnums.ConvertFilesTo convertType, DirectoryItem fileItem)
        {
            var oldName = fileItem.ItemName + fileItem.ItemExtension;
            var newName = fileItem.ItemName + "." + convertType;
            if (fileItem.ItemPath != path + "\\" + appName && !fileItem.ItemPath.HasExtension(convertType))
            {
                Console.WriteLine($"Old File Name: {fileItem.ItemName}{fileItem.ItemExtension}\nNew File Name: {fileItem.ItemName}.{convertType}");
                File.Move(fileItem.ItemPath, fileItem.ItemPath.Replace(oldName, newName));
            }
        }

        private static void ConvertDirectory(GeneralEnums.ConvertFilesTo convertType, string path)
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

        private static bool HasExtension(this string file, GeneralEnums.ConvertFilesTo conv)
        {
            return file.Contains("." + conv.ToString());
        }
    }

    public class GeneralEnums
    {
        public enum ConvertFilesTo
        {
            exe,
            mp4
        }
    }
}
