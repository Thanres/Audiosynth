using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using System.ComponentModel;
using OxyPlot;
using System.Threading;
using OxyPlot.Series;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Sensordata.StartData();
            ChartInit();
            this.DataContext = this;
        }

        public void InitializationThread()
        {
            Console.WriteLine("der geht");
            while (true) {
                Console.WriteLine("der geht");
            }
        }

        private void btnrectype_Click(object sender, RoutedEventArgs e)
        {
            if (btnrectype.Content.ToString() == "Use Savedata")
            {
                btnrectype.Content = "Use Kinect";
                Sensordata.DisposeSensors();
                stop();

            }

            else if (btnrectype.Content.ToString() == "Use Kinect")
            {
                btnrectype.Content = "Use Savedata";
                Sensordata.StartPausedSensors();
                stop();
            }
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Startet den Stream
            Console.WriteLine(btnStart.Content.ToString());
            if (btnStart.Content.ToString() == "Start")
            {
                record.Background = new SolidColorBrush(Color.FromRgb(50, 0, 0));
                btnStart.Content = "Stopp";
                Sensordata.trackdata = true;
                dispatcherTimer.Start();
            }

            //Stoppt den Stream
            else if (btnStart.Content.ToString() == "Stopp")
            {
                stop();
            }
        }


        //Stoppt den Stream
        private void stop() {
            if (btnStart.Content.ToString() == "Stopp")
            {
                record.Background = new SolidColorBrush(Colors.Black);
                btnStart.Content = "Start";
                Sensordata.trackdata = false;
                dispatcherTimer.Stop();
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string data = Sensordata.getData();
            Console.WriteLine(data);
            File.AppendAllText("test.txt", data);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }



        public ImageSource ImageSource
        {
            get
            {
                return Sensordata.getBitmap();
            }
        }

        public ImageSource Bodysource
        {
            get
            {
                return Sensordata.getBodysource();
            }
        }


        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Sensordata.BodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                Sensordata.BodyFrameReader.Dispose();
                Sensordata.BodyFrameReader = null;
            }

            if (Sensordata.kinectSensor != null)
            {
                Sensordata.kinectSensor.Close();
                Sensordata.kinectSensor = null;
            }
        }

        private void Plot_DragEnter(object sender, DragEventArgs e)
        {

        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static PlotModel DataPlot { get; set; }
        public static BackgroundWorker chartsetter = new BackgroundWorker();
        public static int datainput;
        public static ulong[]  tracked_id = new ulong[5];
        private static DispatcherTimer dispatcherTimer;
        private static int index;


        public static void ChartInit()
        {
            DataPlot = new PlotModel();
            DataPlot.TextColor = OxyColors.FloralWhite;
            DataPlot.Title = "Frequönz";
            DataPlot.TitleColor = OxyColors.FloralWhite;
            DataPlot.PlotAreaBorderColor = OxyColors.FloralWhite;
            Thread.Sleep(100);
            DataPlot.Series.Add(new LineSeries());
            DataPlot.Series.Add(new LineSeries());
            DataPlot.Series.Add(new LineSeries());
            DataPlot.Series.Add(new LineSeries());
            DataPlot.Series.Add(new LineSeries());
            dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50) };
            dispatcherTimer.Tick += dispatcherTimer_Tick;
        }


        private static double _xValue = 1;


        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (Sensordata.frequences != null)
                {
                    //check trackingid
                    //delete if trackingid is no more in bodies
                    for (int i = 0; i < 5; i++)
                    {
                        index = 0;
                        foreach (ulong value in Sensordata.bodyIdList)
                        {
                            if (value == tracked_id[i])
                            {
                                index++;
                            }                          
                        }
                        if (index == 0)
                            tracked_id[i] = 0;
                    }
                    // write new trackingids to tracked_id if still space for more ids
                    for (int i = 0; i < Sensordata.bodyIdList.Count; i++)
                    {
                        index = 0;
                        for (int j = 0; j < 5; j++)
                        {
                            if (Sensordata.bodyIdList[i] == tracked_id[j])
                            {
                                index++;
                            }
                        }
                        if (index == 0)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (tracked_id[j] == 0)
                                {
                                    tracked_id[j] = Sensordata.bodyIdList[i];
                                    break;
                                }
                            }
                        }
                    }
                    //punkte zeichnen
                    for(int i = 0; i < 5; i++)
                    {
                        index = 0;
                        for (int j = 0; j < Sensordata.bodyIdList.Count; j++)
                        {   
                            if (tracked_id[i] == Sensordata.bodyIdList[j])
                            {
                                (DataPlot.Series[i] as LineSeries).Points.Add(new DataPoint(_xValue, Sensordata.frequences[j]));
                                index++;
                            }
                        }
                        if (index > 0) {
                            if ((DataPlot.Series[i] as LineSeries).Points.Count > 100)
                            {
                                (DataPlot.Series[i] as LineSeries).Points.RemoveAt(0);
                            }
                        }
                        if (index == 0)
                        {
                            if ((DataPlot.Series[i] as LineSeries).Points.Count > 0)
                            {
                                if(index==0)
                                     (DataPlot.Series[i] as LineSeries).Points.RemoveAt(0);
                            } 
                        }

                    }
                    WriteBinFile.writeFile(Sensordata.getData());
                    DataPlot.InvalidatePlot(true);
                    _xValue += 0.05;
                }
            });
        }

    }

}
