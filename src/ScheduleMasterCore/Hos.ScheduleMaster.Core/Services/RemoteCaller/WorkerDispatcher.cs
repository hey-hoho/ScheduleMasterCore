using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Services.RemoteCaller
{
    [ServiceMapTo(typeof(WorkerDispatcher))]
    public class WorkerDispatcher
    {
        delegate Task<bool> RequestDelegate(ServerNodeEntity server);

        [Autowired]
        public INodeService _nodeService { get; set; }

        private ScheduleServiceClient _scheduleClient;

        private DelayedTaskServiceClient _delayedTaskServiceClient;

        public WorkerDispatcher(ScheduleServiceClient scheduleClient, DelayedTaskServiceClient delayedTaskServiceClient)
        {
            _scheduleClient = scheduleClient;
            _delayedTaskServiceClient = delayedTaskServiceClient;
        }

        private async Task<bool> DispatcherHandler(Guid sid, RequestDelegate func)
        {
            var nodeList = _nodeService.GetAvaliableWorkerForSchedule(sid);
            if (nodeList.Any())
            {
                foreach (var item in nodeList)
                {
                    if (!await func(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            throw new InvalidOperationException("running worker not found.");
        }

        public async Task<bool> ScheduleStart(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
             {
                 _scheduleClient.Server = node;
                 return await _scheduleClient.Start(sid);
             });
        }

        public async Task<bool> ScheduleStop(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
            {
                _scheduleClient.Server = node;
                return await _scheduleClient.Stop(sid);
            });
        }

        public async Task<bool> SchedulePause(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
            {
                _scheduleClient.Server = node;
                return await _scheduleClient.Pause(sid);
            });
        }

        public async Task<bool> ScheduleResume(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
            {
                _scheduleClient.Server = node;
                return await _scheduleClient.Resume(sid);
            });
        }

        public async Task<bool> ScheduleRunOnce(Guid sid)
        {
            ServerNodeEntity node = _nodeService.GetAvaliableWorkerByPriority(sid).FirstOrDefault();

            _scheduleClient.Server = node;
            return await _scheduleClient.RunOnce(sid);
        }

        public async Task<bool> DelayedTaskStart(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
            {
                _delayedTaskServiceClient.Server = node;
                return await _delayedTaskServiceClient.Start(sid);
            });
        }

        public async Task<bool> DelayedTaskRemove(Guid sid)
        {
            return await DispatcherHandler(sid, async (ServerNodeEntity node) =>
            {
                _delayedTaskServiceClient.Server = node;
                return await _delayedTaskServiceClient.Remove(sid);
            });
        }

        public async Task<bool> DelayedTaskExecute(Guid sid)
        {
            ServerNodeEntity node = _nodeService.GetAvaliableWorkerByPriority(sid).FirstOrDefault();

            _delayedTaskServiceClient.Server = node;
            return await _delayedTaskServiceClient.Execute(sid);
        }
    }
}
