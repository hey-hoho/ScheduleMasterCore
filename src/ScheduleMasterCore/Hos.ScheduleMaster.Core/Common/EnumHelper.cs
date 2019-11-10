using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Common
{
    public class EnumHelper
    {
        /// <summary>
        /// 根据枚举值获取描述
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        /// 用法：int value = 1;GetEnumDescription((MyEnum)value)
        public static string GetEnumDescription(Enum value)
        {
            try
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
                else
                {
                    return value.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 根据枚举值获取描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="typeValue">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDescription(Type enumType, int typeValue)
        {
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    if (typeValue == ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)))
                    {
                        object[] arr = field.GetCustomAttributes(typeDescription, true);
                        if (arr.Length > 0)
                        {
                            DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                            strText = aa.Description;
                        }
                        else
                        {
                            strText = field.Name;
                        }
                        break;
                    }
                }
            }
            return strText;
        }

        /// <summary>
        /// 获取枚举类型的数据字典
        /// </summary>
        /// <param name="enu">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetEnumData(Type enu)
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            var array = Enum.GetValues(enu);
            foreach (var item in array)
            {
                int value = (int)item;
                list.Add(GetEnumDescription(enu, value), value);
            }
            return list;
        }
    }
}
