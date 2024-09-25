using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Management;
using System.Windows.Data;
using static WPF_Serial_Ver_1_00.MainWindow;
using WPF_Serial_Ver_1_00.ViewModel;
using WPF_Serial_Ver_1_00;
using System.ComponentModel;
using System.Windows.Controls;

namespace UART_Library
{
    public static class UART_Handling
    {
        public static bool Rx_status;
        public static bool COM_port_status;
        public static SerialPort _serialPort = new SerialPort();
        public delegate bool DelegateCall(List<string> _CDC_Rx_arg);

        public static bool Serial_Init(string Port_Number)
        {
            _serialPort.PortName = Port_Number;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(Serial_Read_Event);

            try
            {
                _serialPort.Open();
                COM_port_status = true;
            }

            catch
            {
                COM_port_status = false;
            }
            return COM_port_status;
        }

        public static void Serial_Read_Event(object sender, SerialDataReceivedEventArgs e)
        {
            Rx_status = true;
            List<DelegateCall> Sequence_List = new List<DelegateCall> { Parse_Functions.Function_1,
                                                                        Parse_Functions.Function_2,
                                                                        Parse_Functions.Function_3,
                                                                        Parse_Functions.Function_4 };
            DelegateCall FunctionCall;

            try
            {
                string message = _serialPort.ReadLine();
                List<string> words = (message.Split(' ')).ToList();

                if (words.Last() == "\r")
                {
                    bool CMD_Detected = false;
                    int CMD_Nr;
                    for (CMD_Nr = 0; CMD_Nr < Rx_Commands.Sequence_Array_List.Count; CMD_Nr++)
                    {
                        if (Rx_Commands.Sequence_Array_List[CMD_Nr].Command_Rx == words[0])
                        {
                            CMD_Detected = true;
                            break;
                        }
                        CMD_Detected = false;
                    }
                    if ((CMD_Detected == true) & (Rx_Commands.Sequence_Array_List[CMD_Nr].Arg_Lenght == words.Count - 2))
                    {
                        List<string> Argument_List = words.GetRange(1, words.Count - 2);     
                        FunctionCall = Sequence_List[CMD_Nr];
                        FunctionCall(Argument_List);
                    }
                    else { Console.WriteLine("Parsing was not successful..."); }
                }
            }
            catch (TimeoutException) { Console.WriteLine($"Timeout happened"); }
        }


        public static async void Send_Tx_Data(string Tx_Content)
        {
            _serialPort.Write($"{Tx_Content}\n");
            await UART_Timeout_Check();
        }

        public static async Task UART_Timeout_Check()
        {
            int i;
            Console.WriteLine("UART_Check_started");
            for (i = 0; i < 10; i++)
            {
                await Task.Delay(50);
                if (Rx_status == true)
                {
                    Rx_status = false;
                    break;
                }
            }
        }

        public static string GetUSBDevices()
        {
            string[] ports = SerialPort.GetPortNames();

            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity where DeviceID Like '%PID_2FC9%' ");
            ManagementObjectCollection collection = searcher.Get();
            string DeviceID_property = "";
            string Name_property = "";
            int loc;

            foreach (var device in collection)
            {
                Name_property = (string)device.GetPropertyValue("Name");
                DeviceID_property = (string)device.GetPropertyValue("DeviceID");
                 
                Debug.WriteLine($"Name: {Name_property} - DeviceID: {DeviceID_property}");
            }

            int COM_port_number = ports.Length;

            for (int i = 0; i < COM_port_number; i++)
            {
                string port = ($"({ports[i]})");
                loc = Name_property.IndexOf(port);
                Debug.WriteLine(port + "  " + loc);
                if (loc >= 0)
                {
                    return ports[i];
                }
            }
            
            return "NOT_DETECTED";
        }

        public static class Parse_Functions
        {
            public static bool Function_1(List<string> _CDC_Rx_arg)
            {
                _status_info.Update_Temperature_Values(_CDC_Rx_arg);
                _status_info.Update_Warning_Info(_CDC_Rx_arg);
                return false;
            }

            public static bool Function_2(List<string> _CDC_Rx_arg)
            {
                Console.WriteLine("Function 2 was called {0}", _CDC_Rx_arg[0]);
                return false;
            }
            public static bool Function_3(List<string> _CDC_Rx_arg)
            {
                Console.WriteLine("Function 3 was called {0}", _CDC_Rx_arg[0]);
                return false;
            }

            public static bool Function_4(List<string> _CDC_Rx_arg)
            {
                Console.WriteLine("Function 4 was called");
                return false;
            }
        }



    }

    public class USBDeviceInfo
    {
        public USBDeviceInfo( string description, string _PNPDeviceID)
        {
            this.Description = description;
            this.PNPDeviceID = _PNPDeviceID;
        }
        
        public string Description { get; private set; }
        public string PNPDeviceID { get; private set; }
    }

    




}
