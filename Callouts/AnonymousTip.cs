using System;
using System.Collections.Generic;
using System.Net;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MysteriousCallouts.Events;
using MysteriousCallouts.HelperSystems;
using MysteriousCallouts.HelperSystems.Scaleforms;
using Rage;
using Rage.Attributes;
using Rage.Native;
using CalloutInterfaceAPI;

namespace MysteriousCallouts.Callouts
{
    [CalloutInterface("Anonymous Tip", CalloutProbability.Low, "Tip of a location of a wanted felon.", "Code 3", "FIB IAA")]
    internal class AnonymousTip : Callout
    {
        // Variables
        internal static Random rndm = new Random(DateTime.Now.Millisecond);
        internal static bool SuccessfulIPPing = true;
        internal static bool SuccessfulDecryption = true;
        internal static string EncryptedIP;
        internal static List<Blip> AllBlips = new List<Blip>();
        internal static List<Ped> AllPeds = new List<Ped>();
        internal static string msg = "";

        internal const string lettermsg =
            "You made it this far. I am shocked. These new gen z cops be kinda dog shit. They do not read. They probably won't even read this. You must be some oldie actual good cop. Anyways. Back to business. I have a hostage. If you want to see them alive, you better find me. Your final clue has been copied to your computer. Good Luck";

        // Callout Setup
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Anonymous Tip";
            CalloutPosition = Game.LocalPlayer.Character.Position;
            return base.OnBeforeCalloutDisplayed();
        }

        // Callout Process
        // Why would you want to unpause the game if the console is open WHYYYY
        public override void Process()
        {
            // Resume game if console is open
            if (Game.Console.IsOpen)
            {
                Game.IsPaused = false;
            }
            base.Process();
        }

        // Callout Accepted
        public override bool OnCalloutAccepted()
        {
            // Get encrypted IP and copy it to the clipboard
            EncryptedIP = IPHelper.GetEncryptedIP();
            // What even is this function
            Game.SetClipboardText(EncryptedIP);
            

            // Log and display notification with the tip message and encrypted IP
            Logger.Normal("OnCalloutAccepted() in AnonymousTip.cs", $"Encrypted IP: {EncryptedIP}");
            string tipMSG = $"{KidnappingEvent.GetRandomTip()} Complete the objective or suffer the consequences. Here is your first clue: {EncryptedIP}";
            msg = tipMSG;
            
            Game.DisplayNotification("mparcadecabinetgrid", "phone_anim_1", "NOTIFICATION", $"By {KidnappingEvent.GetRandomPhoneNumber()}", tipMSG);
            CalloutInterfaceAPI.Functions.SendMessage(this, "Anonymous tip received. Objective: Find the wanted felon. Clue: Encrypted IP");
            // Initiate the callout
            Callout();
            return base.OnCalloutAccepted();
        }

        // Callout Not Accepted
        public override void OnCalloutNotAccepted()
        {
            Logger.Normal("OnCalloutNotAccepted() in AnonymousTip.cs", "Callout not accepted by user");
            base.OnCalloutNotAccepted();
        }

        // Callout Method
        internal void Callout()
        {
            GameFiber.StartNew(delegate
            {
                // Start the decryption process
                StartDecryptionProcess();

                // Check if a broken down vehicle spawn point can be found
                if (BrokenDownVehicleEvent.FindSpawnPoint())
                {
                    CalloutInterfaceAPI.Functions.SendMessage(this, "Broken down vehicle reported by RP possible relation to this callout.");
                    // Spawn and trigger the broken down vehicle event
                    BrokenDownVehicleEvent.Spawning();
                    BrokenDownVehicleEvent.MainEvent();
                    
                }
                CalloutInterfaceAPI.Functions.SendMessage(this, "Mission completed successfully");
                // End the callout
                End();
            });
        }

        // Check if the IPPing process was successful
        internal static bool IsIPPingSuccessful() => SuccessfulIPPing;

        // Check if the decryption process was successful
        internal static bool IsDecryptionSuccessful() => SuccessfulDecryption;

        // Start the decryption process
        public void StartDecryptionProcess()
        {
            IPHelper.HelpWithDecryption();

            // Wait until decryption is successful
            while (!IsDecryptionSuccessful())
            {
                GameFiber.Wait(0);
            }

            IPHelper.HelpWithIPPing();

            // Wait until IPPing is successful
            while (!IsIPPingSuccessful())
            {
                GameFiber.Wait(0);
            }

            /*SuccessfulIPPing = false;
            SuccessfulDecryption = false;
            */
            CalloutInterfaceAPI.Functions.SendMessage(this, "Decryption of IP adress completed");
        }

        // Delete all blips
        internal static void DeleteAllBlips()
        {
            foreach (Blip b in AllBlips)
            {
                if (b.Exists())
                {
                    b.Delete();
                }
                else
                {
                    continue;
                }

            }
        }

        // Dismiss all peds
        internal static void DismissAllPeds()
        {
            foreach (Ped p in AllPeds)
            {
                if (p.Exists())
                {
                    p.Dismiss();
                }
                else
                {
                    continue;
                }

            }
        }

        // End the callout
        public override void End()
        {
            DeleteAllBlips();
            DismissAllPeds();
            Logger.Normal("End() in AnonymousTip.cs", "Ending anonymous tip callout");
            base.End();
        }
    }
}
