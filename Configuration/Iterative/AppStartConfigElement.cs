using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlExpert.ExpertiseCheck.TaskSchedulerService.Configuration
{
    public class AppStartConfigElement : ConfigurationElement
    {
        public AppStartConfigElement()
        {

        }

        public AppStartConfigElement(string name, string path, string arg)
        {
            this.Name = name;
            this.Path = path;
            this.Arg = arg;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("arg", IsRequired = true)]
        public string Arg
        {
            get
            {
                return (string)this["arg"];
            }
            set
            {
                this["arg"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public string UserName
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        [ConfigurationProperty("dayofweek", IsRequired = false)]
        public string DayOfWeek
        {
            get
            {
                return (string)this["dayofweek"];
            }
            set
            {
                this["dayofweek"] = value;
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    return TimeSpan.Zero;
                }

                return TimeSpan.Parse(TimeString);
            }
        }

        [ConfigurationProperty("time", DefaultValue = "00:00:00", IsRequired = true)]
        //[RegexStringValidator(@"^\d{2}:\d{2}:\d{2}$")]
        protected string TimeString
        {
            get
            {
                return (string)this["time"];
            }

            set
            {
                this["time"] = Convert.ToString(value);
            }
        }

        public DateTime? LastRun
        {
            get;
            set;
        }

        public DateTime NextRun
        {
            get;
            set;
        }
    }
}
