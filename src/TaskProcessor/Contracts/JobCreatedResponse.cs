using System;
using System.Text.Json.Serialization;
using TaskProcessor.DbContexts;

namespace TaskProcessor.Contracts
{
    public sealed class JobCreatedResponse
    {
        [JsonPropertyName("job_id")]
        public Guid JobId { get; set; }

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; }

        public static JobCreatedResponse FromJob(Job job)
        {
            return new JobCreatedResponse
            {
                JobId = job.Id,
            };
        }

        public static JobCreatedResponse FromErrorMessage(string errorMessage)
        {
            return new JobCreatedResponse
            {
                ErrorMessage = errorMessage,
            };
        }
    }
}
