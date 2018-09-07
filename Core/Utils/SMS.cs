using Agrishare.Core.Entities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    class SMS
    {
        private static string _username { get; set; }
        private static string Username
        {
            get
            {
                if (_username.IsEmpty())
                    _username = Config.Find(Key: "SMS Username").Value;
                return _username;
            }
        }

        private static string _sender { get; set; }
        private static string Sender
        {
            get
            {
                if (_sender.IsEmpty())
                    _sender = Config.Find(Key: "SMS Sender").Value;
                return _sender;
            }
        }

        public static decimal GetBalance()
        {
            var client = new RestClient($"https://www.txt.co.zw/Remote/AccountBalance?Username={Username}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            var response = client.Execute(request);
            return Convert.ToDecimal(response.Content);
        }

        public static bool SendMessage(string Recipient, string Message)
        {
            var client = new RestClient($"https://www.txt.co.zw/Remote/SendMessage?Username={Username}&Recipients={Recipient}&Body={Message}&sending_number={Sender}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            var response = client.Execute(request);
            if (response.Content.StartsWith("SUCCESS"))
                return true;

            Log.Error("SMS Error", JsonConvert.SerializeObject(response));
            return false;
        }
    }
}
