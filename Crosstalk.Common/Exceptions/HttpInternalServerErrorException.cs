using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Crosstalk.Common.Exceptions
{
    class HttpInternalServerErrorException : HttpException
    {
        public HttpInternalServerErrorException(string exception) : base(500, exception) {}
    }
}
