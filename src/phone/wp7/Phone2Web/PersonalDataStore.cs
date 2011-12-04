using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace Phone2Web
{
    public static class PersonalDataStore
    {
        // order must match the order in phone2web.js
        static string[] identifiers = 
        { 
            "name",
            "address",
            "city",
            "zip",
            "state",
            "phone",
            "email",
            "visa"
        };

        static Dictionary<string, string> sampleData = new Dictionary<string, string>
        {
            { "name", "Tomasz Janczuk" },
            { "address", "1 Redmond Way" },            
            { "city", "Redmond" },
            { "zip", "98052" },
            { "state", "WA" },
            { "phone", "425-123-1234" },
            { "email", "foo@bar.com" },
            { "visa", "1234123412341234" },
        };

        public static Collection<string> GetIdentifiers(ulong options)
        {
            Collection<string> result = new Collection<string>();
            int i = 0;
            while (0 != options && i < identifiers.Length)
            {
                if (0 != (1 & options))
                {
                    result.Add(identifiers[i]);
                }

                options >>= 1;
                i++;
            }

            return result;
        }

        public static string GetData(Collection<string> ids)
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (string id in ids)
            {
                if (sampleData.ContainsKey(id))
                {
                    if (sb.Length > 1)
                    {
                        sb.Append(",");
                    }

                    sb.AppendFormat("\"{0}\":\"{1}\"", id, sampleData[id]);
                }
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
