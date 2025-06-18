using System;
using System.IO;
using System.DirectoryServices;

namespace deploytool.iis
{
    public class IISRuntime
    {
        internal delegate void InstallIIS();
        internal delegate void UnInstallIIS();
        private string operatingSystemName;
        /// <summary>
        /// 操作系统名称
        /// </summary>
        protected string OperatingSystemName
        {
            get
            {
                if (operatingSystemName == null)
                {
                    operatingSystemName = CheckOSInfo.GetOSName();
                }
                return operatingSystemName;
            }
        }

        InstallIIS installIIS;
        UnInstallIIS unInstallIIS;
        IISManager iisManager;

        public IISRuntime()
        {
            bool is64Bit = CheckOSBitness.Is64BitOperatingSystem();
            string setupPackPath = is64Bit ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName + @" x64") :
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName);
            string inOptionalFilePath = is64Bit ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName + @" x64\Config\Install.txt") :
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName + @"\Config\Install.txt");
            string unOptionalFilePath = is64Bit ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName + @" x64\Config\UnInstall.txt") :
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Source\" + OperatingSystemName + @"\Config\UnInstall.txt");

            switch (OperatingSystemName)
            {
                case "Microsoft Windows XP":
                case "Microsoft Windows Server 2003":
                case "Microsoft Windows Server 2003 R2":
                    iisManager = new IISManagerXP(setupPackPath, inOptionalFilePath, unOptionalFilePath);
                    break;
                case "Microsoft Windows 7":
                case "Microsoft Windows Server 2008":
                case "Microsoft Windows Server 2008 R2":
                case "Microsoft Windows Vista":
                case "Microsoft Windows 8":
                case "Microsoft Windows Server 2012":
                case "Microsoft Windows 8.1":
                case "Microsoft Windows Server 2012 R2":
                    iisManager = new IISManager7();
                    break;
            }
            installIIS = new InstallIIS(iisManager.InstallIIS);
            unInstallIIS = new UnInstallIIS(iisManager.UnInstallIIS);
        }

        public void InstallIISRun()
        {
            Console.WriteLine(DateTime.Now + "IIS Manager {0} Begin!", OperatingSystemName);
            if (IsExsitIIS())
            {
                Console.WriteLine("ExsitIIS!");
            }
            else
            {
                installIIS();
            }
            Console.WriteLine(DateTime.Now + "IIS Installer {0} Success!", OperatingSystemName);
        }
        public void UnInstallIISRun()
        {
            Console.WriteLine(DateTime.Now + "IIS Manager {0} Begin!", OperatingSystemName);
            if (IsExsitIIS())
            {
                unInstallIIS();
            }
            else
            {
                Console.WriteLine("NotExsitIIS!");
            }
            Console.WriteLine(DateTime.Now + "IIS UnInstall {0} Success!", OperatingSystemName);
        }

        public bool IsExsitIIS()
        {
            DirectoryEntry de = new DirectoryEntry("IIS://localhost/W3SVC/INFO");
            if (de != null)
            {
                string Version = de.Properties["MajorIISVersionNumber"].Value.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
