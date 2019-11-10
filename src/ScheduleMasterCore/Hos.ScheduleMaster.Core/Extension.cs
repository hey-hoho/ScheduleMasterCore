using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core
{
  public static  class Extension
    {
        /// <summary>
        /// 格式化为yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string StringFormat(this DateTime? obj)
        {
            if (!obj.HasValue)
            {
                return string.Empty;
            }
            return StringFormat(obj.Value);
        }

        /// <summary>
        /// 格式化为yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string StringFormat(this DateTime obj)
        {
            return obj.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化为yyyy-MM-dd
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DateFormat(this DateTime? obj)
        {
            if (!obj.HasValue)
            {
                return string.Empty;
            }
            return DateFormat(obj.Value);
        }

        /// <summary>
        /// 格式化为yyyy-MM-dd
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DateFormat(this DateTime obj)
        {
            return obj.ToString("yyyy-MM-dd");
        }

    }
}
