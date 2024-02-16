using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TaskProcessor.DbContexts;
using TaskProcessor.Services;

namespace TaskProcessor.Controllers
{
    [Route("task")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly Logger m_logger = LogManager.GetCurrentClassLogger();
        private readonly ProcessingContext m_processingContext;
        private readonly TaskProcessorService m_taskProcessorService;

        public JobsController(ProcessingContext processingContext, TaskProcessorService taskProcessorService)
        {
            m_processingContext = processingContext;
            m_taskProcessorService = taskProcessorService;
        }

        // GET task/{id}
        // e.g. task/8ba5e07b-9a89-47b0-b3f0-a65c33bbe0ec
        [HttpGet("{id?}")]
        public async Task<IActionResult> Get([FromRoute] Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            m_logger.Info("Returning information about task {Id}", id);

            Job job = await m_processingContext.Jobs.FirstOrDefaultAsync(x => x.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(job.Status.ToStringFast());
            }
        }

        // POST api/<JobsController>
        [HttpPost("/task")]
        public async Task<IActionResult> Post()
        {
            m_logger.Info("Creating new task");

            try
            {
                Job newJob = new Job
                {
                    Id = Guid.NewGuid(),
                    Time = DateTime.UtcNow,
                    Status = Job.JobStatus.Created,
                };

                m_processingContext.Jobs.Add(newJob);
                await m_processingContext.SaveChangesAsync();

                m_logger.Info("Task with id {Id} has been created", newJob.Id);

                m_taskProcessorService.ProcessJob(newJob);

                return Accepted(newJob.Id);
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Failed to create a new task");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
