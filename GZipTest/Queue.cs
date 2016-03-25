using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipTest
{
    public class Queue
    {
        private readonly Queue<byte[]> _queue = new Queue<byte[]>();
        private bool _isStopped { get; set; }

        private byte _maxSize { get; }

        public Queue(byte maxSize)
        {
            _maxSize = maxSize;
        }
        public void Enqueue(byte[] item)
        {
            lock (_queue)
            {
                if (_queue.Count > _maxSize)
                {
                    GC.Collect();
                    Monitor.Wait(_queue);
                }
                _queue.Enqueue(item);
                Monitor.PulseAll(_queue);
            }
        }

        public void Dequeue(out byte[] buffer)
        {
            lock (_queue)
            {
                if (_queue.Count == 0 && !_isStopped)
                {
                    Monitor.Wait(_queue);
                }

                if (_isStopped)
                {
                    Monitor.PulseAll(_queue);
                    buffer = default(byte[]);
                    return;
                }
                buffer = _queue.Dequeue();
                Monitor.PulseAll(_queue);
            }
        }

        public int GetCount()
        {
            lock (_queue)
            {
                return _queue.Count();
            }
        }

        public void Stop()
        {
            lock (_queue)
            {
                Console.WriteLine("Stopped!");
                _isStopped = true;
                Monitor.PulseAll(_queue);
            }
        }
    }
}
