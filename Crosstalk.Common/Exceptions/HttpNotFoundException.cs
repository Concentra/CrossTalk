using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Crosstalk.Common.Exceptions
{
    class HttpNotFoundException : HttpException
    {
        public HttpNotFoundException(string message) : base(404, message) {}
    }
}
