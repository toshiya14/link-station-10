using Newtonsoft.Json;
using RMEGo.Localization;
using RMEGo.Sunflower.LinkStation10.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RMEGo.Sunflower.LinkStation10
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        internal static Launch LaunchWindow { get; private set; }
        internal static string AppRoot = AppDomain.CurrentDomain.BaseDirectory;
        internal static string CfgFileExt = ".cfg";
        internal static AppConfig Config;
        internal static Locale Locale;
        protected override void OnStartup(StartupEventArgs e)
        {
            // Check Environment
            #region CHECK ENVIRONMENT
            CheckOrCreateDirectory(AppRoot, "config");
            CheckOrCreateDirectory(AppRoot, "lang");
            var config = new AppConfig();
            CheckOrCreateFile(JsonConvert.SerializeObject(config.Genernal, Formatting.Indented), AppRoot, "config/genernal", CfgFileExt);
            CheckOrCreateFile(JsonConvert.SerializeObject(config.Float, Formatting.Indented), AppRoot, "config/float", CfgFileExt);
            CheckOrCreateFile(JsonConvert.SerializeObject(config.Theme, Formatting.Indented), AppRoot, "config/theme", CfgFileExt);
            CheckOrCreateFile(JsonConvert.SerializeObject(config.LaunchBar, Formatting.Indented), AppRoot, "config/launchbar", CfgFileExt);
            var locale = new Locale();
            CheckOrCreateFile(JsonConvert.SerializeObject(locale, Formatting.Indented), AppRoot, "lang/default.locale");

            Config = AppConfig.GetSettingsFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"));
            Locale = Locale.GetLocale("default");
            #endregion

            bool createdNew;
            mutex = new Mutex(true, @"RMEGo.Sunflower.LinkStation10", out createdNew);
            if (!createdNew)
            {
                mutex = null;
                NamedPipeHelper.SendMessage("$$ACTIVE$$");
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
                mutex.ReleaseMutex();
            base.OnExit(e);
        }

        public static bool CheckOrCreateDirectory(params string[] pathlist) {
            try
            {
                var path = Path.Combine(pathlist);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public static bool CheckOrCreateFile(byte[] filecontent, params string[] pathlist) {
            try
            {
                var path = Path.Combine(pathlist);
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, filecontent);
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckOrCreateFile(string filecontent, params string[] pathlist) {
            try
            {
                var path = Path.Combine(pathlist);
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, filecontent);
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
