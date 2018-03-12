using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public class ConfigFile
    {
        private static readonly Regex reSectionStart = new Regex(@"^\[(?<section>[^\[\]]+)\]$");
        private static readonly Regex reKeyValue = new Regex(@"^(?<key>[^=\s]+)\s*=\s*(?<value>.+)$");
        private static readonly Regex reComments = new Regex(@"^[#;].*$");
        Dictionary<string, Dictionary<string, string>> config;
        public void Load(string input)
        {
            string currentSection = string.Empty;
            if (config == null)
            {
                config = new Dictionary<string, Dictionary<string, string>>();
            }
            foreach (var line in input.Split('\r', '\n'))
            {
                var e = line.Trim();
                if (string.IsNullOrWhiteSpace(e))
                {
                    continue;
                }
                // Match Section
                var m = reSectionStart.Match(e);
                if (m.Success)
                {
                    if (m.Groups["section"] != null)
                    {
                        currentSection = m.Groups["section"].Value;
                        if (!config.ContainsKey(currentSection))
                        {
                            config[currentSection] = new Dictionary<string, string>();
                        }
                        continue;
                    }
                    throw new FormatException("This is not a correct section defination: " + e);
                }
                // Match Key-Value
                m = reKeyValue.Match(e);
                if (m.Success)
                {
                    if (m.Groups["key"] != null && m.Groups["value"] != null)
                    {
                        var key = m.Groups["key"].Value;
                        var value = m.Groups["value"].Value;
                        config[currentSection][key] = value;
                        continue;
                    }
                    throw new FormatException("This is not a valid key-value pair: " + e);
                }
                // Match Comment
                m = reComments.Match(e);
                if (m.Success)
                {
                    continue;
                    throw new FormatException("This is not a valid comments: " + e);
                }
                // Other
                throw new FormatException("Could not parse this line: " + e);
            }
        }
        public void ApplyToObject<T>(T obj, string section = null)
        {
            var type = obj.GetType();
            if (string.IsNullOrWhiteSpace(section))
            {
                if (!type.IsDefined(typeof(ConfigGroupAttribute), true))
                {
                    return;
                }
                var groupattr = type.GetCustomAttributes(typeof(ConfigGroupAttribute), true).First();
                var gName = groupattr.GetType()
                                     .GetField("Name", BindingFlags.Instance | BindingFlags.Public)
                                     .GetValue(groupattr).ToString();
                if (string.IsNullOrWhiteSpace(gName))
                {
                    var cName = type.Name;
                    section = cName;
                }
                else
                {
                    section = gName;
                }
            }
            var dict = config[section];
            // Set Fields
            foreach (var fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fi.IsDefined(typeof(ConfigFieldAttribute), true))
                {
                    var newValue = FindNewValue(dict, fi);
                    if (newValue == null)
                    {
                        continue;
                    }
                    fi.SetValue(obj, newValue);
                }
            }
            // Set Properties
            foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                if (pi.IsDefined(typeof(ConfigFieldAttribute), true))
                {
                    var newValue = FindNewValue(dict, pi);
                    if (newValue == null)
                    {
                        continue;
                    }
                    pi.SetValue(obj, newValue);
                }
            }
        }
        public static string StringifySectionConfigObject<T>(T obj)
        {
            var type = obj.GetType();
            var sb = new StringBuilder();
            if (!type.IsDefined(typeof(ConfigGroupAttribute), true))
            {
                return string.Empty;
            }
            var gattr = GetGroupAttr(type);
            if (!string.IsNullOrWhiteSpace(gattr.Description))
            {
                sb.AppendLine("; " + gattr.Description);
            }
            sb.AppendLine("[" + gattr.Name + "]");
            sb.AppendLine();
            foreach (var fi in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fi.IsDefined(typeof(ConfigFieldAttribute), true))
                {
                    var attr = GetFieldAttr(fi);
                    var value = fi.GetValue(obj);
                    if (!string.IsNullOrWhiteSpace(attr.Description))
                    {
                        sb.AppendLine("; " + attr.Description);
                    }
                    sb.AppendLine(attr.Name + "=" + value);
                    sb.AppendLine();
                }
            }
            foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
            {
                if (pi.IsDefined(typeof(ConfigFieldAttribute), true))
                {
                    var attr = GetFieldAttr(pi);
                    var value = pi.GetValue(obj);
                    if (!string.IsNullOrWhiteSpace(attr.Description))
                    {
                        sb.AppendLine("; " + attr.Description);
                    }
                    sb.AppendLine(attr.Name + "=" + value);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        private static object FindNewValue(Dictionary<string, string> dict, MemberInfo member)
        {
            var fieldattr = member.GetCustomAttribute<ConfigFieldAttribute>();
            var fName = fieldattr.GetType()
                                 .GetField("Name", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            var fDesc = fieldattr.GetType()
                                 .GetField("Description", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            var fType = fieldattr.GetType()
                                 .GetField("Type", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            if (string.IsNullOrWhiteSpace(fName))
            {
                fName = member.Name;
            }
            if (string.IsNullOrWhiteSpace(fType))
            {
                fType = "string";
            }
            if (dict.ContainsKey(fName))
            {
                var value = dict[fName];
                object cvtvalue;
                if (fType.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    cvtvalue = value;
                }
                else if (fType.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    cvtvalue = Convert.ToInt32(value);
                }
                else if (fType.Equals("long", StringComparison.OrdinalIgnoreCase))
                {
                    cvtvalue = Convert.ToInt64(value);
                }
                else if (fType.Equals("double", StringComparison.OrdinalIgnoreCase))
                {
                    cvtvalue = Convert.ToDouble(value);
                }
                else if (fType.Equals("bool", StringComparison.OrdinalIgnoreCase))
                {
                    cvtvalue = Convert.ToBoolean(value);
                }
                else
                {
                    cvtvalue = value;
                }
                return cvtvalue;
            }
            return null;
        }
        private static ConfigFieldAttribute GetFieldAttr(MemberInfo member)
        {
            var fieldattr = member.GetCustomAttribute<ConfigFieldAttribute>();
            var fName = fieldattr.GetType()
                                 .GetField("Name", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            var fDesc = fieldattr.GetType()
                                 .GetField("Description", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            var fType = fieldattr.GetType()
                                 .GetField("Type", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr)?.ToString();
            var result = new ConfigFieldAttribute();
            result.Description = fDesc;
            if (string.IsNullOrWhiteSpace(fName))
            {
                result.Name = member.Name;
            }
            else
            {
                result.Name = fName;
            }
            if (string.IsNullOrWhiteSpace(fType))
            {
                result.Type = "string";
            }
            else
            {
                result.Type = fType;
            }
            return result;
        }

        private static ConfigGroupAttribute GetGroupAttr(Type type)
        {
            var fieldattr = type.GetCustomAttribute<ConfigGroupAttribute>();
            var fName = fieldattr.GetType()
                                 .GetField("Name", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr).ToString();
            var fDesc = fieldattr.GetType()
                                 .GetField("Description", BindingFlags.Instance | BindingFlags.Public)
                                 .GetValue(fieldattr).ToString();
            var result = new ConfigGroupAttribute();
            result.Description = fDesc;
            if (string.IsNullOrWhiteSpace(fName))
            {
                result.Name = type.Name;
            }
            else
            {
                result.Name = fName;
            }
            return result;
        }
    }

    public class ConfigGroupAttribute : Attribute
    {
        public string Name;
        public string Description;
    }

    public class ConfigFieldAttribute : Attribute
    {
        public string Name;
        public string Type;
        public string Description;
    }
}
