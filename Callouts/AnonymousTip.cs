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
        internal static Random rndm = new Random(DateTime.Now.Millisecond);
        internal static bool SuccessfulIPPing = true;
        internal static bool SuccessfulDecryption = true;
        internal static string EncryptedIP;
        internal static List<Blip> AllBlips = new List<Blip>();
        internal static List<Ped> AllPeds = new List<Ped>();
        internal static string msg = "";

        internal const string lettermsg =
            "You made it this far. I am shocked. These new gen z cops be kinda dog shit. They do not read. They probably won't even read this. You must be some oldie actual good cop. Anyways. Back to business. I have a hostage. If you want to see them alive, you better find me. Your final clue has been copied to your computer. Good Luck";
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Anonymous Tip";
            CalloutPosition = Game.LocalPlayer.Character.Position;
            return base.OnBeforeCalloutDisplayed();
        }
        
        public override void Process()
        {
            if (Game.Console.IsOpen)
            {
                Game.IsPaused = false;
            }
            base.Process();
        }
        public override bool OnCalloutAccepted()
        {
            EncryptedIP = IPHelper.GetEncryptedIP();
            Game.SetClipboardText(EncryptedIP);
            Logger.Normal("OnCalloutAccepted() in AnonymousTip.cs",$"Encrypted IP: {EncryptedIP}");
            string tipMSG = $"{KidnappingEvent.GetRandomTip()} Complete the objective or suffer the consequences. Here is your first clue: {EncryptedIP}";
            msg = tipMSG;
            Game.DisplayNotification("mparcadecabinetgrid","phone_anim_1","NOTIFICATION",$"By {KidnappingEvent.GetRandomPhoneNumber()}",tipMSG);
            Callout();
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            Logger.Normal("OnCalloutNotAccepted() in AnonymousTip.cs", "Callout not accepted by user");
            base.OnCalloutNotAccepted();
        }

        internal void Callout()
        {
            GameFiber.StartNew(delegate
            {
                StartDecryptionProcess();
                if(BrokenDownVehicleEvent.FindSpawnPoint());
                {
                    BrokenDownVehicleEvent.Spawning();
                    BrokenDownVehicleEvent.MainEvent();
                }
                End();
            });
        }
        
        internal static bool IsIPPIngSuccessful() => SuccessfulIPPing;
        internal static bool IsDecryptionSuccessful() => SuccessfulDecryption;
        
        internal static void StartDecryptionProcess()
        {
                IPHelper.HelpWithDecryption();
                while(!IsDecryptionSuccessful()){GameFiber.Wait(0);}
                IPHelper.HelpWithIPPing();
                while(!IsIPPIngSuccessful()){GameFiber.Wait(0);}
                /*SuccessfulIPPing = false;
                SuccessfulDecryption = false;
                */
        }
            



        internal static void DeleteALlBlips()
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
        internal static void DismissALlPeds()
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

        public override void End()
        {
            DeleteALlBlips();
            DismissALlPeds();
            Logger.Normal("End() in AnonymousTip.cs","Ending anonymous tip callout");
            base.End();
        }
    }
}