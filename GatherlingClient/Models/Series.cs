using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gatherling.Models
{
    public class Series
    {
        public IGatherlingApi Gatherling { get; }

        public string Name { get; }

        public bool Active { get; }
        public string StartDay { get; }
        
        public string StartTime { get; }

        public string[] Organizers { get; }

        public string MtgoRoom { get; }
        public string SeasonNumber { get; }
        public long? DiscordGuildId { get; }
        public long? DiscordChannelId { get; }
        public string DiscordChannelName { get; }
        public string DiscordGuildName { get; }

        public Series(IGatherlingApi api)
        {
            Gatherling = api ?? throw new ArgumentNullException(nameof(api));
        }

        public Series(JObject data, IGatherlingApi api)
        {
            Gatherling = api ?? throw new ArgumentNullException(nameof(api));
            Name = data.Value<string>("name");
            Active = data.Value<bool>("active");
            StartTime = data.Value<string>("start_time");
            StartDay = data.Value<string>("start_day");
            Organizers = ((JArray)data["organizers"]).Values<string>().ToArray();
            MtgoRoom = data.Value<string>("mtgo_room");
            SeasonNumber = data.Value<string>("this_season_season");
            DiscordGuildId = data.Value<long?>("discord_guild_id");
            DiscordChannelId = data.Value<long?>("discord_channel_id");
            DiscordChannelName = data.Value<string>("discord_channel_name");
            DiscordGuildName = data.Value<string>("discord_guild_name");
        }


    }
}
