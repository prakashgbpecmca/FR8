using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteApiException : Exception
    {
        public FrayteApiException()
        {

        }

        public FrayteApiException(string From, Exception message) : base(From, message)
        {

        }
    }
}
