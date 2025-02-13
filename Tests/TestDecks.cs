using Gatherling;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace Tests
{
    class TestDecks
    {
        [TestCase]
        public async Task TestGatherlingDecks()
        {
            var deck = await GatherlingClient.GatherlingDotCom.GetDeckAsync(87052);
            ClassicAssert.AreEqual(true, deck.Found);
            ClassicAssert.AreEqual(87052, deck.Id);
            ClassicAssert.AreEqual("PD Drake", deck.Name);
        }
    }
}
