using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Entities
{
    public class Error:BaseClass
    {
        public string ErrorMessage { get;set; }
        public string StackTrace { get; set; }
        public string MethodName { get; set; }  
        public string InnerMessage { get; set; }
        public string ErrorSource { get; set; }
    }
}
