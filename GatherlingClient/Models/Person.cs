using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gatherling.Models
{
    public class Person
    {
        public string Name { get; set; }
        [JsonProperty("discord_id")]
        public long? DiscordId { get; set; }
        [JsonProperty("mtga_username")]
        public string MtgaUsername { get; set; }
        [JsonProperty("mtgo_username")]
        public string MtgoUsername { get; set; }

    }
}
