using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherling.Models
{
    public class ServerSettings
    {
        public string Host { get; set; }
        public string Passkey { get; set; }

        public string APIKey { get; set; }
        public string Username { get; set; }

        internal void Update(IPasskeyProvider passkeyProvider)
        {
            var copy = passkeyProvider.GetServer(Host);
            if (!string.IsNullOrEmpty(copy.Passkey))
                this.Passkey = copy.Passkey;
            if (!string.IsNullOrEmpty(copy.APIKey))
                this.APIKey = copy.APIKey;
            if (!string.IsNullOrEmpty(copy.Username)) 
                this.Username = copy.Username;
        }
    }
}
