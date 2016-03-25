using System;
using System.IO;
using System.IO.Compression;
namespace GZipTest
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Usage: compress|decompress source file output file");
            if (args.Length < 3)
            {
                Console.WriteLine("Input param mismatch");
                return 1;
            }

            try
            {
                string task = args[0];
                string sourceFile = args[1];
                string outputFile = args[2];
                Task t = new Task(sourceFile,outputFile);
                switch (task.ToLower())
                {
                    case "compress":
                    {
                        t.Compress();
                        break;
                    }
                    case "decompress":
                    {
                        t.Decompress();
                        break;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An {0} in {1} occured",ex.GetType(),ex.Message);
                return 0;
            }
        }
    }
}
