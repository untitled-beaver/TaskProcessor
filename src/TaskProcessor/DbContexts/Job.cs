using NetEscapades.EnumGenerators;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProcessor.DbContexts
{
    [Table("tasks")]
    public sealed class Job
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("time")]
        public DateTime Time { get; set; }

        [Column("status")]
        public JobStatus Status { get; set; }

        // Use source generated version of ToString for enums
        [EnumExtensions]
        public enum JobStatus
        {
            [Display(Name = "unknown")]
            Unknown = 0,
            [Display(Name = "created")]
            Created,
            [Display(Name = "running")]
            Running,
            [Display(Name = "finished")]
            Finished
        }
    }
}