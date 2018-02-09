using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace JusTipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer RunningTimer;

        bool running;
        bool complete;

        int progSetting;
        int progCurrent;
        int countSetting;
        int countCurrent;

        string safetext;

        public MainWindow()
        {
            InitializeComponent();

            progSetting = Convert.ToInt32(txtProgTimer.Text);
            countSetting = Convert.ToInt32(txtCountdown.Text);
            progCurrent = progSetting;
            countCurrent = countSetting;

            RunningTimer = new DispatcherTimer();
            RunningTimer.Interval = new TimeSpan(10_000_000);
            RunningTimer.Tick += new EventHandler(OnTick);
            running = false;
            complete = false;
            SaveButton.IsEnabled = false;

            safetext = "";

            MainText.Focus();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            safetext = "";
            Clear(false);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text document (.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, MainText.Text);
        }

        private void MainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainText.Text != "")
            {
                if (running)
                {
                    countCurrent = countSetting;
                    txtCountdown.Text = Convert.ToString(countCurrent);
                }
                else
                {
                    running = true;
                    complete = false;
                    txtProgTimer.IsEnabled = false;
                    txtCountdown.IsEnabled = false;
                    progSetting = Convert.ToInt32(txtProgTimer.Text);
                    countSetting = Convert.ToInt32(txtCountdown.Text);
                    progCurrent = progSetting;
                    countCurrent = countSetting;
                    SaveButton.IsEnabled = false;

                    RunningTimer.IsEnabled = true;
                }
            }
        }

        public void OnTick(object sender, EventArgs e)
        {
            if (progCurrent <= 0)
            {
                complete = true;
                Clear(complete);
            }
            else if (countCurrent <= 0)
            {
                Clear(complete);
            }
            else
            {
                progCurrent--;
                countCurrent--;
                txtProgTimer.Text = Convert.ToString(progCurrent);
                txtCountdown.Text = Convert.ToString(countCurrent);
            }
        }

        public void Clear(bool complete)
        {
            if (!complete)
                MainText.Text = safetext;
            else
            {
                SaveButton.IsEnabled = true;
                safetext = MainText.Text;
            }

            txtProgTimer.Text = Convert.ToString(progSetting);
            txtCountdown.Text = Convert.ToString(countSetting);
            txtProgTimer.IsEnabled = true;
            txtCountdown.IsEnabled = true;
            running = false;
            complete = false;

            RunningTimer.IsEnabled = false;
        }
    }
}
