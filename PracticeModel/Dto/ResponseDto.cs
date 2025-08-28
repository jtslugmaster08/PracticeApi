using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Dto
{
    public class ResponseDto<T>
    {
        
        public bool IsSuccess { get { return Errors.Count > 0 ? false : true; } }
        public T Data { get; set; }
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();
    }
}
