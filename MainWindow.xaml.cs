using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static WPF_Serial_Ver_1_00.MainWindow;
using WPF_Serial_Ver_1_00.ViewModel;
using UART_Library;

namespace WPF_Serial_Ver_1_00
{
    public partial class MainWindow : Window
    {
        public static Status_info _status_info = new Status_info();
        public static CDC_info _cdc_information = new CDC_info();
        public int Button_Counter = 0;
        public bool Counter_Enable = false;
        
        public MainWindow()
        {
            InitializeComponent();

            Temp_Panel.DataContext =  _status_info;
            CDC_Panel.DataContext = _cdc_information;
            Warning_Field.DataContext = _status_info;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Main_WPF.Close();
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            string Detected_Port_Name = UART_Handling.GetUSBDevices();

            if (Detected_Port_Name != "NOT_DETECTED")
            {
                bool CDC_Init_Status = UART_Handling.Serial_Init(Detected_Port_Name);
                if (CDC_Init_Status)
                {
                    Send_Tx_Request();
                }
            }
        }

        public async void Send_Tx_Request()
        {
            Counter_Enable = true;
            while (Counter_Enable)
            {
                Button_Counter++;
                UART_Handling.Send_Tx_Data("<GET_TEMP>");
                await Task.Delay(350);
            }
        }

        private void TMax_Reset(object sender, RoutedEventArgs e)
        {

            _status_info.Reset_Temp_Max_Values();
            Debug.WriteLine("Reset was set...");
        }
    }
}