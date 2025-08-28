using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Interface.Services
{
    public interface IEmailService
    {
        bool SendEmail(string emailTo,string subject, string body);
    }
}
