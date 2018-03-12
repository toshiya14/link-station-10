using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using RMEGo.Sunflower.LinkStation10.Common;

namespace RMEGo.Sunflower.LinkStation10
{
    public class AppConfig
    {

        /// <summary>
        /// The Genernal Section.
        /// </summary>
        public GeneralConfig Genernal { get; set; }

        /// <summary>
        /// The Float Section.
        /// </summary>
        public FloatConfig Float { get; set; }

        /// <summary>
        /// The LaunchBar Section.
        /// </summary>
        public LaunchbarConfig LaunchBar { get; set; }

        /// <summary>
        /// The Theme Section.
        /// </summary>
        public ThemeConfig Theme { get; set; }

        /// <summary>
        /// The base size of the icon in the menu.
        /// </summary>
        public int BaseMenuIconSize = 24;

        /// <summary>
        /// Show the head icon in the menu.
        /// </summary>
        public bool ShowHeadIconInMenu = true;

        /// <summary>
        /// Show the head title in the menu.
        /// </summary>
        public bool ShowHeadTitleInMenu = true;

        /// <summary>
        /// If show the head icon, this value specified the size of the icon.
        /// </summary>
        public int HeadIconSize = 32;

        /// <summary>
        /// Get the sections from `app.config`.
        /// </summary>
        /// <param name="key">the key in the `app.config`.</param>
        /// <returns>the value that the key indicated in the `app.config`.</returns>
        public string this[string key] {
            get {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    return ConfigurationManager.AppSettings[key];
                }
                else
                {
                    return null;
                }
            }
        }

        public AppConfig()
        {
            this.Genernal = new GeneralConfig();
            this.LaunchBar = new LaunchbarConfig();
            this.Theme = new ThemeConfig();
            this.Float = new FloatConfig();
        }

        public static AppConfig GetSettingsFromFile(params string[] filepath)
        {
            var settings = JObject.FromObject(new AppConfig());
            foreach (var f in filepath)
            {
                try
                {
                    if (File.Exists(f))
                    {
                        var attr = File.GetAttributes(f);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            var di = new DirectoryInfo(f);
                            var files = from file in di.GetFiles("*" + App.CfgFileExt) select file.FullName;
                            var jobj = JObject.FromObject(GetSettingsFromFile(files.ToArray()));
                            settings.Merge(jobj);
                        }
                        else
                        {
                            var cnt = File.ReadAllText(f);
                            var cntjobj = JObject.Parse(cnt);
                            settings.Merge(cntjobj);
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("[ConfigLoader] Could not load settings from `" + f + "`. ");
                }
            }
            return settings.ToObject<AppConfig>();
        }
    }

    [ConfigGroup(Name = "General", Description = "General Settings.")]
    public class GeneralConfig
    {
        [ConfigField(Type = "double", Description = "The UI elements would be zoomed with this rate. To get a better performance in the Hi-DPI screen.")]
        public double ZoomRate { get; set; }

        [ConfigField(Description = "General font would be used.")]
        public string FontFamily { get; set; }

        public FontSection FontFamilyStruct { get => FontSection.Parse(FontFamily); }

        public GeneralConfig()
        {
            ZoomRate = 1.0;
            FontFamily = new FontSection().ToString();
        }
    }

    [ConfigGroup(Name = "Float", Description = "Settings used for floating mode.")]
    public class FloatConfig
    {
        /// <summary>
        /// Set this to true if you would like to make the launch bar float in the side of the screen.
        /// When it is collpsed, you could move your mouse onto the indicator to expend the launch bar.
        /// </summary>
        public bool FloatSide { get; set; }

        /// <summary>
        /// If the `FloatSide` set to true, this value is how long would you need to wait after you move your
        /// mouse onto the indicator to expend the launch bar. (Unit: ms)
        /// </summary>
        public int ExpendTimeout { get; set; }

        public FloatConfig()
        {
            FloatSide = false;
            ExpendTimeout = 1000;
        }
    }

    [ConfigGroup(Name = "Launchbar", Description = "Settings used for the main panel.")]
    public class LaunchbarConfig
    {
        /// <summary>
        /// The base size of the icon in the launch bar.
        /// </summary>
        public int BaseIconSize { get; set; }

