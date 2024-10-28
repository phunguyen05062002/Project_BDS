using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Responses
{
    public class ResponseMessage
    {
        public static string GetEmailSuccessMessage(string email)
        {
            return $"Email da duoc gui den: {email}";
        }
    }
}
