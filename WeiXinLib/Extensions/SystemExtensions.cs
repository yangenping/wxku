using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 系统扩展
    /// </summary>
    public static class SystemExtensions
    {
        public static long TimeStamp(this DateTime dateTime)
        {
            DateTime timeStamp = new DateTime(1970, 1, 1,0,0,0,0); //得到1970年的时间戳
            long ticks = DateTime.UtcNow.Ticks - timeStamp.Ticks;
            ticks /= 10000000; //Convert windows ticks to seconds //注意这里有时区问题，用now就要减掉8个小时

            return ticks;

        }
    }
}
