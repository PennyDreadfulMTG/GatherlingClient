using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Gatherling.Models
{
    public class Round
    {
        public int RoundNum { get; private set; }

        public bool IsFinals { get; private set; }

        public List<Pairing> Matches { get; } = new List<Pairing>();

        public IEnumerable<string> Players => Matches.SelectMany(m => m.Players);

        public EventStructure Structure { get; private set; }

        internal static Round FromJson(JArray matches, Event tournament = null)
        {
            var round = new Round();
            if (matches == null)
                return round;
            foreach (var m in matches)
            {
                if (m.Value<int>(nameof(round)) != round.RoundNum)
                {
                    round = new Round
                    {
                        RoundNum = m.Value<int>(nameof(round)),
                        IsFinals = m.Value<int>("timing") > 1,
                    };
                    if (tournament != null)
                    {
                        tournament.Rounds[round.RoundNum] = round;
                        if (round.IsFinals)
                            round.Structure = tournament.Finals.Mode;
                        else
                            round.Structure = tournament.Main.Mode;

                    }
                }
                var p = new Pairing
                {
                    A = m.Value<string>("playera"),
                    B = m.Value<string>("playerb"),
                    Verification = m.Value<string>("verification"),
                };
                if (m["playera_wins"] != null)
                {
                    p.A_wins = m.Value<int>("playera_wins");
                    p.B_wins = m.Value<int>("playerb_wins");
                }
                try
                {

                if (m["res"] != null)
                    p.Res = m.Value<string>("res");
                }
                catch (NullReferenceException c)
                {
                    Console.WriteLine(c);
                }
                round.Matches.Add(p);
            }
            round.Matches.Sort();

            return round;
        }
    }
}
