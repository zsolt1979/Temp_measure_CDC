using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UART_Library
{
    public class Rx_Commands
    {
        public static List<MyClass> Sequence_Array_List = new List<MyClass> 
        { new MyClass{ Command_Rx = "<EXHAUST_TEMP>",   Arg_Lenght = 4,   Command_Nr = 0 },
          new MyClass{ Command_Rx = "<GET_LIMIT>",      Arg_Lenght = 1,   Command_Nr = 1 },
          new MyClass{ Command_Rx = "<SET_LIMIT>",      Arg_Lenght = 1,   Command_Nr = 2 },
          new MyClass{ Command_Rx = "<INVALID_CMD>",    Arg_Lenght = 0,   Command_Nr = 3 }
        };

        public class MyClass
        {
            public string? Command_Rx { get; set; }
            public int Arg_Lenght { get; set; }
            public int Command_Nr { get; set; }
        }
    }
}
