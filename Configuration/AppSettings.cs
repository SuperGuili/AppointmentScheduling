using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduling.Configuration
{
    public class AppSettings
    {
        public IDictionary<string, string> ConnectionStrings { get; set; }

        //public string this[string key] { get => ConnectionStrings[key]; }

        public string apiKey { get; set; }

        public string apiSecret { get; set; }

        //public AppSettings() { }

    }
}
