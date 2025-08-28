using PracticeModel.Entities;
using PracticeModel.Interface;
using PracticeModel.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeService
{
    public class ErrorService : IErrorService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ErrorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateError(Exception exception)
        {
            var error = new Error
            {
                StackTrace = exception.StackTrace,
                ErrorMessage = exception.Message,
                ErrorSource = exception.Source,
                MethodName = exception.TargetSite.Name
            };
            string exceptionText = "Exception:<br/>"+exception.ToString();
            if (exception.InnerException != null)
            {
                exceptionText = exceptionText + "<br/><br/> Exception.InnerException:<br/>" + exception.InnerException.ToString();
                if(exception.InnerException.InnerException != null)
                {
                    exceptionText = exceptionText+"<br/> <br/> Exception.InnerException.InnerException:<br/>"+exception.InnerException.InnerException.ToString();
                }
            }
            error.InnerMessage= exceptionText;
            SaveError(error);
        }

        public void SaveError(Error error)
        {
          _unitOfWork.ErrorRepository.Add(error);
            _unitOfWork.Commit();
        }
    }
}