        /// <summary>
        /// When the mouse over the tile, show its title.
        /// </summary>
        public bool ShowTitleHover { get; set; }

        /// <summary>
        /// Set to true if you want to active the menu when the mouse over the tile but not click it.
        /// </summary>
        public bool ActiveWhenHover { get; set; }

        public LaunchbarConfig()
        {
            BaseIconSize = 48;
            ShowTitleHover = true;
            ActiveWhenHover = true;
        }
    }

    [ConfigGroup(Name = "Theme", Description = "Settings for custom colors and styles.")]
    public class ThemeConfig
    {
        /// <summary>
        /// The width of the side indicator.
        /// </summary>
        public double SideIndicatorWidth { get; set; }

        /// <summary>
        /// The color of the side indicator.
        /// </summary>
        public string SideIndicatorColor { get; set; }

        /// <summary>
        /// The style when mouse over the tile.
        /// </summary>
        public HoverStyleEnum HoverStyle { get; set; }

        /// <summary>
        /// The color when mouse over the tile.
        /// </summary>
        public string HoverColor { get; set; }

        /// <summary>
        /// The background color of the tile.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Show separators between tiles.
        /// </summary>
        public bool ShowSeparator { get; set; }

        /// <summary>
        /// The color of the separator.
        /// </summary>
        public string SeparatorColor { get; set; }

        /// <summary>
        /// The width of the separator.
        /// </summary>
        public int SeparatorWidth { get; set; }

        /// <summary>
        /// The size of the separator.(0-100, unit:%)
        /// </summary>
        public int SeparatorHeight { get; set; }

        /// <summary>
        /// The color of the active tile.
        /// </summary>
        public string ActiveColor { get; set; }

        /// <summary>
        /// Show the separator in the menu.
        /// </summary>
        public bool ShowSeparatorInMenu { get; set; }

        /// <summary>
        /// The icon pack location.
        /// </summary>
        public string IconPackLocation { get; set; }

        public ThemeConfig()
        {
            ActiveColor = "#2e4874";
            BackgroundColor = "#343635";
            HoverColor = "#484a49";
            HoverStyle = HoverStyleEnum.UnderLine;
            SeparatorColor = "#c2c2c2";
            SeparatorHeight = 80;
            SeparatorWidth = 1;
            ShowSeparator = false;
            ShowSeparatorInMenu = true;
            SideIndicatorColor = "#2e4874";
            SideIndicatorWidth = 10;
            IconPackLocation = "{$app}/themes/iconpack/default";
        }

        public Brush SideIndicatorBrush {
            get {
                return new SolidColorBrush(ColorStringHelper.Parse(SideIndicatorColor));
            }
        }

        public Brush BackgroundBrush {
            get {
                return new SolidColorBrush(ColorStringHelper.Parse(BackgroundColor));
            }
        }
    }

    public class FontSection
    {
        /// <summary>
        /// The family name of the font.
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// The size of the font, (Unit: pt).
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Set true the font would be italic.
        /// </summary>
        public bool isItalic { get; set; }

        /// <summary>
        /// Set ture the font would be bold.
        /// </summary>
        public bool isBold { get; set; }

        public FontSection()
        {
            FamilyName = "Microsoft YaHei UI";
            Size = 9;
            isItalic = false;
            isBold = false;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(FamilyName);
            sb.Append(", ");
            sb.Append(Size.ToString("0.#"));
            if (isItalic) {
                sb.Append(", ");
                sb.Append("Italic");
            }
            if (isBold)
            {
                sb.Append(", ");
                sb.Append("Bold");
            }
            return sb.ToString();
        }

        public static FontSection Parse(string config) {
            var parts = config.Split(new[] { ',' },3);
            var result = new FontSection();

            result.FamilyName = parts[0];
            if (parts.Length > 1)
            {
                result.Size = Convert.ToDouble(parts[1]);
            }
            if (parts.Length > 2) {
                var other = parts[2];
                if (other.Contains("Italic")) {
                    result.isItalic = true;
                }
                if (other.Contains("Bold")) {
                    result.isBold = true;
                }
            }
            return result;
        }
    }

    public enum HoverStyleEnum { Block, UnderLine }
}
