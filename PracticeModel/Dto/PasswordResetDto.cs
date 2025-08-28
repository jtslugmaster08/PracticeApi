using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Dto
{
    public class PasswordResetDto
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; } 
        public string NewPassword { get;set; }
    }
}
