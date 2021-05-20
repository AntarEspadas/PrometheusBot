using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Discord;
using System.ComponentModel;

namespace PrometheusBot.Services.Settings
{
    public class SettingModel
    {
        public Type Type { get; private set; } = typeof(string);
        public object DefaultValue { get; private set; } = null;
        public ImmutableList<GuildPermission> GuildPermissions { get; private set; } = Array.Empty<GuildPermission>().ToImmutableList();
        public ImmutableList<ChannelPermission> ChannelPermissions { get; private set; } = Array.Empty<ChannelPermission>().ToImmutableList();
        public bool PermissionRole { get; private set; } = true;
        public string ValidationRegex { get; private set; } = ".+";
        public string RawErrorMessage { get; private set; } = "The value `{0}` is invalid for this setting";
        public SettingScope Scope { get; private set; } = SettingScope.Both;
        public bool Visible { get; private set; } = true;
        private readonly TypeConverter converter;

        public SettingModel(Type type, string defaultValue, IEnumerable<GuildPermission> guildPermissions, IEnumerable<ChannelPermission> channelPermissions, bool permissionRole, string validationRegex, string rawErrorMessage, SettingScope scope, bool visible)
        {
            converter = TypeDescriptor.GetConverter(type);


            Type = type;
            DefaultValue = converter.ConvertFromInvariantString(defaultValue);
            GuildPermissions = guildPermissions.ToImmutableList();
            ChannelPermissions = channelPermissions.ToImmutableList();
            PermissionRole = permissionRole;
            ValidationRegex = validationRegex;
            RawErrorMessage = rawErrorMessage;
            Scope = scope;
            Visible = visible;
        }

        public bool IsValid(string value)
        {
            return Regex.IsMatch(value, ValidationRegex, RegexOptions.None, TimeSpan.FromSeconds(5));
        }

        public string GetErrorMessage(string value)
        {
            return string.Format(RawErrorMessage, value);
        }

        public object Convert(string value)
        {
            object convertedValue = converter.ConvertFromInvariantString(value);
            return convertedValue ?? throw new NotSupportedException();
        }
        public string Convert(object value)
        {
            string convertedValue = converter.ConvertToInvariantString(value);
            return convertedValue;
        }
    }
}
