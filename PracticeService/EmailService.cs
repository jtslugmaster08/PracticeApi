using Microsoft.Extensions.Configuration;
using PracticeModel.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PracticeService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorService _errorService;

        public EmailService(IConfiguration configuration, IErrorService errorService)
        {
            _configuration = configuration;
            _errorService = errorService;
        }

        public bool SendEmail(string emailTo, string subject, string body)
        {
            bool retVal = true;
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("you email host");
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthicationInfo = new System.Net.NetworkCredential("YourUserName", "YourPassword");
                mySmtpClient.Credentials = basicAuthicationInfo;
                mySmtpClient.EnableSsl = true;
                mySmtpClient.Port = 001; //user the port of your choice


                MailAddress from = new MailAddress("TheFromAddress@example.com");
                MailAddress to = new MailAddress(emailTo);
                MailMessage myMail = new MailMessage(from, to);

                myMail.Subject = subject;
                myMail.SubjectEncoding = Encoding.UTF8;
                myMail.Body = body;
                myMail.BodyEncoding = Encoding.UTF8;

                myMail.IsBodyHtml = true;
                mySmtpClient.Send(myMail);

            }
            catch (SmtpException ex) { 
                _errorService.CreateError(ex);
                retVal = false;
            }
            catch(Exception ex)
            {
                _errorService.CreateError(ex);
                retVal = false;
            }

            return retVal;
        }
    }
}
