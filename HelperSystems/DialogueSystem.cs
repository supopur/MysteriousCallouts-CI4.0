using System.Collections.Generic;
using Rage;

namespace MysteriousCallouts.HelperSystems
{
    internal class DialogueSystem
    {
        internal List<string> Dialogue;
        internal int index;

        internal DialogueSystem(List<string> Dialogue)
        {
            index = 0;
            this.Dialogue = Dialogue;
        }

        internal void DisplayDialogue() => Game.DisplaySubtitle(Dialogue[index]);

        internal void AdvanceDialogue() => index++;

        internal void RewindDialogue() => index--;
    }
}