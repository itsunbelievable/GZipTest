using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class Worker
    {
        private Stopwatch _stopwatch;
        private Queue _bl;
        private bool _shouldStop;

        public Worker()
        {
            _stopwatch = Stopwatch.StartNew();
            _bl = new Queue(50);
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { e.Cancel = false; };
        }

        public void Reader(string inputFile, CompressionMode mode)
        {
            Console.CancelKeyPress += ExitEvent;
            try
            {
                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException();
                }
                using (FileStream fs = new FileStream(inputFile,FileMode.OpenOrCreate,FileAccess.ReadWrite))
                {
                    using (GZipStream gz = new GZipStream(fs,mode,false))
                    {
                        //chunk size
                        byte[]buffer = new byte[1024*1024];
                        uint numBytesToRead = mode == CompressionMode.Compress ? (uint)fs.Length : GetGzBytesToRead(fs);
                        fs.Seek(0, SeekOrigin.Begin);

                        while (!_shouldStop)
                        {
                            if (numBytesToRead > 0)
                            {
                                uint bytesRead = mode == CompressionMode.Compress ? (uint)fs.Read(buffer, 0, buffer.Length) : (uint)gz.Read(buffer, 0, buffer.Length);
                                byte[] outArray = new byte[bytesRead];
                                Buffer.BlockCopy(buffer, 0, outArray, 0, (int)bytesRead);
                                numBytesToRead -= bytesRead;
                                _bl.Enqueue(outArray);
                            }

                            if (numBytesToRead != 0 || _bl.GetCount() != 0)
                                continue;
                            _bl.Stop();
                            break;
                        }
                        Console.WriteLine("Reader Terminated");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} occured. {1}",ex.GetType(),ex.Message);
            }
        }

        public void Writer(string outputFile, CompressionMode mode)
        {
            try
            {
                using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                {
                    using (GZipStream gz = new GZipStream(fs, mode, false))
                    {
                        byte[] buffer;

                        while (!_shouldStop)
                        {
                            _bl.Dequeue(out buffer);

                            if (buffer == null)
                            {
                                _stopwatch.Stop();
                                Console.WriteLine("Elapsed time: {0} seconds", _stopwatch.Elapsed.TotalSeconds);
                                Console.ReadKey();
                                return;
                            }
                            if (mode == CompressionMode.Compress)
                            {
                                gz.Write(buffer, 0, buffer.Length);
                            }
                            else
                            {
                                fs.Write(buffer, 0, buffer.Length);
                            }

                        }
                        Console.WriteLine("Writer Terminated");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("An error {0} has occured. Message: {1}", ex.GetType(), ex.Message);
            }
        }

        private uint GetGzBytesToRead(FileStream fs)
        {
            fs.Position = fs.Length - 4;
            byte[] b = new byte[4];
            fs.Read(b, 0, 4);
            return BitConverter.ToUInt32(b, 0);
        }

        public void ExitEvent(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Terminating Threads");
            _shouldStop = true;
        }
    }
}
