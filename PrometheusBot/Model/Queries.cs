using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusBot.Model
{
    static class Queries
    {
        public const string getSetting =
            @"select value from settings
              where setting_name = @SettingName
                    and ids = @Ids;";

        public const string updateSetting =
            @"insert into settings (setting_name, ids, value) Values(@SettingName, @Ids, @Value)
            on duplicate key update value=@Value";

        public const string removeSetting =
            @"delete from settigns
            where setting_name = @SettingName
            and Ids = @Ids";
    }
}
