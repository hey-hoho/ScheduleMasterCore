using Hos.ScheduleMaster.Core.Dto;
using Hos.ScheduleMaster.Core.Log;
using Hos.ScheduleMaster.Core.Models;
using Hos.ScheduleMaster.QuartzHost.HosSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.QuartzHost.Common
{
    public class HosScheduleFactory
    {
        public static IHosSchedule GetHosSchedule(ScheduleView view)
        {
            IHosSchedule result;
            switch ((ScheduleMetaType)view.Schedule.MetaType)
            {
                case ScheduleMetaType.Assembly: { result = new AssemblySchedule(); break; }
                case ScheduleMetaType.Http: { result = new HttpSchedule(); break; }
                default: throw new InvalidOperationException("unknown schedule type.");
            }
            result.Main = view.Schedule;
            result.CustomParams = ConvertParamsJson(view.Schedule.CustomParamsJson);
            result.Keepers = view.Keepers;
            result.Children = view.Children;
            result.CreateRunnableInstance(view);
            return result;
        }

        public static Dictionary<string, object> ConvertParamsJson(string source)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                List<ScheduleParam> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ScheduleParam>>(source);
                foreach (var item in list)
                {
                    result[item.ParamKey] = item.ParamValue;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return result;
        }
    }
}
