using System;
using System.Collections.Generic;
using Rage;

namespace MysteriousCallouts.HelperSystems
{
    internal class DialogueSystem
    {
        internal List<string> Dialogue;
        internal int index;
        internal Action FunctionAssociated;

        internal DialogueSystem(List<string> Dialogue, Action FunctionAssociated)
        {
            index = 0;
            this.Dialogue = Dialogue;
            this.FunctionAssociated = FunctionAssociated;
        }

        internal void DisplayDialogue() => Game.DisplaySubtitle(Dialogue[index]);

        internal void AdvanceDialogue()
        {
            index++;
            DisplayDialogue();
        }

        internal void RewindDialogue()
        {
            index--;
            DisplayDialogue();
        }

        internal void Run()
        {
            FunctionAssociated();
        }
    }
}