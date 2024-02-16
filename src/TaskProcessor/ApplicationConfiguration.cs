using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace TaskProcessor
{
    public sealed class ApplicationConfiguration
    {
        [ConfigurationKeyName("DbConnectionSettings")]
        public DbConnectionSettings DbSettings { get; set; }

        public string DbConnectionString { get; set; }

        private ApplicationConfiguration()
        {
        }

        private void ProcessRuntimeConfiguration()
        {
            if (DbSettings == null)
            {
                throw new ApplicationException("Set db configuration via appsettings.json or environment variables");
            }

            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();

            NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();
            npgsqlConnectionStringBuilder.ApplicationName = assemblyName.Name;
            npgsqlConnectionStringBuilder.Database = DbSettings.Database;
            npgsqlConnectionStringBuilder.Username = DbSettings.Username;
            npgsqlConnectionStringBuilder.Password = DbSettings.Password;
            npgsqlConnectionStringBuilder.Host = DbSettings.Hostname;
            npgsqlConnectionStringBuilder.Port = DbSettings.Port;
            npgsqlConnectionStringBuilder.Pooling = true;

            DbConnectionString = npgsqlConnectionStringBuilder.ConnectionString;
        }

        public static ApplicationConfiguration Load(IWebHostEnvironment environment)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables("TASK_PROCESSOR_");

            if (environment.IsDevelopment())
            {
                configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false);
            }
            else if (environment.IsProduction())
            {
                configurationBuilder.AddJsonFile("appsettings.Release.json", optional: true, reloadOnChange: false);
            }
            else
            {
                throw new ApplicationException("Unsupported environment");
            }

            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
            configurationRoot.Bind(applicationConfiguration);

            applicationConfiguration.ProcessRuntimeConfiguration();

            return applicationConfiguration;
        }

        public sealed class DbConnectionSettings
        {
            [ConfigurationKeyName("Hostname")]
            public string Hostname { get; set; }
            
            [ConfigurationKeyName("Port")]
            public int Port { get; set; }

            [ConfigurationKeyName("Database")]
            public string Database { get; set; }

            [ConfigurationKeyName("Username")]
            public string Username { get; set; }

            [ConfigurationKeyName("Password")]
            public string Password { get; set; }
        }
    }
}
