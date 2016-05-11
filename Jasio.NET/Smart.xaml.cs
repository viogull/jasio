using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Management;
namespace Jasio.NET
{
    /// <summary>
    /// Логика взаимодействия для Smart.xaml
    /// </summary>
    public partial class Smart : Window
    {

        public class HDD
        {

            public int Index { get; set; }
            public bool IsOK { get; set; }
            public string Model { get; set; }
            public string Type { get; set; }
            public string Serial { get; set; }
            public Dictionary<int, SmartH> Attributes = new Dictionary<int, SmartH>() {
                {0x00, new SmartH("Помилка")},
                {0x01, new SmartH("Частота помилок читання")},
                {0x02, new SmartH("Продуктивність")},
                {0x03, new SmartH("Час розкрутки")},
                {0x04, new SmartH("Кількість циклів роботи шпинделя")},
                {0x05, new SmartH("Кількість переназначених секторів")},
                {0x06, new SmartH("Запас каналу читання")},
                {0x07, new SmartH("Частота помилок магн.блока")},
                {0x08, new SmartH("Продуктивність позиціонування магн. блока")},
                {0x09, new SmartH("Час роботи")},
                {0x0A, new SmartH("Кількість повторних розкруток")},
                {0x0B, new SmartH("Запити рекалібровки")},
                {0x0C, new SmartH("Цикли ввімкнення")},
                {0x0D, new SmartH("Помилки читання в ОС")},
                {0xB8, new SmartH("End-to-End")},
                {0xBE, new SmartH("Airflow")},
                {0xBF, new SmartH("Помилки від мех.ушк.")},
                {0xC0, new SmartH("Кількість завантаженнь магн.головок")},
                {0xC1, new SmartH("Цикли парковки")},
                {0xC2, new SmartH("HDD температура")},
                {0xC3, new SmartH("Hardware ECC")},
                {0xC4, new SmartH("Кількість переназначень")},
                {0xC5, new SmartH("Нестабільні сектори")},
                {0xC6, new SmartH("Кількість помилок читання/запису в сектор")},
                {0xC7, new SmartH("UDMA CRC помилки")},
                {0xC8, new SmartH("Помилки запису")},
                {0xC9, new SmartH("Помилки читання")},
                {0xCA, new SmartH(" Помилки Data Address Mark ")},
                {0xCB, new SmartH("Помилки через невірні хешсуми")},
                {0xCC, new SmartH("Soft ECC ")},
                {0xCD, new SmartH("TAR помилки")},
                {0xCE, new SmartH("Літаюча висота")},
                {0xCF, new SmartH("Кількість імп.струму")},
                {0xD0, new SmartH("Кількість для розкрутки диску")},
                {0xD1, new SmartH("Продуктивність в тестовах")},
                {0xDC, new SmartH("Зсув")},
                {0xDD, new SmartH("Помилки від мех.ушк")},
                {0xDE, new SmartH("Час роботи")},
                {0xDF, new SmartH("Цикли роботи")},
                {0xE0, new SmartH("Кількість завантаженнь")},
                {0xE1, new SmartH("Цикли роботи")},
                {0xE2, new SmartH("Час завантаження магн.головок")},
                {0xE3, new SmartH("Спроби компенсації швидкості")},
                {0xE4, new SmartH("Цикли підчас замиканнь")},
                {0xE6, new SmartH("GMR Амплітуда")},
                {0xE7, new SmartH("Температура")},
                {0xF0, new SmartH("Час старту")},
                {0xFA, new SmartH("Кількість помилок повторного читання")},
            };

        }
        public class SmartH
        {
            public bool HasData
            {
                get
                {
                    if (Current == 0 && Worst == 0 && Threshold == 0 && Data == 0)
                        return false;
                    return true;
                }
            }
            public string Attribute { get; set; }
            public int Current { get; set; }
            public int Worst { get; set; }
            public int Threshold { get; set; }
            public int Data { get; set; }
            public bool IsOK { get; set; }

            public SmartH()
            {

            }

            public SmartH(string attributeName)
            {
                this.Attribute = attributeName;
            }
        }


