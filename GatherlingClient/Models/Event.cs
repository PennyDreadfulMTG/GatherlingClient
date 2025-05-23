using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gatherling.Models
{
    public class Event : IEqualityComparer<Event>
    {
        internal IGatherlingApi Gatherling;
        private Series _series = null;

        public string Id { get; set; }
        public DateTimeOffset Start { get; private set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public string Channel { get; set; }

        public string Series { get; set; }

        public string Host { get; set; }

        public SubEvent Main { get; set; }

        public SubEvent Finals { get; set; }
        public int CurrentRoundNum { get; }

        public string Client { get; set; }

        public Round CurrentRound
        {
            get
            {
                Rounds.TryGetValue(CurrentRoundNum, out var round);
                return round;
            }
        }

        public string[] Unreported { get; set; }

        public Standing[] Standings { get; }

        public Task<Round> GetCurrentPairingsAsync()
        {
            return Gatherling.GetCurrentPairings(this);
        }

        public Dictionary<int, Round> Rounds { get; } = new Dictionary<int, Round>();
        public Dictionary<string, Person> Players { get; }

        public override string ToString()
        {
            return $"<{Name}>";
        }

        public override bool Equals(object obj)
        {
            if (obj is Event e)
                return e.Name == this.Name;
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(Event x, Event y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Event obj)
        {
            return obj.Name.GetHashCode();
        }

        public Event(IGatherlingApi api)
        {
            Gatherling = api ?? throw new ArgumentNullException(nameof(api));
        }

        public Event(string name, JObject data, IGatherlingApi api)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Event Name is null", nameof(name));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Players = new Dictionary<string, Person>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var p in (JArray)data["players"])
            {
                 Players.Add(p.Value<string>("name"), p.ToObject<Person>());
            }

            Gatherling = api ?? throw new ArgumentNullException(nameof(api));
            Name = name;
            Channel = data.Value<string>("mtgo_room");
            if (Channel != null && !Channel.StartsWith("#"))
                Channel = "#" + Channel;
            Series = data.Value<string>("series");
            Main = new SubEvent(data.Value<string>("mainstruct"), data.Value<int>("mainrounds"));
            Finals = new SubEvent(data.Value<string>("finalstruct"), data.Value<int>("finalrounds"));
            CurrentRoundNum = data.Value<int>("current_round");
            Host = data.Value<string>("host");
            Id = data.Value<string>("id");
            var start = data.Value<string>("start");
            Start = DateTimeOffset.Parse(start + "-0500");
            if (data.ContainsKey("unreported"))
                Unreported = ((JArray)data["unreported"]).Values<string>().ToArray();
            Client = data.Value<string>("client");
            try
            {
                if (data.ContainsKey("standings"))
                {
                    var jArray = ((JArray)data["standings"]);
                    Standings = jArray.Select(t => ((JObject)t).ToObject<Standing>()).ToArray();
                }
            }
            catch (Exception c)
            {
                SentrySdk.CaptureException(c, scope => scope.SetExtra("event", data));
            }
            Round.FromJson((JArray)data["matches"], this);
        }

        public Task CreatePairingAsync(int round, Person A, Person B, string Res) => Gatherling.CreatePairingAsync(this, round, A, B, Res);

        public async Task<Series> GetSeriesAsync()
        {
            _series ??= await this.Gatherling.GetSeries(this.Series);
            return _series;
        }

        public TimeSpan ElapsedTime { get
            {
                var duration = DateTimeOffset.Now - Start;
                return duration;
            } 
        }
    }
}
