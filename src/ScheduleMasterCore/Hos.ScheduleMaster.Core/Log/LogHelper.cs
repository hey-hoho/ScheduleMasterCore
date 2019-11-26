using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Log
{
    public class LogHelper
    {

        public static void Info(string message)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Info,
                Message = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Info(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Info,
                Message = message,
                ScheduleId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Warn,
                Message = message,
                CreateTime = DateTime.Now
            });
        }

        public static void Warn(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Warn,
                Message = message,
                ScheduleId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(Exception ex)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(Exception ex, Guid task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                ScheduleId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = $"{message}，ERROR：{exp.Message}",
                StackTrace = exp.StackTrace,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Guid task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = message,
                ScheduleId = task,
                CreateTime = DateTime.Now
            });
        }

        public static void Error(string message, Exception exp, Guid task)
        {
            LogManager.Queue.Write(new SystemLogEntity
            {
                Category = (int)LogCategory.Error,
                Message = $"{message}，ERROR：{exp.Message}",
                StackTrace = exp.StackTrace,
                ScheduleId = task,
                CreateTime = DateTime.Now
            });
        }
    }

}