        public Smart()
        {
            InitializeComponent();
            this.Icon = MainWindow.CreateBitmapSourceFromGdiBitmap(Properties.Resources.pc);

            try
            {
                var dicDrives = new Dictionary<int, HDD>();
                var wdSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                int iDriveIndex = 0;
                foreach (ManagementObject drive in wdSearcher.Get())
                {
                    var hdd = new HDD();
                    hdd.Model = drive["Model"].ToString().Trim();
                    hdd.Type = drive["InterfaceType"].ToString().Trim();
                    dicDrives.Add(iDriveIndex, hdd);
                    iDriveIndex++;
                }

                var pmsearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

                iDriveIndex = 0;
                foreach (ManagementObject drive in pmsearcher.Get())
                {
                    if (iDriveIndex >= dicDrives.Count)
                        break;

                    dicDrives[iDriveIndex].Serial = drive["SerialNumber"] == null ? "None" : drive["SerialNumber"].ToString().Trim();
                    iDriveIndex++;
                }

                var searcher = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
                searcher.Scope = new ManagementScope(@"\root\wmi");

                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictStatus");
                iDriveIndex = 0;
                foreach (ManagementObject drive in searcher.Get())
                {
                    dicDrives[iDriveIndex].IsOK = (bool)drive.Properties["PredictFailure"].Value == false;
                    iDriveIndex++;
                }

                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictData");
                iDriveIndex = 0;
                foreach (ManagementObject data in searcher.Get())
                {
                    Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
                    for (int i = 0; i < 30; ++i)
                    {
                        try
                        {
                            int id = bytes[i * 12 + 2];

                            int flags = bytes[i * 12 + 4];
                            bool failureImminent = (flags & 0x1) == 0x1;

                            int value = bytes[i * 12 + 5];
                            int worst = bytes[i * 12 + 6];
                            int vendordata = BitConverter.ToInt32(bytes, i * 12 + 7);
                            if (id == 0) continue;

                            var attr = dicDrives[iDriveIndex].Attributes[id];
                            attr.Current = value;
                            attr.Worst = worst;
                            attr.Data = vendordata;
                            attr.IsOK = failureImminent == false;
                        }
                        catch
                        {
                        }
                    }
                    iDriveIndex++;
                }

                searcher.Query = new ObjectQuery("Select * from MSStorageDriver_FailurePredictThresholds");
                iDriveIndex = 0;
                foreach (ManagementObject data in searcher.Get())
                {
                    Byte[] bytes = (Byte[])data.Properties["VendorSpecific"].Value;
                    for (int i = 0; i < 30; ++i)
                    {
                        try
                        {

                            int id = bytes[i * 12 + 2];
                            int thresh = bytes[i * 12 + 3];
                            if (id == 0) continue;

                            var attr = dicDrives[iDriveIndex].Attributes[id];
                            attr.Threshold = thresh;
                        }
                        catch
                        {
                        }
                    }

                    iDriveIndex++;
                }

              
               
                foreach (var drive in dicDrives)
                {


                    foreach (var attr in drive.Value.Attributes)
                    {
                        Console.WriteLine("Init started");
                        if (attr.Value.HasData)
                        {
                            String s = (attr.Value.IsOK) ? "OK" : "";
                            Console.WriteLine(attr.Value.Attribute  + "   "  + " : " + attr.Value.Data);
                          
                            smartListView.Items.Add( new SMARTListItem() {
                                id = attr.Value.Attribute,
                                current = attr.Value.Current,
                                worst = attr.Value.Worst,
                                threshold = attr.Value.Threshold,
                                data = attr.Value.Data,
                                status = s
                            });

                        }
                    }

                }
            }
            catch (ManagementException ex)
            {
                MessageBox.Show("Перезапустіть програму з правами адміністратора");
            }
               
            catch(System.NullReferenceException e)
            {
                MessageBox.Show("Помилка виводу");
            }
           
            
        }

            
        public class SMARTListItem
        {
            public String id { get; set; }
            public int current {get; set;}

            public int worst { get; set; }

            public int threshold { get; set; }

            public int data { get; set; }

            public String status { get; set; }
        }
        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
