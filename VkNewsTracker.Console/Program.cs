﻿using System;
using System.IO;
using System.Reflection;
using Autofac;
using VkNewsTracker.Common;
using VkNewsTracker.Common.Constants;
using VkNewsTracker.Common.DependencyInjection;

namespace VkNewsTracker.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var container = ContainerConfig.Configure(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Defaults.ConfigurationFileName));
                using (var scope = container.BeginLifetimeScope())
                {
                    var worker = scope.Resolve<Worker>();
                    worker.Run();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }
    }
}