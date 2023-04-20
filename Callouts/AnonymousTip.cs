using System;
using System.Collections.Generic;
using System.Net;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using MysteriousCallouts.Events;
using MysteriousCallouts.HelperSystems;
using Rage;
using Rage.Attributes;
using Rage.Native;

namespace MysteriousCallouts.Callouts
{
    [CalloutInfo("Anonymous Tip", CalloutProbability.Low)]
    internal class AnonymousTip : Callout
    {
        internal static Random rndm = new Random(DateTime.Now.Millisecond);
        internal static bool SuccessfulIPPing = false;
        internal static bool SuccessfulDecryption = false;
        internal static string EncryptedIP;
        internal static List<Blip> AllBlips = new List<Blip>();
        internal static List<Ped> AllPeds = new List<Ped>();
        
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
            Game.DisplayNotification("mparcadecabinetgrid","phone_anim_1","NOTIFICATION",$"By {KidnappingEvent.GetRandomPhoneNumber()}",tipMSG);
            StartDecryptionProcess();
            GameFiber.WaitUntil(CheckpointStatus);
            KidnappingEvent.SetupVehicleWithHostage();
            return base.OnCalloutAccepted();
        }
        
        
        internal static bool IsIPPIngSuccessful() => SuccessfulIPPing;
        internal static bool IsDecryptionSuccessful() => SuccessfulDecryption;

        internal static bool CheckpointStatus() => SuccessfulIPPing && SuccessfulDecryption;

        internal static bool StartDecryptionProcess()
        {
            GameFiber.StartNew(delegate
            {
                IPHelper.HelpWithDecryption();
                GameFiber.WaitUntil(IsDecryptionSuccessful);
                IPHelper.HelpWithIPPing();
                GameFiber.WaitUntil(IsIPPIngSuccessful);
            });
            return IsIPPIngSuccessful() && IsDecryptionSuccessful();
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
            base.End();
            DeleteALlBlips();
            DismissALlPeds();
            Logger.Normal("End() in AnonymousTip.cs","Ending anonymous tip callout");
        }
    }
}