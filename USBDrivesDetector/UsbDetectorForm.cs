using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USBDrivesDetector.Manager;
using USBDrivesDetector.Mapping;
using USBDrivesDetector.Properties;

namespace USBDrivesDetector
{
    public partial class UsbDetectorForm : Form
    {
        public const string APP_TITLE = "USB Drives Detector";
        private const string APP_VERSION = "1.0";
        private const string APP_COPY = "Copyright © 2018 NaturoSofts";

        private static readonly string APP_NAME = APP_TITLE; // Same, but could be different
        private const char DEFAULT_DRIVE = 'S';
        private char previousMappedDrive = DEFAULT_DRIVE;

        private UsbManager manager;

        private UsbDiskCollection disks = null;

        private ContextMenu trayMenu;

        private ContextMenu menu = new ContextMenu();

        private string[] usbDrivesToMount;

        private bool appCanExit = false;

        private bool allowStateChangeEvent = true;

        public char mappedDrive
        {
            get
            {
                return (char)mappedDriveComboBox.SelectedValue;
            }
            set
            {
                previousMappedDrive = value;
                mappedDriveComboBox.SelectedValue = value;
            }
        }

        public UsbDetectorForm()
        {
            InitializeComponent();

            manager = new UsbManager();

            initMappedDrive();

            LoadConfiguration();

            manager.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);

            mappedDriveComboBox.SelectedIndexChanged += OnMappedDriveComboBoxSelectedIndexChanged;

            SetAutorun();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Text = string.Format("{0} - v{1} - {2}", APP_TITLE, APP_VERSION, APP_COPY);

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false; // Remove from taskbar.
            MaximizeBox = false;
            MinimizeBox = false;

            trayIconSetup();

            base.OnLoad(e);
        }

