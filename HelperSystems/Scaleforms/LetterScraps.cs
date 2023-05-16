//ALL CREDIT FOR THIS CODE GOES TO SCALEFORMSRESEARCH:https://github.com/drummer-codes/ScaleformsResearch/blob/master/ScaleformsResearch/

using System;
using MysteriousCallouts.Callouts;
using Rage;

namespace MysteriousCallouts.HelperSystems.Scaleforms
{
    internal class LetterScraps : Movie
    {
        public override string MovieName => "LETTER_SCRAPS";

        public string Text { set => CallFunction("SET_LETTER_TEXT", value); }
        public void AddText(params string[] text) => CallFunction("ADD_TO_LETTER_TEXT", text);

        
    }
}