using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    public class Task
    {
        private string InputFile { get; set; }
        private string OutputFile { get; }
        private readonly Worker _worker;

        public Task(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
            _worker = new Worker();

            
        }

        public void Compress()
        {
            
            Thread readerThread = new Thread(()=>_worker.Reader(InputFile,CompressionMode.Compress));
            Thread writerThread = new Thread(()=>_worker.Writer(OutputFile,CompressionMode.Compress));

            readerThread.Start();
            writerThread.Start();

            if (true)
            {
                
            }
        }

        public void Decompress()
        {
            Thread readerThread = new Thread(() => _worker.Reader(InputFile, CompressionMode.Decompress));
            Thread writerThread = new Thread(() => _worker.Writer(OutputFile, CompressionMode.Decompress));

            readerThread.Start();
            writerThread.Start();
        }

    }
}
