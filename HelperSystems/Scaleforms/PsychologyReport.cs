using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MysteriousCallouts.Callouts;
using MysteriousCallouts.HelperSystems;
using MysteriousCallouts.HelperSystems.Scaleforms;

namespace ScaleformsResearch.Movies
{
    internal class PsychologyReport : LetterScraps
    {
        public override string MovieName => "PSYCHOLOGY_REPORT";
        public string Text { set => CallFunction("SET_LETTER_TEXT", value); }
        public void AddText(params string[] text) => CallFunction("ADD_TO_LETTER_TEXT", text);

        public void SetPlayerName(string gamerTitle, string gamerTag, string crewtag)
        {
            CallFunction("SET_PLAYER_NAME", gamerTitle, gamerTag, crewtag);
        }

        protected override void OnTestStart()
        {
            base.OnTestStart();
            SetPlayerName("Your Worst Enemy: ", "Anonymous", "AR");
            string msg = AnonymousTip.lettermsg;
            string[] msgSplit = msg.Split(' ');
            CallFunction("SET_LETTER_TEXT", (msgSplit[0] + " "));
            for (int i = 1; i < msgSplit.Length; i++)
            {
                CallFunction("ADD_TO_LETTER_TEXT", (msgSplit[i] + " "));
            }
        }
    }
}