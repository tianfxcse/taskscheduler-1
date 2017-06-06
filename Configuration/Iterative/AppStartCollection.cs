using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlExpert.ExpertiseCheck.TaskSchedulerService.Configuration
{
    public class AppStartCollection : ConfigurationElementCollection
    {
        public AppStartCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AppStartConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var item = (AppStartConfigElement)element;
            return item.Name;
        }

        public AppStartConfigElement this[int index]
        {
            get
            {
                return (AppStartConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public AppStartConfigElement this[string Name]
        {
            get
            {
                return (AppStartConfigElement)BaseGet(Name);
            }
        }

        public int IndexOf(AppStartConfigElement app)
        {
            return BaseIndexOf(app);
        }

        public void Add(AppStartConfigElement app)
        {
            BaseAdd(app);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(AppStartConfigElement app)
        {
            if (BaseIndexOf(app) >= 0)
            {
                BaseRemove(app.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
