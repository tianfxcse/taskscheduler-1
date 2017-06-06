using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlExpert.ExpertiseCheck.TaskSchedulerService.Configuration
{
    /// <summary>
    /// Task scheduler
    /// </summary>
    public class AppStartSection : ConfigurationSection
    {
        [ConfigurationProperty("apps", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(AppStartCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public AppStartCollection Apps
        {
            get
            {
                AppStartCollection appStartCollection = (AppStartCollection)base["apps"];
                return appStartCollection;
            }

            set
            {
                AppStartCollection appStartCollection = value;
            }
        }

        public AppStartSection()
        {
            AppStartConfigElement app = new AppStartConfigElement();
            Apps.Add(app);
        }
    }
}
