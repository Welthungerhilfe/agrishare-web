using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class SNS
    {
        private static string ApplicationARN => Entities.Config.Find(Key: "AWS Application ARN").Value;
        private static string AccessKey => Entities.Config.Find(Key: "AWS Access Key").Value;
        private static string SecretKey => Entities.Config.Find(Key: "AWS Secret Key").Value;
        private static RegionEndpoint Region => RegionEndpoint.GetBySystemName(Entities.Config.Find(Key: "AWS Region").Value);
        private static bool Log => Entities.Config.Find(Key: "Log SNS").Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);

        private static IAmazonSimpleNotificationService CreateClient()
        {
            return AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(AccessKey, SecretKey, Region);
        }

        public static bool AddDevice(string DeviceToken, out string DeviceARN)
        {
            DeviceARN = null;
            try
            {
                using (var client = CreateClient())
                {
                    var request = new CreatePlatformEndpointRequest
                    {
                        PlatformApplicationArn = ApplicationARN,
                        Token = DeviceToken
                    };

                    var response = client.CreatePlatformEndpoint(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        DeviceARN = response.EndpointArn;

                        var attributeRequest = new GetEndpointAttributesRequest()
                        {
                            EndpointArn = DeviceARN
                        };
                        var attributeResponse = client.GetEndpointAttributes(attributeRequest);
                        if (attributeResponse.Attributes["Token"] != DeviceARN || attributeResponse.Attributes["Enabled"].ToLower() != "true")
                        {
                            var updateRequest = new SetEndpointAttributesRequest()
                            {
                                EndpointArn = DeviceARN,
                                Attributes = new Dictionary<string, string>()
                                {
                                    {"Token", DeviceToken },
                                    {"Enabled", "true" }
                                }
                            };

                            var updateResponse = client.SetEndpointAttributes(updateRequest);
                            return updateResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
                        }
                        else return true;
                    }
                    else return false;
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SNS.CreateDeviceEndpoint", ex);
                return false;
            }
        }

        public static bool RemoveDevice(string DeviceARN)
        {
            try
            {
                using (var client = CreateClient())
                {
                    var request = new DeleteEndpointRequest()
                    {
                        EndpointArn = DeviceARN
                    };

                    var resp = client.DeleteEndpoint(request);
                    return resp.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Entities.Log.Error("SNS.RemoveDeviceEndpoint", ex);
                return false;
            }
        }

        public static bool SendMessage(string DeviceARN, string Message, string Category = "", Dictionary<string, object> Params = null)
        {
            dynamic gcmPayload = new ExpandoObject();
            gcmPayload.data = new ExpandoObject();
            gcmPayload.data.message = Message;
            gcmPayload.data.category = Category;

            if (Params != null)
                foreach (string key in Params.Keys)
                    ((IDictionary<String, Object>)gcmPayload.data)[key] = Params[key];

            var payload = string.Format(@"{{""default"":{0},""GCM_SANDBOX"":{1},""GCM"":{1}}}",
               JsonConvert.SerializeObject(Message),
               JsonConvert.SerializeObject(JsonConvert.SerializeObject(gcmPayload)));

            if (Log)
                Entities.Log.Debug("SNS.SendMessage", JsonConvert.SerializeObject(payload));

            using (var client = CreateClient())
            {
                var request = new PublishRequest
                {
                    TargetArn = DeviceARN,
                    Message = payload,
                    Subject = Message,
                    MessageStructure = "json"
                };

                var resp = client.Publish(request);
                return (resp.HttpStatusCode == System.Net.HttpStatusCode.OK);
            }
        }
        
    }
}
