using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PrometheusBot.Model.Settings
{
    public struct SettingLookupInfo
    {
        public string SettingName { get; set; }
        public ulong? UId { get; set; }
        public ulong? GId { get; set; }
        public ulong? CId { get; set; }

        public string Ids
        {
            get
            {
                ulong?[] arr = { UId, GId, CId};
                string json = JsonConvert.SerializeObject(arr);
                return json;
            }
        }

        public SettingLookupInfo(string settingName)
        {
           SettingName = settingName;

            UId = GId = CId = null;
        }
    }
}
