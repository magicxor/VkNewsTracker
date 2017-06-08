using System;
using System.IO;
using System.Reflection;
using Autofac;
using NLog;
using VkNewsTracker.Common;
using VkNewsTracker.Common.Constants;
using VkNewsTracker.Common.DependencyInjection;

namespace VkNewsTracker.Console
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                _logger.Trace("{0} has been started", Assembly.GetEntryAssembly().GetName().Name);

                var container = ContainerConfig.Configure(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Defaults.ConfigurationFileName));
                using (var scope = container.BeginLifetimeScope())
                {
                    var worker = scope.Resolve<Worker>();
                    worker.Run();
                }
                
                _logger.Trace("{0} has been stopped", Assembly.GetEntryAssembly().GetName().Name);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }
    }
}