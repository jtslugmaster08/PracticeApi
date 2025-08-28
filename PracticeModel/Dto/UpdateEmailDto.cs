using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Dto
{
    public class UpdateEmailDto
    {
        public string NewEmail { get; set; }
        public string CurrentEmail { get; set; }
        public string Token { get; set; } 
    }
}
