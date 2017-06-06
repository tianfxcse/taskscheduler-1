using ControlExpert.ExpertiseCheck.TaskSchedulerService.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Timers;
using System.Collections.Generic;
using System.IO;
using System.Security;
namespace ControlExpert.ExpertiseCheck.TaskSchedulerService
{
    public partial class TaskSchedulerService : ServiceBase
    {
        private static List<AppStartConfigElement> appsToStart;
        public static List<AppStartConfigElement> AppsToStart
        {
            get
            {
                if (appsToStart == null || appsToStart.Count() <= 0)
                {
                    appsToStart = new List<AppStartConfigElement>();

                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    AppStartSection appStartSection = (AppStartSection)config.Sections["AppStartSection"];
                    foreach (var app in appStartSection.Apps)
                    {
                        appsToStart.Add((AppStartConfigElement)app);
                    }
                }

                return appsToStart;
            }
        }

        private Timer Worker { get; set; }

        public TaskSchedulerService()
        {
            InitializeComponent();
            Validate();
            Worker = new Timer(15 * 1000);
        }

        private void Validate()
        {
            AppsToStart.ForEach(timeEvent => 
            {
                if (timeEvent.Time < TimeSpan.Zero || timeEvent.Time > new TimeSpan(27, 23, 59, 59))
                {
                    LogingService.Write.Error(string.Format("Invalid configuration of Time for TimeEvent.Name={0}", timeEvent.Name));
                    throw new Exception(string.Format("Invalid configuration of Time for TimeEvent.Name={0}", timeEvent.Name));
                }

                if (timeEvent.Time.Days <= 0)
                {
                    // Daily
                    if (string.IsNullOrWhiteSpace(timeEvent.DayOfWeek))
                    {
                        timeEvent.NextRun = DateTime.Today.Add(timeEvent.Time);
                        if (timeEvent.NextRun < DateTime.Now)
                        {
                            timeEvent.NextRun = timeEvent.NextRun.AddDays(1);
                        }
                    }
                    // Weekly
                    else
                    {
                        var dayofweekscheduled = int.Parse(timeEvent.DayOfWeek);
                        var today = DateTime.Today;
                        var todayofweek = (int)today.DayOfWeek;
                        timeEvent.NextRun = today.Add(timeEvent.Time).AddDays(dayofweekscheduled - todayofweek);
                        if (timeEvent.NextRun < DateTime.Now)
                        {
                            timeEvent.NextRun = timeEvent.NextRun.AddDays(7);
                        }
                    }
                }
                // Monthly
                else
                {
                    var today = DateTime.Today;
                    var dt = new DateTime(today.Year, today.Month, 1);
                    timeEvent.NextRun = dt.Add(timeEvent.Time).AddDays(-1);
                    if (timeEvent.NextRun < DateTime.Now)
                    {
                        timeEvent.NextRun = timeEvent.NextRun.AddMonths(1);
                    }
                }
            });
        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.WriteLine("Press <Enter> to debug onStop().");
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            Worker.Elapsed += T1_Elapsed;
            Worker.Start();
        }

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var appItem in TaskSchedulerService.AppsToStart)
            {
                try
                {
                    var now = DateTime.Now;
                    if (appItem.NextRun < now)
                    {
                        Work(appItem);
                        appItem.LastRun = now;
                        while (appItem.NextRun < DateTime.Now)
                        {
                            if (appItem.Time.Days <= 0)
                                // Daily
                                if (string.IsNullOrWhiteSpace(appItem.DayOfWeek))
                                    appItem.NextRun = appItem.NextRun.AddDays(1);
                                // Weekly
                                else
                                    appItem.NextRun = appItem.NextRun.AddDays(7); 
                            // Monthly
                            else
                                appItem.NextRun = appItem.NextRun.AddMonths(1); 
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogingService.Write.Error(ex.Message + ex.StackTrace);
                    continue;
                }
            }
        }

        private void Work(AppStartConfigElement appItem)
        {
            StartProcess(appItem);

            //string processName = Path.GetFileName(appItem.Path);
            //bool isProcessStarted = IsProcessStarted(processName, appItem.Arg);

            //if (!isProcessStarted)
            //{
            //    StartProcess(appItem);
            //}
        }

        private void StartProcess(AppStartConfigElement appItem)
        {
            string workingDirectory = Path.GetDirectoryName(appItem.Path);
            string processName = Path.GetFileName(appItem.Path);

            LogingService.Write.ErrorFormat("Try to start: {0} {1}", processName, appItem.Arg);
            ProcessStartInfo psi = new ProcessStartInfo(appItem.Path, appItem.Arg);
            psi.WorkingDirectory = workingDirectory;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.ErrorDialog = true;
            psi.UseShellExecute = false;

            if (!string.IsNullOrWhiteSpace(appItem.UserName) &&
                !string.IsNullOrWhiteSpace(appItem.Password))
            {
                psi.UserName = appItem.UserName;
                psi.Password = appItem.Password.ToSecureString();
            }

            Process.Start(psi);
        }

        //private List<string> GetProcessByArg(string processName, string arg)
        //{
        //    string wmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", processName);
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
        //    ManagementObjectCollection retObjectCollection = searcher.Get();

        //    List<string> commandlines = new List<string>();

        //    foreach (ManagementObject retObject in retObjectCollection)
        //    {
        //        if (retObject["CommandLine"] != null)
        //            commandlines.Add(retObject["CommandLine"].ToString());
        //    }

        //    return commandlines;
        //}

        //private bool IsProcessStarted(string processName, string arg)
        //{
        //    var items = GetProcessByArg(processName, arg);
        //    return items.Any(i => i.Contains(arg));
        //}

        protected override void OnStop()
        {
            Worker.Stop();
        }
    }
}
