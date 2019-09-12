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
            
            using (DocMaker docMaker = new DocMaker(dllFile, xmlFile, outputDir))
            {
                docMaker.Make();
            };
        }        
    }
}
