using System;
using System.Management;

namespace USBDrivesDetector.Manager
{

	
	public static class WmiExtensions
	{

		/// <summary>
		/// Fetch the first item from the search result collection.
		/// </summary>
		/// <param name="searcher"></param>
		/// <returns></returns>

		public static ManagementObject First (this ManagementObjectSearcher searcher)
		{
			ManagementObject result = null;
			foreach (ManagementObject item in searcher.Get())
			{
				result = item;
				break;
			}
			return result;
		}

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }
            
            if (value.Length > maxLength-1)
                return value.Substring(0, maxLength-1) + '…';
            return value;
        }
    }
}
