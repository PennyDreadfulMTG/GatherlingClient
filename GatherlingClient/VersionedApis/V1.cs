using Gatherling.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using PDBot.API.GatherlingExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gatherling.VersionedApis
{
    class V1 : BaseApi
    {
        public override int ApiVersion { get; } = 1;

        public V1(ServerSettings settings, CookieContainer cookies)
            : base(settings, cookies)
        {
        }

        public override async Task<Event[]> GetActiveEventsAsync()
        {
            var uri = new Uri(new Uri(Settings.Host), "player.php");
            var playerCP = new HtmlDocument();
            await ScrapeAsync(uri, playerCP);
            var tables = playerCP.DocumentNode.Descendants("table");
            var activeEvents = tables.First(t => t.Descendants("b").FirstOrDefault(b => b.InnerText.Trim() == "ACTIVE EVENTS") != null);
            var rows = activeEvents.Descendants("tr");
            var paths = rows.Select(tr => tr.Descendants("a").FirstOrDefault()?.Attributes["href"]?.Value);
            return paths.Where(n => n != null).Select(n => n.Replace("eventreport.php?event=", string.Empty)).Select(n => LoadEvent(n)).ToArray();
        }

        private async Task ScrapeAsync(Uri uri, HtmlDocument document)
        {
            using (var wc = CreateWebClient())
            {
                document.LoadHtml(await wc.DownloadStringTaskAsync(uri));
                if (document.PageRequiresLogin())
                {
                    await AuthenticateAsync().ConfigureAwait(false);
                    document.LoadHtml(await wc.DownloadStringTaskAsync(uri));
                    if (document.PageRequiresLogin())
                    {
                        throw new InvalidOperationException("Can't log in!");
                    }
                }
            }
        }

        public override Task<Round> GetCurrentPairings(Event tournament)
        {
            throw new NotImplementedException();
        }

        private Event LoadEvent(string name)
        {
            return new Event(this)
            {
                Name = name,
                Series = name.Trim(' ', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'),
                Channel = RoomForSeries(name),
            };
        }

        /// <summary>
        /// Hack for v0 events.
        /// </summary>
        /// <param name="eventName">Name of an event or series</param>
        /// <returns>Name of the room</returns>
        protected static string RoomForSeries(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("message", nameof(eventName));
            }

            var series = eventName.Trim(' ', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            switch (series)
            {
                case "Penny Dreadful Thursdays":
                    return "#PDT";
                case "Penny Dreadful Saturdays":
                case "Penny Dreadful Sundays":
                    return "#PDS";
                case "Penny Dreadful Mondays":
                    return "#PDM";
                case "Classic Heirloom":
                    return "#heirloom";
                case "Community Legacy League":
                    return "#CLL";
                case "PauperPower":
                    return "#pauperpower";
                case "Modern Times":
                    return "#modern";
                case "Pauper Classic Tuesdays":
                    return "#pct";
                case "Vintage MTGO Swiss":
                    return "#vintageswiss";
                default:
                    break;
            }
            if (series.StartsWith("CLL Quarterly") || series.StartsWith("Community Legacy League"))
                return "#CLL";

            return null;
        }

        public override Task<Standing[]> GetCurrentStandingsAsync(Event tournament)
        {
            return Task.FromResult<Standing[]>(null);
        }

        public override Task<Event> GetEvent(string name)
        {
            throw new NotImplementedException();
        }
    }
}
