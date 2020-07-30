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
        public string DbProvider { get; private set; }

        public SqlContext()
        {
            DbProvider = ConfigurationHelper.Config["ConnectionStrings:provider"] ?? "mysql";
        }

        public void InitTables()
        {
            this.Database.EnsureCreated();

            //this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conn = ConfigurationHelper.Config["ConnectionStrings:conn"];
            switch (DbProvider)
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
                    DbProvider = "mysql";
                    optionsBuilder.UseMySql(conn);
                    break;
            }
        }
    }
}
