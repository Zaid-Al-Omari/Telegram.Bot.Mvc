using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.Scheduler
{
    public class PerSecondScheduler : IDisposable {
        private ILogger _logger;
        private int _tasksCount;
        private int _inSeconds;

        private Queue<Task> _queue;
        public Thread _thread;
        private Semaphore _semaphore;
        private ManualResetEvent _waitHandler = new ManualResetEvent(false);

        public PerSecondScheduler(ILogger logger, int tasksCount = 30, int inSeconds = 1) {
            _logger = logger;
            _tasksCount = tasksCount;
            _inSeconds = inSeconds;
            _queue = new Queue<Task>();
            _semaphore = new Semaphore(_tasksCount, _tasksCount);
            _thread = new Thread(Start);
            _thread.Start();
        }

        public void Enqueue(Task task) {
            if (task == null) return;
            lock (_queue) {
                _queue.Enqueue(task);
                Resume();
            }
        }

        public void Pause() {
            _waitHandler.Reset();
        }
        public void Resume() {
            _waitHandler.Set();
        }

        private void Start() {
            List<Task> handler = new List<Task>();
            while (true) {
                _waitHandler.WaitOne();
                Task t = null;
                lock (_queue) {
                    if (_queue.Count > 0) {
                        _semaphore.WaitOne();
                        t = _queue.Dequeue();
                    }
                    else {
                        Pause(); continue;
                    }
                }
                var task = new Task(() =>
                {
                    t.Start();
                    Task.Delay((1000 * _inSeconds) / _tasksCount).Wait();
                    _semaphore.Release();
                    t.Wait();
                });
                task.ContinueWith(x => { lock (handler) { handler.Remove(x); } });
                task.ContinueWith(x => { _logger.Log(x.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
                lock (handler) { handler.Add(task); }
                task.Start();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    Pause();
                    lock (_queue) {
                        _queue.Clear();
                    }
                }
                _thread = null;
                _queue = null;
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }
        #endregion
    }
}
