using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Common
{
    /// <summary>
    /// 双向读写缓冲队列
    /// by hoho
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BufferQueue<T> where T : class
    {
        private ConcurrentQueue<T> _inQueue;

        private ConcurrentQueue<T> _outQueue;

        /// <summary>
        /// 每次批量读出的数量，默认为1000
        /// </summary>
        public int UnitReadCount = 1000;

        /// <summary>
        /// 读出完成的标记，当上次完成后才切换队列状态
        /// </summary>
        private bool _lastReadFinished = true;

        public BufferQueue()
        {
            _inQueue = new ConcurrentQueue<T>();
            _outQueue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="message"></param>
        public void Write(T message)
        {
            _inQueue.Enqueue(message);
        }

        /// <summary>
        /// 读取消息，在Action中会传入消息对象和索引
        /// </summary>
        /// <param name="action"></param>
        public void Read(Action<T, int> action)
        {
            //上一次写入是否完成
            if (_lastReadFinished)
            {
                _lastReadFinished = false;
                //切换队列
                Switch();
                //取数据写入
                QueueProcess(action);
                _lastReadFinished = true;
            }
        }

        public void Clear()
        {
            while (_inQueue.IsEmpty && _outQueue.IsEmpty)
            {
                break;
            }
            _inQueue = null;
            _outQueue = null;
        }

        /// <summary>
        /// 处理队列中的消息
        /// </summary>
        private void QueueProcess(Action<T, int> action)
        {
            if (_outQueue == null) return;
            int cnt = 0;
            while (cnt < UnitReadCount)
            {
                if (_outQueue.IsEmpty)
                {
                    break;
                }
                T item;
                if (_outQueue.TryDequeue(out item))
                {
                    action(item, cnt);
                    cnt++;
                }
            }
        }

        /// <summary>
        /// 切换队列读写状态
        /// </summary>
        private void Switch()
        {
            var q = _outQueue;
            _outQueue = _inQueue;
            _inQueue = q;
        }
    }
}
