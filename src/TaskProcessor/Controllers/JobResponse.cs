using System;
using System.Text.Json.Serialization;
using TaskProcessor.DbContexts;

namespace TaskProcessor.Controllers
{
    public class JobResponse
    {
        [JsonPropertyName("job")]
        public JobContract Job { get; set; }

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; }

        public class JobContract
        {
            public Guid Id { get; set; }
            public string Status { get; set; }
            public DateTime Time { get; set; }
        }

        public static JobResponse FromError(string errorMessage)
        {
            return new JobResponse()
            {
                ErrorMessage = errorMessage,
            };
        }

        public static JobResponse MapFromJob(Job job)
        {
            return new JobResponse()
            {
                Job = new JobContract()
                {
                    Id = job.Id,
                    Status = job.Status.ToStringFast(),
                    Time = job.Time,
                }
            };
        }
    }
}
