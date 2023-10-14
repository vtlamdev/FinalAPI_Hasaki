using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalAPI_Hasaki.Controllers
{
    public class Forgot_Password
    {
        public int makh { get; set; }
        public string gmail { get; set; }
        public string MATKHAUHASH { get; set; }
        public string MATKHAUSALT { get; set; }
    }
}