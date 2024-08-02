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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;

namespace Arduino_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort sp = new SerialPort();
        string[] ports = SerialPort.GetPortNames();
        SerialPort[] spPorts = new SerialPort[SerialPort.GetPortNames().Length];
        bool result = false;

        public MainWindow()
        {
            InitializeComponent();

            //Доступные порты в cb1
            cb1.ItemsSource = ports;

            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
        }

        void PortInitialization()
        {
            for (int i = 0; i < ports.Length; i++)
            {
                try
                {
                    spPorts[i] = new SerialPort();
                    spPorts[i].BaudRate = 9600;
                    spPorts[i].PortName = ports[i];
                    spPorts[i].DataReceived += new SerialDataReceivedEventHandler(InitializationDataReceived);
                    spPorts[i].Open();
                }
                catch { }
            }

            Thread.Sleep(2000);

            for (int i = 0; i < spPorts.Length; i++)
            {
                try
                {
                    spPorts[i].Write("Hello");
                }
                catch { }
            }
        }

        void InitializationDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort mySerial = (SerialPort)sender;
            string str = mySerial.ReadExisting();
            if (str == "Hello")
            {
                sp.PortName = mySerial.PortName;
                result = true;
            }

        }

        async Task PortInitializationTimeOut()
        {
            await Task.Delay(1000);
            for (int i = 0; i < spPorts.Length; i++)
                spPorts[i].Close();

            if (result)
                cb1.SelectedItem = sp.PortName;
            else
                MessageBox.Show("Не удалось автоматически определить устройство.");
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                sp.PortName = cb1.SelectedItem as string;
                sp.BaudRate = 9600;
                sp.Open();
                MessageBox.Show("Порт открыт");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void on_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Write("on");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void off_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Write("off");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() => TextOut.Text = sp.ReadExisting());
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            sp.Write(TextIn.Text);
        }

        private void AutoSelectPort_Click(object sender, RoutedEventArgs e)
        {
            PortInitialization();
            _ = PortInitializationTimeOut();
        }
    }
}
