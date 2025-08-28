using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Dto
{
    public class ConfirmUpdateEmailDto
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public string NewEmail { get; set; }
    }
}
