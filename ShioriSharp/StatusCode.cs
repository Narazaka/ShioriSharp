using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShioriSharp {
    /** <summary>SHIORI status code</summary> */
    public enum StatusCode {
        OK = 200,
        No_Content = 204,
        Communicate = 310,
        Not_Enough = 311,
        Advice = 312,
        Bad_Request = 400,
        Internal_Server_Error = 500,
    }
}

