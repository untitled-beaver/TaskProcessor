
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TaskProcessor
{
    public class Program
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AssemblyName assemblyName = Assembly.GetEntryAssembly().GetName();
            m_logger.Info($"Starting {assemblyName.Name} {assemblyName.Version}");

            try
            {
                await MainWrapped(args);
            }
            catch (Exception ex)
            {
                m_logger.Fatal(ex, "Unhandled exception!");
            }

            m_logger.Info("Shutting down");
        }

        public static async Task MainWrapped(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            builder.Host.UseNLog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            m_logger.Fatal(e.Exception, "Got UnobservedTaskException. Shutting down!");

            LogManager.Flush();
            Environment.Exit(1);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                m_logger.Fatal(exception, "Got UnhandledException. Shutting down!");
            }
            else
            {
                m_logger.Fatal($"Got UnhandledException. Shutting down!");
            }

            LogManager.Flush();
            Environment.Exit(1);
        }
    }
}
