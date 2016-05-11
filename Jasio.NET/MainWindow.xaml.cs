using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Management;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Cryptography;
using Gat.Controls;

namespace Jasio.NET
{
 

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Icon = CreateBitmapSourceFromGdiBitmap(Properties.Resources.pc);
          
            this.os.Content =  getOsName();
            this.cpu.Content = GetProcessorInformation();
            this.video.Content = GetVideoAdapterName();
            this.memory.Content = GetPhysicalMemory();

            
            if(this.cpu.Content.ToString().Contains("Intel"))
                CPU_IMAGE.Source = CreateBitmapSourceFromGdiBitmap(Properties.Resources.intel);
            else if (this.cpu.Content.ToString().Contains("AMD"))
                CPU_IMAGE.Source = CreateBitmapSourceFromGdiBitmap(Properties.Resources.amd);

            if (this.os.Content.ToString().Contains("Windows 10"))
                OS_IMAGE.Source = CreateBitmapSourceFromGdiBitmap(Properties.Resources.win10);
            else if (this.os.Content.ToString().Contains("Windows 7"))
                OS_IMAGE.Source = CreateBitmapSourceFromGdiBitmap(Properties.Resources.win7);
            else if (this.os.Content.ToString().Contains("Windows 8"))
                OS_IMAGE.Source = CreateBitmapSourceFromGdiBitmap(Properties.Resources.win8);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Smart smart = new Smart();
            smart.Show();
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }


        public static String GetProcessorInformation()
        {
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String info = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name + ", " + (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
                //mo.Properties["Name"].Value.ToString();
                //break;
            }
           
            
            return  info.Substring(0, info.IndexOf(",") + 1);
        }

        public static string GetNoRamSlots()
        {

            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }
            return MemSlots.ToString();
        }

        public static string GetPhysicalMemory()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oCollection = oSearcher.Get();

            long MemSize = 0;
            long mCap = 0;

            foreach (ManagementObject obj in oCollection)
            {
                mCap = Convert.ToInt64(obj["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;
            return MemSize.ToString() + "MB";
        }

        public static String GetHDDSerialNo()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol)
            {
                result += Convert.ToString(strt["VolumeSerialNumber"]);
            }
            return result;
        }


        public static String GetVideoAdapterName()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            foreach (ManagementObject mo in searcher.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        return property.Value.ToString();
                    }
                }
            }
            return null;
        }

        public static String getOsName()
        {
            String s = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject cos in searcher.Get())
            {
                s =  cos["Caption"].ToString();
                break;
            }
            return s;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            string choose;
            

            if (result == true && md5.IsChecked == true )
            {
                string filename = dlg.FileName;
                MessageBox.Show("MD5 Checksum: " + GetMD5HashFromFile(filename));
            }

            else if
                (result == true && sha1.IsChecked == true)
            {
                string filename = dlg.FileName;
                MessageBox.Show("SHA-1 Checksum: " + GetSHAHashFromFile(filename));
            }


        }

        protected string GetMD5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }

        protected string GetSHAHashFromFile(string fileName)
        {
            using (var md5 = SHA1.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }

        private void sha1_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Disks disks = new Disks();
            disks.Show();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            AboutControlView about = new AboutControlView();
            AboutControlViewModel vm = (AboutControlViewModel)about.FindResource("ViewModel");

            vm.ApplicationLogo = CreateBitmapSourceFromGdiBitmap(Properties.Resources.pc);
            vm.Description =
                "Jasio.NET - невелика утиліта, що дозволяє переглянути базову інформацію про ПК, перевірити контрольні суми" +
                "файлів, а також S.M.A.R.T" +
                " і простір жорсткого диску.  ";
            vm.Publisher = "RSV";
            vm.Title = "Jasio.NET";
            vm.Version = "1.0b";
            vm.Copyright = "RSV";
            vm.AdditionalNotes = "";
   
            

            vm.Window.Content = about;
            vm.Window.Show();
        
        }
    }


}
