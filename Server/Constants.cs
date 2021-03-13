using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Constants 
    {
        public const string Issuer = Audiance;
        public const string Audiance = "https://localhost:5001/";
        public const string Secret = "not_to_short_secret_otherwise_it_might_error";

    }
}
