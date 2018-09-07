using Mandrill;
using Mandrill.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public class Email
    {
        private static string _live { get; set; }
        private static bool Live
        {
            get
            {
                if (_live.IsEmpty())
                    _live = Config.Find(Key: "Live Email").Value;
                return _live.Equals("True", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private static string _developerEmailAddress { get; set; }
        private static string DeveloperEmailAddress
        {
            get
            {
                if (_developerEmailAddress.IsEmpty())
                    _developerEmailAddress = Config.Find(Key: "Developer Email Address").Value;
                return _developerEmailAddress;
            }
        }

        private static string _mandrillApiKey { get; set; }
        private static string MandrillApiKey
        {
            get
            {
                if (_mandrillApiKey.IsEmpty())
                    _mandrillApiKey = Config.Find(Key: "Mandrill API Key").Value;
                return _mandrillApiKey;
            }
        }

        public string SenderEmail { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
     
        private async Task<bool> SendAsync()
        {
            if (!Live)
                RecipientEmail = DeveloperEmailAddress;

            var message = new MandrillMessage()
            {
                ReplyTo = SenderEmail,
                FromName = Config.ApplicationName,
                FromEmail = Config.ApplicationEmailAddress,
                Html = Message,
                Subject = Subject
            };

            message.To.Add(new MandrillMailAddress()
            {
                Name = RecipientName,
                Email = RecipientEmail
            });

            var api = new MandrillApi(MandrillApiKey);
            var responses = await api.Messages.SendAsync(message);

            if (responses.Count() != 1)
                return false;

            var response = responses[0];
            if (response.Status == MandrillSendMessageResponseStatus.Invalid || response.Status == MandrillSendMessageResponseStatus.Rejected)
            {
                Log.Debug("Mandrill API Error", JsonConvert.SerializeObject(response));
                return false;
            }

            return true;
        }

        public void Send()
        {
            Task.Run(() => SendAsync());
        }

    }
}
