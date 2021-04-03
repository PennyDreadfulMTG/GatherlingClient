using Gatherling;
using NUnit.Framework;
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
            var first = events.First();
            var pairings = await first.GetCurrentPairingsAsync();
            Assert.That(pairings.Matches.Any());
            Assert.That(first.Channel != null);
        }

        [Test]
        public async Task ParseStandings()
        {
            var @event = await GatherlingClient.GatherlingDotCom.GetEvent("Penny Dreadful Thursdays 12.01");
            Assert.NotNull(@event.Standings);
        }

        [Test]
        public async Task TestPlayers()
        {
            var e = await GatherlingClient.GatherlingDotCom.GetEvent("Penny Dreadful Thursdays 19.03");
            Assert.IsNotEmpty(e.Players);
            Assert.That(e.Players.Any(p => p.Value.DiscordId.HasValue));
            Assert.That(e.Players.Any(p => p.Value.MtgoUsername != null));
            Assert.That(e.Players.Any(p => p.Value.MtgaUsername != null));
        }
    }
}
