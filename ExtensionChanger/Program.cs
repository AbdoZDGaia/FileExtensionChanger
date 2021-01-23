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
        public static string path = Directory.GetCurrentDirectory();
        public static readonly string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        public static readonly string appName = Path.GetFileName(codeBase);

        static void Main(string[] args)
        {
            bool Exit = false;
            while (!Exit)
            {
                PrintBeginningText();
                var pressedKey = Console.ReadKey();
                Console.WriteLine("\n\n");

                if (pressedKey.Key == ConsoleKey.D1 || pressedKey.Key == ConsoleKey.NumPad1)
                {
                    var conversion = new Conversion(path, appName);
                    conversion.ConvertToExe();
                }
                else if (pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.NumPad2)
                {
                    var conversion = new Conversion(path, appName);
                    conversion.ConvertToMp4();
                }
                else if (pressedKey.Key == ConsoleKey.D3 || pressedKey.Key == ConsoleKey.NumPad3)
                {
                    var encryption = new Encryption(path, appName);
                    encryption.EncryptFiles();
                }
                else if (pressedKey.Key == ConsoleKey.D4 || pressedKey.Key == ConsoleKey.NumPad4)
                {
                    var encryption = new Encryption(path, appName);
                    encryption.DecryptFiles();
                }
                else
                {
                    Exit = true;
                }
                Console.Clear();
            }
        }

        private static void PrintBeginningText()
        {
            Console.WriteLine("Hello...Welcome to extension changer!");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Current Directory ({path})");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Please Choose an extension to change the files to: \n1) Change to .Exe\n2) Change to .Mp4\n3) Encrypt Files\n4) Decrypt Files\n5) Any other key to EXIT");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
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
