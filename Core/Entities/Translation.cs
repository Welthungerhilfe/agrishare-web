using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Entities
{
    public class Translations
    {
        private static List<Translation> List = new List<Translation>
        {
            new Translation { Key = "Verification Code", English = "Your verification code is {0}", Shona = "", Ndebele = "" },
            new Translation { Key = "Booking Cancelled", English = "Booking #{0} cancelled", Shona = "", Ndebele = "" },
            new Translation { Key = "Booking Confirmed", English = "Booking #{0} confirmed", Shona = "", Ndebele = "" },
            new Translation { Key = "New Booking", English = "New booking received - #{0}", Shona = "", Ndebele = "" },
            new Translation { Key = "New Review", English = "New review received for booking #{0}", Shona = "", Ndebele = "" },
            new Translation { Key = "Payment Received", English = "Payment received for booking #{0}", Shona = "", Ndebele = "" },
            new Translation { Key = "Booking Completed", English = "Booking #{0} completed", Shona = "", Ndebele = "" },
            new Translation { Key = "Booking Not Completed", English = "Booking #{0} not completed", Shona = "", Ndebele = "" },
        };

        public static string Translate(string Key, Language Language)
        {
            var phrase = List.FirstOrDefault(o => o.Key == Key);

            if (phrase != null)
                switch (Language)
                {
                    case Language.English:
                        return phrase.English;
                    case Language.Shona:
                        return phrase.Shona;
                    case Language.Ndebele:
                        return phrase.Ndebele;
                }

            return string.Empty;
        }
    }

    public class Translation
    {
        public string Key { get; set; }
        public string English { get; set; }
        public string Shona { get; set; }
        public string Ndebele { get; set; }
    }
}
