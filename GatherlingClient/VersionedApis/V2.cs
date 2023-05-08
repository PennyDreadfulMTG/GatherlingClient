using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gatherling.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Gatherling.VersionedApis
{
    internal class V2 : V1
    {
        public V2(ServerSettings settings, CookieContainer cookies) : base(settings, cookies)
        {
        }

        public override int ApiVersion => 2;

        public override async Task<Event[]> GetActiveEventsAsync()
        {
            using (var api = CreateWebClient())
            {
                var json = JToken.Parse(await api.DownloadStringTaskAsync("/ajax.php?action=active_events"));
                if (json.Type == JTokenType.Object)
                {
                    var dict = json as JObject;

                    var events = new List<Event>();
                    foreach (var item in dict)
                    {
                        events.Add(LoadEvent(item.Key, item.Value as JObject));
                    }
                    return events.ToArray();
                }
                return new Event[0];
            }
        }

        public override async Task<Round> GetCurrentPairings(Event tournament)
        {
            using var api = CreateWebClient();
            var json = JObject.Parse(await api.DownloadStringTaskAsync("/ajax.php?action=active_events"));
            json = json[tournament.Name] as JObject;
            return Round.FromJson(json["matches"] as JArray, tournament);
        }

        private Event LoadEvent(string key, JObject value)
        {
            var @event = new Event(key, value, this);
            if (string.IsNullOrWhiteSpace(@event.Channel))
            {
                @event.Channel = RoomForSeries(@event.Series);
            }
            return @event;
        }

        public override Task<Standing[]> GetCurrentStandingsAsync(Event tournament)
        {
            if (tournament.Standings != null)
                return Task.FromResult(tournament.Standings);
            return base.GetCurrentStandingsAsync(tournament);
        }

        public async override Task<Event> GetEvent(string name)
        {
            using var api = CreateWebClient();
            string blob = await api.DownloadStringTaskAsync("/ajax.php?action=eventinfo&event=" + Uri.EscapeUriString(name));
            var json = JObject.Parse(blob);
            return LoadEvent(name, json);
        }
        public override async Task CreatePairingAsync(Event tournament, int round, Person a, Person b, string res)
        {
            using var api = CreateWebClient();
            await api.DownloadStringTaskAsync($"/api.php?action=create_pairing&event={tournament.Id}&round={round}&player_a={Uri.EscapeUriString(a.Name)}&player_b={Uri.EscapeUriString(b.Name)}&res={res}");
        }
    }
}
