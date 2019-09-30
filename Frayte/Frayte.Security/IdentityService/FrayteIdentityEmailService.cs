using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Security.IdentityService
{
    class FrayteIdentityEmailService : IIdentityMessageService
    {
        #region methods

        //public Task SendAsync(IdentityMessage message)
        //{
        //    // Plug in your email service here to send an email.
        //    return Task.FromResult(0);
        //}
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
            //return sendmailasync(message);
        }

        private async Task sendmailasync(IdentityMessage message)
        {
            try
            {
                var apiKey = "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";//Environment.GetEnvironmentVariable("IdentityDemo");
                //var client = new SendGridClient(apiKey);
                //var from = new EmailAddress("praakshgbpecmca@outlook.com", "Test User");
                //var subject = message.Subject;
                //var to = new EmailAddress("prakash.pant@irasyssolutions.com", "Example User");
                //var plainTextContent = message.Body;
                //var htmlContent = message.Body;
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                //var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
