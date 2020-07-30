using Hos.ScheduleMaster.Core.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    public interface ISqlContext
    {
        void InitTables();
    }

    public class SqlContext : DbContext, ISqlContext
    {
        public void InitTables()
        {
            this.Database.EnsureCreated();

            //this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbProvider = ConfigurationHelper.Config["ConnectionStrings:provider"];
            var conn = ConfigurationHelper.Config["ConnectionStrings:conn"];
            switch (dbProvider)
            {
                case "sqlserver":
                    optionsBuilder.UseSqlServer(conn);
                    break;
                case "mysql":
                    optionsBuilder.UseMySql(conn);
                    break;
                case "npgsql":
                    optionsBuilder.UseNpgsql(conn);
                    break;
                default:
                    optionsBuilder.UseMySql(conn);
                    break;
            }
        }
    }
}
