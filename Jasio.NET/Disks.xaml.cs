using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jasio.NET
{
    /// <summary>
    /// Логика взаимодействия для Disks.xaml
    /// </summary>
    public partial class Disks 
    {
        public Disks()
        {
            InitializeComponent();
            this.Icon = MainWindow.CreateBitmapSourceFromGdiBitmap(Properties.Resources.pc);

            System.IO.DriveInfo cdrive = new System.IO.DriveInfo("C");
            double availPercentage = Math.Round(100.0d *
                (double)cdrive.TotalFreeSpace / (double)cdrive.TotalSize);
            //gg
            List<DrivePercentage> dpList = new List<DrivePercentage>();
            dpList.Add(new DrivePercentage() { Percentage = availPercentage, Description = "Free " + (cdrive.TotalFreeSpace)/(1000*1000) + " MB" });
            dpList.Add(new DrivePercentage() { Percentage = 100.0d - availPercentage, Description = "Used " + (cdrive.TotalSize-cdrive.TotalFreeSpace)/(1000*1000) + " MB" });
            pieChart.DataContext = dpList;



        }

        internal class DrivePercentage
        {
            public double Percentage
            { get; set; }
            public string Description
            { get; set; }
        }

        
    }

   



}



