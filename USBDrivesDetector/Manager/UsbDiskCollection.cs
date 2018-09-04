
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace USBDrivesDetector.Manager
{

    /// <summary>
    /// Maintains a collection of USB disk objects.
    /// </summary>

    public class UsbDiskCollection : ObservableCollection<UsbDisk>
	{

        /// <summary>
        /// Determines if the named disk is contained in this collection.
        /// </summary>
        /// <param name="name">The Windows name, or drive letter, of the disk to remove.</param>
        /// <returns>
        /// <b>True</b> if the item is found; otherwise <b>false</b>.
        /// </returns>
        public bool HasAny
        {
            get {
                return this.Count > 0;
            }           
        }
        public bool Contains (string name)
		{
			return this.AsQueryable<UsbDisk>().Any(d => d.Name == name) == true;
		}

        public IList<UsbDisk> ContainsVolumeSerials(string[] volumeSerials)
        {
            return this.AsQueryable<UsbDisk>().Where(d => d.SerialNumber.ContainsAny(volumeSerials)).OrderBy(

                d => Array.IndexOf(volumeSerials, d.SerialNumber)

            ).ToList();
        }


        /// <summary>
        /// Remove the named disk from the collection.
        /// </summary>
        /// <param name="name">The Windows name, or drive letter, of the disk to remove.</param>
        /// <returns>
        /// <b>True</b> if the item is removed; otherwise <b>false</b>.
        /// </returns>

        public bool Remove (string name)
		{
			UsbDisk disk = 
				(this.AsQueryable<UsbDisk>()
				.Where(d => d.Name == name)
				.Select(d => d)).FirstOrDefault<UsbDisk>();

			if (disk != null)
			{
				return this.Remove(disk);
			}

			return false;
		}
	}

    public static class StringExtension
    {
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length > 0)
            {
                foreach (string value in values)
                {
                    if (str.Contains(value))
                        return true;
                }
            }

            return false;
        }
    }
}
