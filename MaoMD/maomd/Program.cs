using maomdlib;
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
            if (args.Length == 1)
            {

            }
            using (DocMaker docMaker = new DocMaker(dllFile,xmlFile,outputDir))
            {
                docMaker.Make();
            };
        }
    }
}
