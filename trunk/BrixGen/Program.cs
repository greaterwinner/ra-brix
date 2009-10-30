using System;
using System.Collections.Generic;
using System.Text;
using Ra.Brix.Generator;
using System.IO;

namespace BrixGen
{
    class Program
    {
        /// <summary>
        /// usage: BrixGen.exe Fully.Qualified.Module.Name path/to/output/folder
        /// use: BrixGen.exe Fully.Qualified.Module.Name . 
        /// to output to same folder
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                string moduleName = args[0];
                string folderName = Path.GetFullPath(args[1]);

                if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(folderName))
                {
                    ModuleGenerator gen = new ModuleGenerator(moduleName);

                    Console.WriteLine(gen.CreateFiles(folderName));
                }
            }
        }
    }
}
