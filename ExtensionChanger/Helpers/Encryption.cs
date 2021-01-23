using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionChanger.Helpers
{
    public class Encryption
    {
        private readonly string _path;
        private readonly string _appName;
        private bool _validSelection;
        private int _selection;
        private string passPhrase;
        private string saltValue;
        private List<DirectoryItem> _myList;
        Common common;

        public Encryption(string path, string appName)
        {
            int selection = 0;
            bool validSelection = false;
            passPhrase = secrets.pass;
            saltValue = secrets.salt;
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

        internal void EncryptFiles()
        {
            while (!_validSelection)
                ReInitialize(common.IterateThroughPath());
            EncryptItems(_selection);
        }

        internal void EncryptItems(int ItemNumber)
        {
            if (ItemNumber == 0)
                return;

            //All files and subdirectories' files are to be renamed
            if (ItemNumber - _myList.Count == 1)
            {
                int fileItemNumber = 1;
                foreach (var fileItem in _myList)
                {
                    EncryptItem(fileItemNumber);
                    fileItemNumber++;
                }
            }

            //A single file or directory selected
            if (ItemNumber <= _myList.Count)
            {
                EncryptItem(ItemNumber);
            }

            //Invalid selection
            if (ItemNumber - _myList.Count > 1)
            {
                return;
            }

        }

        internal void DecryptItems(int ItemNumber)
        {
            if (ItemNumber == 0)
                return;

            //All files and subdirectories' files are to be renamed
            if (ItemNumber - _myList.Count == 1)
            {
                int fileItemNumber = 1;
                foreach (var fileItem in _myList)
                {
                    DecryptItem(fileItemNumber);
                    fileItemNumber++;
                }
            }

            //A single file or directory selected
            if (ItemNumber <= _myList.Count)
            {
                DecryptItem(ItemNumber);
            }

            //Invalid selection
            if (ItemNumber - _myList.Count > 1)
            {
                return;
            }

        }

        internal void DecryptFiles()
        {
            while (!_validSelection)
                ReInitialize(common.IterateThroughPath());
            DecryptItems(_selection);
        }

        internal void EncryptItem(int ItemNumber)
        {
            Console.WriteLine($"Encrypting Items\n");

            var item = _myList[ItemNumber - 1];
            var path = item.ItemPath;

            if (Directory.Exists(path) && !item.isFile)
            {
                EncryptDirectory(path);
            }

            if (File.Exists(path) && item.isFile)
            {
                EncryptFile(item);
            }
        }

        internal void DecryptItem(int ItemNumber)
        {
            Console.WriteLine($"Decrypting Items\n");

            var item = _myList[ItemNumber - 1];
            var path = item.ItemPath;

            if (Directory.Exists(path) && !item.isFile)
            {
                DecryptDirectory(path);
            }

            if (File.Exists(path) && item.isFile)
            {
                DecryptFile(item);
            }
        }

        internal void EncryptFile(DirectoryItem fileItem)
        {

            var oldName = fileItem.ItemName + fileItem.ItemExtension;
            if (fileItem.ItemPath != _path + "\\" + _appName && !HasExtension(fileItem.ItemPath, "DC"))
            {
                var fileBytes = File.ReadAllBytes(fileItem.ItemPath);
                var RM = new RijndaelManaged();
                RM.Mode = CipherMode.CBC;
                var salt = Encoding.ASCII.GetBytes(saltValue);
                var password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);
                var encryptor = RM.CreateEncryptor(password.GetBytes(32), password.GetBytes(16));
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(fileBytes, 0, fileBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    File.WriteAllBytes(fileItem.ItemPath + ".DC", ms.ToArray());
                    File.Delete(fileItem.ItemPath);
                }
            }
        }

        internal bool HasExtension(string file, string extension)
        {
            return file.Contains("." + extension);
        }

        internal void EncryptDirectory(string path)
        {
            var directoryFiles = Directory.GetFiles(path);
            if (directoryFiles.Count() == 0)
                Console.WriteLine("No files or sub-directories were present...Nothing's changed");
            foreach (var file in directoryFiles)
            {
                var itemFile = ItemsConverter.ConvertToDirectoryItem(file);
                EncryptFile(itemFile);
            }
        }

        internal void DecryptFile(DirectoryItem fileItem)
        {
            var oldName = fileItem.ItemName + fileItem.ItemExtension;
            if (fileItem.ItemPath != _path + "\\" + _appName && HasExtension(fileItem.ItemPath, "DC"))
            {
                byte[] plainBytes;
                var encryptedBytes = File.ReadAllBytes(fileItem.ItemPath);
                var RM = new RijndaelManaged();
                RM.Mode = CipherMode.CBC;
                var salt = Encoding.ASCII.GetBytes(saltValue);
                var password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);
                var decryptor = RM.CreateDecryptor(password.GetBytes(32), password.GetBytes(16));
                using (var ms = new MemoryStream(encryptedBytes))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        plainBytes = new byte[encryptedBytes.Length];
                        cs.Read(plainBytes, 0, plainBytes.Length);
                    }
                    File.Delete(fileItem.ItemPath);
                    File.WriteAllBytes(fileItem.ItemPath.Replace(".DC", ""), plainBytes);
                }
            }
        }

        internal void DecryptDirectory(string path)
        {
            var directoryFiles = Directory.GetFiles(path);
            if (directoryFiles.Count() == 0)
                Console.WriteLine("No files or sub-directories were present...Nothing's changed");
            foreach (var file in directoryFiles)
            {
                var itemFile = ItemsConverter.ConvertToDirectoryItem(file);
                DecryptFile(itemFile);
            }
        }
    }
}
