using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCS
{
    public static class ThreadManager
    {
        #region Fields 

        /// <summary>
        /// 运行中线程数
        /// </summary>
        private static int _threadNum = 0;
        /// <summary>
        /// 统计线程锁
        /// </summary>
        private static object _lockObj = new object();

        #endregion

        #region Properties

        /// <summary>
        /// 运行中线程数
        /// </summary>
        public static int ThreadNum { get => _threadNum; }

        #endregion

        #region Methods

        public static void NoteTreadNum(int num)
        {
            lock (_lockObj)
            {
                _threadNum += num;
            }
        }
        #endregion
    }
}
