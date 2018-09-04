using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System;
using USBDrivesDetector.Manager;

namespace USBDrivesDetector.Mapping
{
    /// <summary>
    /// 
    /// Subst.MapDrive('z', @"c:\temp");
    /// Console.WriteLine(Subst.GetDriveMapping('z'));
    /// Subst.UnmapDrive('z');
    /// </summary>
    static class Subst
    {
        private const string substPrefix = @"\??\";

        public static void MapDrive(char letter, string path)
        {
            if (!DefineDosDevice(0, devName(letter), path))
                throw new Win32Exception();
        }
        public static void UnmapDrive(char letter)
        {
            if (!DefineDosDevice(2, devName(letter), null))
                throw new Win32Exception();
        }
        public static string GetDriveMapping(char letter)
        {
            var sb = new StringBuilder(259);
            if (QueryDosDevice(devName(letter), sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2) return "";
                throw new Win32Exception();
            }
            return sb.ToString().Substring(4);
        }

        public static bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static bool IsMappedDrive(char letter)
        {
            var sb = new StringBuilder(259);
            if (QueryDosDevice(devName(letter), sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2) return false;
                throw new Win32Exception();
            }
            return sb.ToString().StartsWith(substPrefix);
        }

        public static bool IsDriveMounted(char letter)
        {
            return Directory.Exists(devName(letter));
        }

        public static bool IsMappedPath(string path)
        {
            var sb = new StringBuilder(259);
            if (QueryDosDevice(pathDeviceName(path), sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2) return false;
                throw new Win32Exception();
            }
            return sb.ToString().StartsWith(substPrefix);
        }

        public static string GetPhysicalPath(String path)
        {
            var sb = new StringBuilder(259);
            string result = path; // do real mapping by default
            if (QueryDosDevice(pathDeviceName(path), sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2) return result;
                throw new Win32Exception();
            }
            // If drive is substed, the result will be in the format of "\??\C:\RealPath\".
            if (sb.ToString().StartsWith(substPrefix))
            {
                string root = sb.ToString().Remove(0, substPrefix.Length);
                result = Path.Combine(root, path.Replace(Path.GetPathRoot(path), ""));
            }

            return result;
        }

        public static string devName(char letter)
        {
            return new string(char.ToUpper(letter), 1) + ":";
        }

        public static string drivePath(char letter)
        {
            return new string(char.ToUpper(letter), 1) + @":\";
        }
         
        public static string getDriveSerialNumber(char letter)
        {
            string volumeSN = UsbManager.GetVolumeSerialNumber(devName(letter));
            if (volumeSN==null && IsMappedDrive(letter)&&IsDriveMounted(letter))
            {
                String realPath = GetDriveMapping(letter);
                volumeSN = UsbManager.GetVolumeSerialNumber(devName(realPath[0]));
            }
            return volumeSN;
        }
        private static string pathDeviceName(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            string pathRoot = Path.GetPathRoot(path);
            if (String.IsNullOrEmpty(pathRoot)) throw new ArgumentNullException("path");
            string lpDeviceName = pathRoot.Replace("\\", "");
            return lpDeviceName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string devname, string path);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string devname, StringBuilder buffer, int bufSize);
    }
}
