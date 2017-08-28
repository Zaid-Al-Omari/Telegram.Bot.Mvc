using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Mvc.Core;

namespace Telegram.Bot.Mvc.Scheduler
{
    public class PerSecondScheduler : IDisposable
    {
        private ILogger _logger;
        private int _tasksCount;
        private int _inSeconds;
        private int _innerDelay;

        private MultiLevelPriorityQueue<Task> _queue;
        private Thread _thread;
        private Semaphore _semaphore;
        private ManualResetEvent _waitHandler = new ManualResetEvent(false);

        public PerSecondScheduler(ILogger logger, int tasksCount = 30, int inSeconds = 1)
        {
            _logger = logger;
            _tasksCount = tasksCount;
            _inSeconds = inSeconds;
            _innerDelay = ((1000 * _inSeconds) / _tasksCount) + 1;
            _queue = new MultiLevelPriorityQueue<Task>();
            _semaphore = new Semaphore(_tasksCount, _tasksCount);
            _thread = new Thread(Start);
            _thread.Start();
        }

        public void Clear(int? priority = null)
        {
            _queue.Clear(priority);
        }

        public void Enqueue(Task task, int priority = 0)
        {
            if (task == null) return;
            _queue.Enqueue(task, priority: priority);
            Resume();
        }

        public void Enqueue(int delay = 1000, int priority = 0, params Task[] tasks)
        {
            if (tasks == null || tasks.Length == 0) return;
            var compiledTask = new Task(async () =>
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    if (i > 0) _semaphore.WaitOne();
                    tasks[i].Start();
                    if (i > 0) _semaphore.Release();
                    await Task.Delay(delay);
                }
                await Task.WhenAll(tasks);
            });
            Enqueue(compiledTask, priority);
        }

        public void Pause()
        {
            _waitHandler.Reset();
        }

        public void Resume()
        {
            _waitHandler.Set();
        }


        private volatile bool _runThread = true;
        private async void Start()
        {
            var handlers = new List<Task>();
            while (true)
            {
                _waitHandler.WaitOne();
                if (!_runThread) break;
                Task t = _queue.Dequeue();
                if (t == null)
                {
                    Pause();
                    continue;
                }
                _semaphore.WaitOne();
                var task = Task.Run(async () =>
                {
                    Task localTask = t;
                    localTask.Start();
                    _semaphore.Release();
                    await localTask;
                }).ContinueWith(x => { lock (handlers) { handlers.Remove(x); } })
                  .ContinueWith(x => { _logger.Log(x.Exception); }, TaskContinuationOptions.OnlyOnFaulted);

                lock (handlers) { handlers.Add(task); }
                await Task.Delay(_innerDelay);
            }
            handlers.Clear();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _runThread = false;
                    _thread.Interrupt();
                    _thread.Join();
                    lock (_queue)
                    {
                        _queue.Dispose();
                    }
                }
                _thread = null;
                _queue = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public class MultiLevelPriorityQueue<T> : IDisposable where T : class
    {
        private Queue<T>[] _queue;
        private const int PRIORITY_MAX = 5;
        private static readonly int[] QUEUE_SLOTS = new int[] { 15, 6, 5, 3, 1 };
        private static readonly int TASKS_COUNT = QUEUE_SLOTS.Sum();

        public MultiLevelPriorityQueue()
        {
            _queue = new Queue<T>[PRIORITY_MAX];
            for (int i = 0; i < _queue.Length; i++)
            {
                _queue[i] = new Queue<T>();
                _currantQueueSlots[i] = QUEUE_SLOTS[i];
            }
        }

        public void Enqueue(T item, int priority = 0)
        {
            if (item == null) return;
            lock (_queue[priority])
            {
                _queue[priority].Enqueue(item);
            }
        }

        private int[] _currantQueueSlots = new int[PRIORITY_MAX];
        private volatile int _currentQueue = 0;
        private Queue<T> GetCurrentQueue()
        {
            int i = _currentQueue;
            for (int tries = 0; tries < PRIORITY_MAX; tries++)
            {
                int currentSlot = Volatile.Read(ref _currantQueueSlots[i]);
                if (currentSlot > 0)
                {
                    int queueItemsCount;
                    lock (_queue[i]) { queueItemsCount = _queue[i].Count; }
                    if (queueItemsCount > 0) return _queue[i];
                }
                Volatile.Write(ref _currantQueueSlots[i], QUEUE_SLOTS[i]);
                _currentQueue = (_currentQueue + 1) % PRIORITY_MAX;
                i = (i + 1) % PRIORITY_MAX;
            }
            return null;
        }

        public T Dequeue()
        {
            T t = null;
            var queue = GetCurrentQueue();
            if (queue == null) return null;
            lock (queue)
            {
                if (queue.Count > 0) t = queue.Dequeue();
            }
            return t;
        }

        public void Clear(int? priority)
        {
            if (priority.HasValue)
            {
                lock (_queue[priority.Value])
                {
                    _queue[priority.Value].Clear();
                }
            }
            else
            {
                for (int i = 0; i < PRIORITY_MAX; i++) Clear(i);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var q in _queue)
                    {
                        q.Clear();
                    }
                    _queue = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
