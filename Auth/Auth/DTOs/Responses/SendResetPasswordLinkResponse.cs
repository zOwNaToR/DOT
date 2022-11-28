using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.DTOs.Responses
{
    public class SendResetPasswordLinkResponse : BaseResponse
    {
        public string? ResetLink { get; set; }
        public string? ResetPasswordToken { get; set; }

        public SendResetPasswordLinkResponse() { }
        public SendResetPasswordLinkResponse(bool success) : base(success) { }
        public SendResetPasswordLinkResponse(bool success, List<string> errors) : base(success, errors) { }
    }
}
