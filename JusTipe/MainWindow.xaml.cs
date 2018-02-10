using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        // Setting tracks what the user configured
        // Current tracks the current project
        // prog = Progress (countdown to complete a typing set)
        int progSetting;
        int progCurrent;
        // count = Countdown (countdown until window clears without input)
        int countSetting;
        int countCurrent;

        // Holds completed sets so failed sets don't clear them
        string safetext;

        public MainWindow()
        {
            InitializeComponent();

            progSetting = Convert.ToInt32(txtProgTimer.Text);
            countSetting = Convert.ToInt32(txtCountdown.Text);
            progCurrent = progSetting;
            countCurrent = countSetting;

            RunningTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(10_000_000) // 10 million ticks = 1 second
            };
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
            // Allows window to be moved without window chrome
            DragMove();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            safetext = "";
            Clear(false);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text document (.txt)|*.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, MainText.Text);
        }

        private void MainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MainText.Text))
            {
                if (running)
                {
                    // Currently running: reset countdown timer
                    countCurrent = countSetting;
                    txtCountdown.Text = Convert.ToString(countCurrent);
                }
                else
                {
                    // Not yet running: start a new set
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
            // OnTick should execute once per second to update counters and check if the set has completed or failed
            if (progCurrent <= 0)
            {
                // Set has completed successfully
                complete = true;
                Clear(complete);
            }
            else if (countCurrent <= 0)
            {
                // Countdown completed without input; set has failed
                Clear(complete);
            }
            else
            {
                // Update counters
                progCurrent--;
                countCurrent--;
                txtProgTimer.Text = Convert.ToString(progCurrent);
                txtCountdown.Text = Convert.ToString(countCurrent);
            }
        }

        public void Clear(bool complete)
        {
            // Save text if completed; otherwise, clear the set
            if (!complete)
                MainText.Text = safetext;
            else
            {
                SaveButton.IsEnabled = true;
                safetext = MainText.Text;
            }

            // Reset and reenable config boxes and save button
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
