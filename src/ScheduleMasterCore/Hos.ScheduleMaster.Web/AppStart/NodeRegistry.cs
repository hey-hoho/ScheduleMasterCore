using Hos.ScheduleMaster.Core;
using Hos.ScheduleMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Web.AppStart
{
    public class NodeRegistry
    {
        public static void Register()
        {
            var setting = ConfigurationCache.NodeSetting;
            using (var scope = new Core.ScopeDbContext())
            {
                bool isCreate = false;
                var db = scope.GetDbContext();
                var node = db.ServerNodes.FirstOrDefault(x => x.NodeName == setting.IdentityName);
                if (node == null)
                {
                    isCreate = true;
                    node = new ServerNodeEntity();
                }
                node.NodeName = setting.IdentityName;
                node.NodeType = setting.Role;
                node.MachineName = Environment.MachineName;
                node.AccessProtocol = setting.Protocol;
                node.Host = $"{setting.IP}:{setting.Port}";
                node.AccessSecret = Guid.NewGuid().ToString("n");
                node.Status = 2;
                node.LastUpdateTime = DateTime.Now;
                if (isCreate) db.ServerNodes.Add(node);
                db.SaveChanges();
            }
        }

        public static void Shutdown()
        {
            using (var scope = new Core.ScopeDbContext())
            {
                var db = scope.GetDbContext();
                var node = db.ServerNodes.FirstOrDefault(x => x.NodeName == ConfigurationCache.NodeSetting.IdentityName);
                if (node != null)
                {
                    node.Status = 0;
                    db.SaveChanges();
                }
            }
        }
    }
}
