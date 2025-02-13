using Gatherling;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PDBot.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class TestEvents
    {
        [Theory]
        public async Task GetActiveEvents()
        {
            var events = new Gatherling.Models.Event[0];
            if (GatherlingClient.GatherlingDotCom.ApiVersion > 0)
                events = await GatherlingClient.GatherlingDotCom.GetActiveEventsAsync();

            if (events.Length == 0)
                events = await GatherlingClient.PennyDreadful.GetActiveEventsAsync();
            Assume.That(events.Length > 0);
            var first = events.Where(e => e.Main.Mode != Gatherling.Models.EventStructure.League).FirstOrDefault() ?? events.Last();
            ClassicAssert.That(first.CurrentRound.Matches.Any());
            var pairings = await first.GetCurrentPairingsAsync();
            ClassicAssert.That(pairings.Matches.Any());
            ClassicAssert.That(first.Channel != null);
        }

        [Test]
        public async Task ParseStandings()
        {
            var @event = await GatherlingClient.GatherlingDotCom.GetEvent("Penny Dreadful Thursdays 12.01");
            ClassicAssert.NotNull(@event.Standings);
        }

        [Test]
        public async Task TestPlayers()
        {
            var e = await GatherlingClient.GatherlingDotCom.GetEvent("Penny Dreadful Thursdays 19.03");
            ClassicAssert.IsNotEmpty(e.Players);
            ClassicAssert.That(e.Players.Any(p => p.Value.DiscordId.HasValue));
            ClassicAssert.That(e.Players.Any(p => p.Value.MtgoUsername != null));
            ClassicAssert.That(e.Players.Any(p => p.Value.MtgaUsername != null));
            ClassicAssert.That(e.Rounds[8].Matches[0].PlayerA != null);
        }

        [Test]
        public async Task TestSeries()
        {
            var e = await GatherlingClient.GatherlingDotCom.GetEvent("Penny Dreadful Thursdays 29.01");
            ClassicAssert.NotNull(e.Series);
            var series = await e.GetSeriesAsync();
            ClassicAssert.NotNull(series);
            ClassicAssert.IsNotEmpty(series.Name);
            ClassicAssert.AreEqual(e.Series, series.Name);
        }
    }
}
