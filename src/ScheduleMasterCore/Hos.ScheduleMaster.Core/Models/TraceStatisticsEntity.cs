using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hos.ScheduleMaster.Core.Models
{
    [Table("tracestatistics")]
    public class TraceStatisticsEntity : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("datenum")]
        public int DateNum { get; set; }


        [Column("datestamp")]
        public long DateStamp { get; set; }

        [Column("success")]
        public int Success { get; set; }

        [Column("fail")]
        public int Fail { get; set; }

        [Column("other")]
        public int Other { get; set; }

        [Column("lastupdatetime")]
        public DateTime LastUpdateTime { get; set; }
    }
}
