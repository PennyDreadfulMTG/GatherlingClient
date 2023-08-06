using System;
using System.Threading.Tasks;
using Gatherling.Models;

namespace Gatherling
{
    public interface IGatherlingApi
    {
        int ApiVersion { get; }
        ServerSettings Settings { get; }

        Task AuthenticateAsync();
        Task<Event[]> GetActiveEventsAsync();
        Task<Round> GetCurrentPairings(Event tournament);
        Task<Deck> GetDeckAsync(int deckID);
        Task<string> GetVerificationCodeAsync(string playerName);
        Task<string> ResetPasswordAsync(string playerName);
        Task<Event> GetEvent(string name);
        Task CreatePairingAsync(Event tournament, int round, Person a, Person b, string res);
        Task<Series> GetSeries(string name);
    }
}
