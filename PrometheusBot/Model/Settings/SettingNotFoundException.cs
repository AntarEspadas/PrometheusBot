using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model.Settings
{

    [Serializable]
    public class SettingNotFoundException : PrometheusException
    {
        private const string msg = "Setting {0} could not be found";
        public SettingNotFoundException() { }
        public SettingNotFoundException(string settingName) : base(string.Format(msg, settingName)) { }
        public SettingNotFoundException(string settingName, Exception inner) : base(string.Format(msg, settingName), inner) { }
        protected SettingNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
