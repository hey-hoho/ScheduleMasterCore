using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Hos.ScheduleMaster.QuartzHost.DelayedTask
{
    /// <summary>
    /// 时间轮算法实现的延时队列
    /// by hoho
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelayQueue<T> where T : DelayQueueSlot
    {
        private List<T>[] _queue;

        private volatile int _currentSlot = 0;

        private bool _isClearing = false;

        private int _count = 0;

        private static object _lockObj = new object();

        public DelayQueue(int length)
        {
            _queue = new List<T>[length];
            for (int i = 0; i < length; i++)
            {
                _queue[i] = new List<T>();
            }
        }

        public int Count => _count;

        public bool IsEmpty
        {
            get
            {
                return _count == 0;
            }
        }

        public bool Insert(T item, int delaySeconds)
        {
            if (_isClearing) return false;

            lock (_lockObj)
            {
                //根据消费时间计算消息应该放入的位置
                item.CycleNum = delaySeconds / _queue.Length;
                item.Slot = (delaySeconds + _currentSlot) % _queue.Length;
                item.Version = 0;
                item.TimeSpan = delaySeconds;
                //加入到延时队列中
                _queue[item.Slot].Add(item);
                Interlocked.Increment(ref _count);
                return true;
            }
        }

        public bool Remove(string slotKey)
        {
            var item = _queue.SelectMany(x => x).FirstOrDefault(x => x.Key == slotKey);
            if (item == null) return false;
            if (_queue[item.Slot] != null)
            {
                int v = item.Version;
                if (Interlocked.CompareExchange(ref item.Version, v + 1, v) == v)
                {
                    _queue[item.Slot].Remove(item);
                    Interlocked.Decrement(ref _count);
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _isClearing = true;
            Parallel.For(0, _queue.Length, (i) =>
            {
                _queue[i]?.Clear();
            });
            _count = 0;
            _isClearing = false;
        }

        public void Read()
        {
            if (_queue.Length > _currentSlot)
            {
                var list = _queue[_currentSlot];
                if (list != null)
                {
                    //在本轮命中，用单独线程去执行业务操作
                    var target = list.Where(x => x.CycleNum == 0).ToList();
                    target.ForEach(item =>
                    {
                        int v = item.Version;
                        if (Interlocked.CompareExchange(ref item.Version, v + 1, v) == v)
                        {
                            Task.Run(() =>
                            {
                                item.Callback(item);
                            });
                            //把已过期的移除掉
                            list.Remove(item);
                            Interlocked.Decrement(ref _count);
                        }
                    });
                    //剩下的等待下一轮
                    list.ForEach(item =>
                    {
                        item.CycleNum--;
                        //System.Diagnostics.Debug.WriteLine($"@@槽点：{item.Slot.ToString()}，剩余环数：{item.CycleNum.ToString()}");
                    });
                }
                //往后移动一位
                _currentSlot++;
                //如果到了尽头下一遍就从头开始
                if (_currentSlot >= _queue.Length)
                {
                    _currentSlot = 0;
                }
            }
        }
    }

    public class DelayQueueSlot
    {
        internal volatile int Version;

        public string Key { get; set; }

        public int TimeSpan { get; internal set; }

        internal int Slot { get; set; }

        internal int CycleNum { get; set; }

        public Func<DelayQueueSlot, Task> Callback { get; set; }
    }
}
