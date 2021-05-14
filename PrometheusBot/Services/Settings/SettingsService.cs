using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PrometheusBot.Model;

namespace PrometheusBot.Services.Settings
{
    public class SettingsService
    {
        private Dictionary<string, SettingModel> _settingsInfo;
        internal DataAccess Data { get; private set; }

        public SettingsService(string settingsPath, string connectionString)
        {
            string json = File.ReadAllText(settingsPath);
            _settingsInfo = JsonConvert.DeserializeObject<Dictionary<string, SettingModel>>(json);

            Data = new(connectionString);
        }

        public SettingModel GetSettingInfo(string setting)
        {
            _settingsInfo.TryGetValue(setting, out var result);
            return result ?? throw new SettingNotFoundException(setting);
        }
        public bool GetSetting<T>(SettingLookupInfo lookupInfo, out T value, bool lookForDefault = false)
        {
            if (GetSetting(lookupInfo, out var result, lookForDefault))
            {
                value = (T)result;
                return true;
            }
            value = default;
            return false;
        }
        public bool GetSetting(SettingLookupInfo lookupInfo, out object value, bool lookForDefault = false)
        {
            using var session = Data.Session;
            return GetSetting(lookupInfo, out value, session, lookForDefault);
        }
        public bool GetSetting(SettingLookupInfo lookupInfo, out object value, DataAccessSession dataSession, bool lookForDefault = false)
        {
            SettingModel settingModel = GetSettingInfo(lookupInfo.SettingName);
            string sql = Queries.getSetting;
            string stringValue = dataSession.LoadData<string, SettingLookupInfo>(sql, lookupInfo).FirstOrDefault();
            if (stringValue is null)
            {
                if (!lookForDefault)
                {
                    value = default;
                    return false;
                }
                if (lookupInfo.UId.HasValue && lookupInfo.GId.HasValue && !lookupInfo.CId.HasValue)
                {
                    lookupInfo.GId = null;
                    return GetSetting(lookupInfo, out value, lookForDefault);
                }
                if (!lookupInfo.UId.HasValue && lookupInfo.GId.HasValue && lookupInfo.CId.HasValue)
                {
                    lookupInfo.CId = null;
                    return GetSetting(lookupInfo, out value, lookForDefault);
                }
                value = settingModel.DefaultValue;
                return true;
            }
            value = settingModel.Convert(stringValue);
            return true;
        }
        public void SetSetting(SettingLookupInfo lookupInfo, object value, DataAccessSession dataSession = null)
        {
            string sql;
            SettingModel settingModel = GetSettingInfo(lookupInfo.SettingName);
            if (value is null)
            {
                sql = Queries.removeSetting;
                Data.SaveData(sql, lookupInfo);
                return;
            }
            sql = Queries.updateSetting;
            string stringValue = settingModel.Convert(value);
            var parameters = new { lookupInfo.SettingName, lookupInfo.Ids, Value = stringValue};
            Data.SaveData(sql, parameters);
        }

        public string[] GetPrefixes(ulong UserId, ulong? GuildId, ulong ChannelId)
        {
            string[] result = new string[2];
            SettingLookupInfo info = new("prefix:use-user-defined") { GId = GuildId, CId = ChannelId };
            GetSetting(info, out bool useUserDefined, true);
            info = new("prefix:natural") { GId = GuildId };
            if (useUserDefined)
                info.UId = UserId;
            else
                info.CId = ChannelId;
            GetSetting(info, out result[0], true);
            info.SettingName = "prefix:synthetic";
            GetSetting(info, out result[1], true);
            return result;
        }
    }
}
