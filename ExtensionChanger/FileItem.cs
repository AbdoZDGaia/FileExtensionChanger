using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionChanger
{
    public class DirectoryItem
    {
        public Guid Id { get; set; }
        public string ItemPath { get; set; }
        public string ItemName { get; set; }
        public string ItemExtension { get; set; }
        public bool isFile { get; set; }
    }
}
