using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TaskProcessor.DbContexts;

namespace TaskProcessor.Services
{
    public sealed class TaskProcessorService
    {
        private const int DELAY_SECONDS = 60 * 2;

        private readonly Logger m_logger = LogManager.GetCurrentClassLogger();
        private readonly ActionBlock<Job> m_processJob;
        private readonly IServiceScopeFactory m_scopeFactory;
        private readonly TimeSpan m_delay = TimeSpan.FromSeconds(DELAY_SECONDS);

        public TaskProcessorService(IServiceScopeFactory scopeFactory)
        {
            // For simple example we use TransformBlock here but in real world scenario we should limit concurrency by using ExecutionDataflowBlockOptions
            m_processJob = new ActionBlock<Job>(TransformJob);
            m_scopeFactory = scopeFactory;
        }

        public bool ProcessJob(Job job)
        {
            return m_processJob.Post(job);
        }

        private async Task TransformJob(Job job)
        {
            m_logger.Trace("Starting to process job {Id}", job.Id);

            using (IServiceScope scope = m_scopeFactory.CreateScope())
            {
                ProcessingContext processingContext = scope.ServiceProvider.GetRequiredService<ProcessingContext>();

                job.Status = Job.JobStatus.Running;
                job.Time = DateTime.UtcNow;
                processingContext.Update(job);
                await processingContext.SaveChangesAsync();

                await Task.Delay(m_delay);

                m_logger.Trace("Job {Id} processed", job.Id);

                job.Status = Job.JobStatus.Finished;
                job.Time = DateTime.UtcNow;
                processingContext.Update(job);
                await processingContext.SaveChangesAsync();
            }
        }
    }
}