        private void trayIconSetup()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add(APP_TITLE + " Exit", OnExit);
            // Add menu to tray icon and show it.
            notifyIcon.ContextMenu = trayMenu;
            notifyIcon.Visible = true;
        }

        private void SetAutorun()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                String keyValue = (string)registryKey.GetValue(APP_NAME);
                String appPath = Application.ExecutablePath;

                if (keyValue == null || keyValue != appPath)
                {

                    if (MessageBox.Show("Run automatically the agent '" + APP_TITLE + "' at startup?",
                                   APP_TITLE,
                                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        registryKey.SetValue(APP_NAME, appPath);
                        logText("Application " + APP_NAME + "sets to run automatically at startup");

                    }
                    //rk.DeleteValue(AppName, false);
                }
            }
        }

        private void SetNotifyInfo(bool mapped = false, string status = "")
        {
            string tooltip = APP_TITLE;
            if (!String.IsNullOrWhiteSpace(status))
            {
                tooltip += "\n" + status;
                notifyIcon.BalloonTipTitle = APP_TITLE;
                notifyIcon.BalloonTipText = status;
                notifyIcon.ShowBalloonTip(1000);
            }
            notifyIcon.Text = tooltip.Truncate(63); // Limited less than 64
            this.notifyIcon.Icon = mapped ? Resources.GREEN_ICON : Resources.RED_ICON;
            this.Icon = mapped ? Resources.GREEN_ICON : Resources.RED_ICON;
            statusLabel.Text = status;
            statusLabel.ForeColor = mapped ? System.Drawing.Color.Red : System.Drawing.Color.Red;
        }

        private void initMappedDrive()
        {
            var data = Enumerable.Range('D', 23).Select(x =>
              new
              {
                  Value = (char)x,
                  Text = Subst.devName((char)x)
              }).ToList();
            mappedDriveComboBox.ValueMember = "Value";
            mappedDriveComboBox.DisplayMember = "Text";
            mappedDriveComboBox.DataSource = data;
        }

        private void OnExit(object sender, EventArgs e)
        {
            appCanExit = true;
            Application.Exit();
        }

        private void mapDrive(char mountDrive, UsbDisk disk, bool unmapfirst = false)
        {
            if (unmapfirst)
            {
                Subst.UnmapDrive(mountDrive); // Unmount the bad one
            }
            string mountedFolder = disk.Name + @"\";
            Subst.MapDrive(mountDrive, mountedFolder); // mount the new one
            logText(string.Format("Virtual drive '{0}:' mounted to disk '{1}' ({2}/{3}) ", mountDrive, disk.Name, disk.Volume, disk.SerialNumber));
        }

        private void checkMountedDrive(char mountDrive)
        {
            bool mountedFolderAccess = Subst.HasWriteAccessToFolder(mountDrive + @":\");
            if (!mountedFolderAccess)
            {
                logText(string.Format("WARNING: The mounted drive '{0}:' seems to be not writeableVirtual drive '{0}:'", mountDrive));
            }
        }

        private void updateVirtualDriveMapping()
        {
            try
            {
                allowStateChangeEvent = false;

                bool isDriveMapped = false;
                int driveMappedIndex = -1;
                string driveStatusText = String.Empty;
                char virtualDrive = mappedDrive; // Copy localy since comes from combobox
                bool isVirtualDriveMapped = Subst.IsMappedDrive(virtualDrive);

                if (!isVirtualDriveMapped && Subst.IsDriveMounted(virtualDrive))
                {
                    // Husten, when have a problem, we cannot mount anything to this permanent drive
                    logText("WARNING : Target mapping drive '" + virtualDrive + "' is a permanent physical drive!");
                    SetNotifyInfo(isDriveMapped, string.Format("Invalid target drive '{0}:'", virtualDrive));
                    return;
                }

                IList<UsbDisk> usbDisks = disks.ContainsVolumeSerials(usbDrivesToMount);

                if (usbDisks.Count > 0) // A backup disk is available to be mounted
                {
                    UsbDisk diskToMount = usbDisks.First();

                    driveMappedIndex = Array.IndexOf(usbDrivesToMount, diskToMount.SerialNumber) + 1;

                    isDriveMapped = true;

                    if (isVirtualDriveMapped)
                    {
                        if (Subst.IsDriveMounted(virtualDrive))
                        {
                            // A drive is mapped and mounted, is it the good one ?
                            string mountedVolumeSN = Subst.getDriveSerialNumber(virtualDrive);
                            if (diskToMount.SerialNumber != mountedVolumeSN)
                            {
                                mapDrive(virtualDrive, diskToMount, true);
                            }
                            else
                            {
                                // The mapped drive is already the good one, so should be fine
                                logText(string.Format("Virtual drive '{0}:' already set and mounted to disk '{1}' ({2}/{3}) ", virtualDrive, diskToMount.Name, diskToMount.Volume, diskToMount.SerialNumber));
                            }
                        }
                        else // mapped, but not connected ....
                             // probably mapped to the wrong one
                        {
                            mapDrive(virtualDrive, diskToMount, true);
                        }
                    }
                    else // nothing mapped, so add the mapping
                    {
                        mapDrive(virtualDrive, diskToMount);
                    }
                    checkMountedDrive(virtualDrive);
                    driveStatusText = string.Format("{0}/{1} {2} mapped to drive {3}:", diskToMount.Volume, driveMappedIndex, diskToMount.Name, virtualDrive);
                }
                else // cleanup mounted disk
                {
                    if (isVirtualDriveMapped)
                    {
                        logText(string.Format("Virtual drive '{0}:' removed since no disk available", virtualDrive));
                        Subst.UnmapDrive(virtualDrive);
                    }

                    driveStatusText = string.Format("No USB disk on {0} mapped to drive {1}:", usbDrivesToMount.Count(), virtualDrive);
                }
                SetNotifyInfo(isDriveMapped, driveStatusText);

                previousMappedDrive = virtualDrive;
            }
            finally
            {
                allowStateChangeEvent = true;
            }
        }

        private void updateAvailableDisks()
        {
            disks = manager.GetAvailableDisks();

            logText(string.Format("{0} USB disks available", disks.Count()));

            if (disks.HasAny)
            {
                foreach (UsbDisk disk in disks)
                {
                    logText(disk.ToString());
                }
                logText();
            }
        }
        private void refreshAvailableDisks()
        {
            updateAvailableDisks();
            updateVirtualDriveMapping();
        }
        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            if (!allowStateChangeEvent) return;
            if (Char.ToUpper(e.Disk.Name[0]) == Char.ToUpper(mappedDrive)) return;
            if (Char.ToUpper(e.Disk.Name[0]) == Char.ToUpper(previousMappedDrive)) return;
            logText(e.State + " " + e.Disk.ToString() + " mapped :" + mappedDrive);

            BeginInvoke((MethodInvoker)delegate { refreshAvailableDisks(); });
        }
        private void logText(string message = "")
        {
            textBox.AppendText(message + Environment.NewLine);
        }

        private void OnNotifyIconClick(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
            this.WindowState = FormWindowState.Normal;
        }

        private void OnUsbDetectorFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!appCanExit)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void OnAddDriveButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            menu.MenuItems.Clear();

            foreach (UsbDisk disk in manager.GetAvailableDisks())
            {
                MenuItem item = new MenuItem();
                item.Text = disk.ToString();
                item.Tag = disk;
                item.Click += OnMenuItemClick;
                menu.MenuItems.Add(item);
            }
            if (menu.MenuItems.Count == 0)
            {
                MenuItem item = new MenuItem();
                item.Text = "No USB disk available";
                menu.MenuItems.Add(item);
            }
            menu.Show(btn, btn.PointToClient(Cursor.Position));

        }

        private void OnMenuItemClick(object sender, EventArgs e)
        {
            UsbDisk disk = (UsbDisk)((MenuItem)sender).Tag;

            bool duplicate = disksList.Items.Cast<UsbDisk>().Any(d => d.SerialNumber == disk.SerialNumber);
            if (!duplicate)
            {
                disksList.Items.Add(disk);
                SaveConfig(true);
            }
            else
            {
                MessageBox.Show(string.Format("Disk {0}/{1} has already been added!", disk.Volume, disk.SerialNumber));
            }
        }

        private void SaveConfig(bool refresh = false)
        {
            UsbDisk[] disks = disksList.Items.Cast<UsbDisk>().ToArray();
            SaveSettings(disks, mappedDrive.ToString());
            if (refresh) refreshAvailableDisks();
        }

        private void SaveSettings(UsbDisk[] disks = null, string drive = "")
        {
            Settings.Default.APP_DISK_IDS = UsbDisk.serialize(disks);
            Settings.Default.APP_MAP_DRIVE = drive;
            Settings.Default.Save();
            usbDrivesToMount = (disks != null) ? disks.Select(d => d.SerialNumber).ToArray() : new string[] { };

        }

        private void LoadConfiguration()
        {
            UsbDisk[] disks = UsbDisk.deserializeAll(Settings.Default.APP_DISK_IDS);
            disksList.Items.Clear();
            disksList.Items.AddRange(disks);

            mappedDrive = !String.IsNullOrEmpty(Settings.Default.APP_MAP_DRIVE) ? Settings.Default.APP_MAP_DRIVE[0] : DEFAULT_DRIVE;
            usbDrivesToMount = disks.Select(d => d.SerialNumber).ToArray();

            refreshAvailableDisks();
        }

        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            if (disksList.SelectedIndex != -1)
            {
                disksList.Items.RemoveAt(disksList.SelectedIndex);
                SaveConfig(true);
            }
        }

        private void OnMappedDriveComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                allowStateChangeEvent = false;
                if (mappedDrive != previousMappedDrive)
                {
                    if (Subst.IsMappedDrive(previousMappedDrive))
                    {
                        logText(string.Format("Previous virtual drive '{0}:' removed since no disk attached", previousMappedDrive));
                        Subst.UnmapDrive(previousMappedDrive);
                    }
                }
            }
            finally
            {
                allowStateChangeEvent = true;
            }
            SaveConfig(true);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear the entire configuration ??",
                                    "Confirm configuration reset!!",
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveSettings();
                LoadConfiguration();
            }
        }
    }

}
