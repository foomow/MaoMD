﻿using maomdlib;
using System;

namespace maomd
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllFile = "";
            string xmlFile = "";
            string outputDir = "";
            bool isNoPath = true;
            string linkroot = "";
            if (args.Length == 1&&(args[0].ToLower() == "-h"||args[0] == " ?"))
            {
                Console.WriteLine("Welcome MaoMD! 'MaoMD -h' for help.");
                Console.WriteLine("Usage:");
                Console.WriteLine("MaoMD [-h||?] || [dllFile [xmlFile [outputDir [isNoPath [linkRoot [logger]]]]]]");
                Console.WriteLine("parameters:");
                Console.WriteLine("dllFile: The source dll file.");
                Console.WriteLine("xmlFile: The xml documentation file generated by Visual Studio.");
                Console.WriteLine("outputDir: The destination directory to put all .md files.");
                Console.WriteLine("isNoPath: Whether the destination system support path or not.");
                Console.WriteLine("linkRoot: The root path of all .md file. Use on systems supported path.");
                Console.WriteLine("no parameters: make documents with myself as a demo.");
                Console.WriteLine(@"github:https://github.com/foomow/MaoMD");
            }
            if (args.Length > 0)
            {
                dllFile = args[0];
            }
            if (args.Length > 1)
            {
                xmlFile = args[1];
            }
            if (args.Length > 2)
            {
                outputDir = args[2];
            }
            if (args.Length > 3)
            {
                isNoPath = args[3].ToLower()!="false";
            }
            if (args.Length > 4)
            {
                linkroot = args[4];
            }

            using (DocMaker docMaker = new DocMaker(dllFile, xmlFile, outputDir, isNoPath, linkroot))
            {
                docMaker.Make();
            };
        }        
    }
}
