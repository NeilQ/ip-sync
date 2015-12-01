using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Ipsync.Controller
{
    public class AutoStartup
    {
        public static bool Set(bool enabled)
        {
            try
            {
                string path = Application.ExecutablePath;
                RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (enabled)
                {
                    runKey.SetValue(Constants.APP_NAME, path);
                }
                else
                {
                    runKey.DeleteValue(Constants.APP_NAME);
                }
                runKey.Close();
                return true;
            }
            catch (Exception e)
            {
                Logging.LogUsefulException(e);
                return false;
            }
        }

        public static bool Check()
        {
            try
            {
                RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                string[] runList = runKey.GetValueNames();
                runKey.Close();
                foreach (string item in runList)
                {
                    if (item.Equals(Constants.APP_NAME))
                        return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logging.LogUsefulException(e);
                return false;
            }
        }
    }
}