using ControlExpert.ExpertiseCheck.TaskSchedulerService.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ControlExpert.ExpertiseCheck.TaskSchedulerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                TaskSchedulerService autoStartService = new TaskSchedulerService();
                autoStartService.TestStartupAndStop(args);

                Console.WriteLine("Press <Enter> to end.");
                Console.ReadLine();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new TaskSchedulerService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
