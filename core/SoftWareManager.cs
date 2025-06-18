using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    internal class SoftWareManager
    {
        /// <summary>
        /// 检查某个软件是否已经安装了
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>

        public static bool IsSoftwareInstalled(string displayName)
        {
            // 检查64位软件
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            string name = subkey?.GetValue("DisplayName") as string;
                            if (name != null && name.IndexOf(displayName, StringComparison.OrdinalIgnoreCase) >= 0)
                                return true;
                        }
                    }
                }
            }

            //// 检查32位软件
            //registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            //using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
            //{
            //    if (key != null)
            //    {
            //        foreach (string subkeyName in key.GetSubKeyNames())
            //        {
            //            using (RegistryKey subkey = key.OpenSubKey(subkeyName))
            //            {
            //                string name = subkey?.GetValue("DisplayName") as string;
            //                if (name != null && name.IndexOf(displayName, StringComparison.OrdinalIgnoreCase) >= 0)
            //                    return true;
            //            }
            //        }
            //    }
            //}

            return false;
        }
    }
}
