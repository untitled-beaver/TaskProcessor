using Microsoft.EntityFrameworkCore;

namespace TaskProcessor.DbContexts
{
    public sealed class ProcessingContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }

        public ProcessingContext(DbContextOptions<ProcessingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresEnum<Job.JobStatus>();
        }
    }
}
