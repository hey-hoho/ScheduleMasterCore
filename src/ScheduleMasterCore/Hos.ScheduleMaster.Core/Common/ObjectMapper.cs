using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hos.ScheduleMaster.Core.Common
{
    public class ClassA
    {
        [MapperSetting(BindField = "Aa")]
        public string A { get; set; }

        [MapperSetting(Ignore = true)]
        public string B { get; set; }

        public int C { get; set; }
    }

    public class ClassB
    {
        public string Aa { get; set; }
        public string B { get; set; }
        public int C { get; set; }
        public bool D { get; set; }
    }

    public class MapperDemo
    {
        public void Demo()
        {
            ClassA a = new ClassA { A = "1", B = "2", C = 3 };
            //方式一，返回新对象：
            ClassB b = ObjectMapper<ClassA, ClassB>.Convert(a);
            //b.D为False

            ClassB b1 = new ClassB { D = true };
            //方式二，保留目标对象的字段值：
            ObjectMapper<ClassA, ClassB>.Convert(a, b1);
            //b.D为True
        }
    }

    /// <summary>
    /// 映射配置标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapperSettingAttribute : Attribute
    {
        /// <summary>
        /// 指定映射字段名称
        /// </summary>
        public string BindField;

        /// <summary>
        /// 映射时是否忽略该字段
        /// </summary>
        public bool Ignore;
    }

    /// <summary>
    /// 对象映射
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ObjectMapper<T1, T2> where T1 : new() where T2 : new()
    {
        public static T2 Convert(T1 t1)
        {
            T2 t2 = new T2();
            return Convert(t1, t2);
        }

        public static T2 Convert(T1 t1, T2 t2)
        {
            if (t1 == null) return t2;
            var dtoProperList = t1.GetType().GetProperties().Where(p => p.PropertyType.IsPublic == true).ToList();
            var entityProperList = t2.GetType().GetProperties().Where(p => p.PropertyType.IsPublic == true).ToList();
            foreach (System.Reflection.PropertyInfo pi in dtoProperList)
            {
                try
                {
                    string realName = pi.Name;
                    object[] cusAttrs = pi.GetCustomAttributes(typeof(MapperSettingAttribute), true);
                    if (cusAttrs.Length > 0)
                    {
                        MapperSettingAttribute attr = cusAttrs[0] as MapperSettingAttribute;
                        if (attr != null)
                        {
                            if (attr.Ignore)
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(attr.BindField))
                            {
                                realName = attr.BindField;
                            }
                        }
                    }
                    var entityPi = entityProperList.FirstOrDefault(p => p.Name == realName);
                    if (entityPi == null)
                    {
                        continue;
                    }

                    //wolf 添加
                    if (entityPi.PropertyType == typeof(string) && pi.PropertyType == typeof(string[]))
                    {
                        continue;
                    }
                    else if (entityPi.PropertyType == typeof(string[]) && pi.PropertyType == typeof(string))
                    {
                        continue;
                    }

                    object value = pi.GetValue(t1, null);
                    if (value == null)
                    {
                        continue;
                    }
                    entityPi.SetValue(t2, value, null);
                }
                catch (Exception)
                {
                    continue;
                }

            }
            return t2;
        }
    }
}
