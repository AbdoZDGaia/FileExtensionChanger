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
                    var encryption = new Encryption(path, appName);
                    encryption.EncryptFiles();

                }
                else if (pressedKey.Key == ConsoleKey.D2 || pressedKey.Key == ConsoleKey.NumPad2)
                {
                    var encryption = new Encryption(path, appName);
                    encryption.DecryptFiles();

                }
                else if (pressedKey.Key == ConsoleKey.D3 || pressedKey.Key == ConsoleKey.NumPad3)
                {
                    var conversion = new Conversion(path, appName);
                    conversion.ConvertToExe();
                }
                else if (pressedKey.Key == ConsoleKey.D4 || pressedKey.Key == ConsoleKey.NumPad4)
                {
                    var conversion = new Conversion(path, appName);
                    conversion.ConvertToMp4();
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
            Console.WriteLine("Please Choose an Option: \n1) Encrypt Files\n2) Decrypt Files\n3) Replace extension with (.exe)\n4) Replace extension with (.mp4)\n5) Any other key to EXIT");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------");
        }

    }

    public class GeneralEnums
    {
        public enum Transformations
        {
            encrypt = 1,
            decrypt = 2,
            exe = 3,
            mp4 = 4
        }
    }
}
