using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packer
{
    public class Packer
    {
        public static string Pack(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            StringBuilder sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(new Package(line).FindItems());
            }

            return sb.ToString();
        }
    }
}
