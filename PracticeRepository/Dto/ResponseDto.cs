using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeRepository.Dto
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get;set; }
        public List<ErrorDto> Errors { get; set; }    
    }
}
