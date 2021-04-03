using Gatherling;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Tests
{
    class TestDecks
    {
        [TestCase]
        public async Task TestGatherlingDecks()
        {
            var deck = await GatherlingClient.GatherlingDotCom.GetDeckAsync(87052);
            Assert.AreEqual(true, deck.Found);
            Assert.AreEqual(87052, deck.Id);
            Assert.AreEqual("PD Drake", deck.Name);
        }
    }
}
