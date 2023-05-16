using System;
using System.Collections.Generic;
using MysteriousCallouts.Callouts;
using MysteriousCallouts.HelperSystems;
using Rage;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using MysteriousCallouts.HelperSystems.Scaleforms;
using Rage.Native;
using ScaleformsResearch.Movies;

namespace MysteriousCallouts.Events
{
    internal static class BrokenDownVehicleEvent
    {
        internal static Ped MainPlayer => Game.LocalPlayer.Character;
        internal static string[] VehicleModels = new string[] {"asbo", "blista", "dilettante", "panto", "prairie", "cogcabrio", "exemplar", "f620", "felon", "felon2", "jackal", "oracle", "oracle2", "sentinel", "sentinel2",
            "zion", "zion2", "baller", "baller2", "baller3", "cavalcade", "fq2", "granger", "gresley", "habanero", "huntley", "mesa", "radi", "rebla", "rocoto", "seminole", "serrano", "xls", "asea", "asterope",
            "emporor", "fugitive", "ingot", "intruder", "premier", "primo", "primo2", "regina", "stanier", "stratum", "surge", "tailgater", "washington", "bestiagts", "blista2", "buffalo", "schafter2", "euros",
            "sadler", "bison", "bison2", "bison3", "burrito", "burrito2", "minivan", "minivan2", "paradise", "pony"};

        internal static Random rndm => new Random(DateTime.Now.Millisecond);
        internal static Vector3 SpawnPoint;
        internal static Vehicle SuspectVeh;
        internal static Ped Suspect;
        internal static float VehicleHeading;
        internal static DialogueSystem dialogue;
        internal static string[] suspectModels = KidnappingEvent.suspectModels;
        internal static List<string> suspectSuicide = new List<string>()
        {
            "~b~Officer:~w~ Hello. What is wrong with your car? Also, what is with the weird message that you sent me?",
            "~b~Owner:~w~ What message? I don't know what message you are talking about",
            $"~b~Officer:~w~ Do not play stupid with me. You know what message you sent me: {AnonymousTip.msg}",
            "~r~Suspect:~w~ Oh. thats very odd. You know...you will never save the true victim because the one person who knows is not going to be here anymore."
        };
        internal static List<string> suspectGiveUp = new List<string>()
        {
            "~b~Officer:~w~ Hello. What is wrong with your car? Also, what is with the weird message that you sent me?",
            "~b~Owner:~w~ I am sorry officer. I was forced into sending this to you.",
            "~b~Officer:~w~ By who?",
            "~b~Owner:~w~ I don't know who he is. He just send to give this to you."
        };
        internal static List<string> suspectPursuit = new List<string>()
        {
            "~b~Officer:~w~ Hello. What is wrong with your car? Also, what is with the weird message that you sent me?",
            "~b~Owner:~w~ What message? I don't know what message you are talking about",
            $"~b~Officer:~w~ Do not play stupid with me. You know what message you sent me: {AnonymousTip.msg}",
            "~r~Suspect:~w~ Oh. thats very odd. Catch me if you can!"
        };
        internal static bool FindSpawnPoint()
        {
            Vector3 Spawn = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(650f));
            try
            {
                NativeFunction.Natives.GetClosestVehicleNodeWithHeading(Spawn, out Vector3 nodePosition, out float heading, 1, 3.0f, 0);
                bool success = NativeFunction.Natives.xA0F8A7517A273C05<bool>(Spawn, heading, out Vector3 outPosition);
                if (success)
                {
                    SpawnPoint = outPosition;
                    VehicleHeading = heading;
                    return true;
                }
                else
                {
                    Logger.Warning("FindSpawnPoint() in BrokenDownVehicleEvent.cs","Could not find spawn point. Aborting event");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error("FindSpawnPoint() in BrokenDownVehicleEvent.cs",$"Exception:{e}. Aborting event.");
                return false;
            }
        }
        
        internal static void Spawning()
        {
            int model = rndm.Next(0, VehicleModels.Length);
            SuspectVeh = new Vehicle(VehicleModels[model], SpawnPoint, VehicleHeading);
            SuspectVeh.IsPersistent = true;
            SuspectVeh.IsEngineOn = false;
            SuspectVeh.EngineHealth = 0;
            SuspectVeh.IsDriveable = false;
            SuspectVeh.IsInvincible = true;
            SuspectVeh.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
            Suspect = new Ped(suspectModels[rndm.Next(suspectModels.Length)],Vector3.Zero,69);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVeh,-1);
            if (SuspectVeh.Exists())
            {
                Blip b = new Blip(SuspectVeh);
                b.IsRouteEnabled = true;
                AnonymousTip.AllBlips.Add(b);
            }
        }

        internal static void MainEvent()
        {
                while (MainPlayer.DistanceTo(Suspect) >= 6f) GameFiber.Wait(0);
                try
                {
                    SuspectVeh.GetAttachedBlip().Delete();
                }
                catch { }
                Suspect.Tasks.LeaveVehicle(SuspectVeh, LeaveVehicleFlags.None).WaitForCompletion();
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(Suspect, MainPlayer, -1);
                Scenario_GiveUp();
                switch (rndm.Next(0, 3))
                {
                    case 0:
                        dialogue = new DialogueSystem(suspectSuicide, Scenario_Suicide);
                        break;
                    case 1:
                        dialogue = new DialogueSystem(suspectGiveUp,Scenario_GiveUp);
                        break;
                    case 2:
                        dialogue = new DialogueSystem(suspectPursuit,Scenario_Pursuit);
                        break;
                }
                WaitForDialogue();
                dialogue.Run();
        }

        
        internal static void WaitForDialogue()
        {
            while (dialogue.index != dialogue.Dialogue.Count - 1)
            {
                GameFiber.Yield();
                if (Game.IsKeyDown(Keys.Y))
                {
                    dialogue.AdvanceDialogue();
                }

                if (Game.IsKeyDown(Keys.R))
                {
                    dialogue.RewindDialogue();   
                }
            }
        }

        internal static void Scenario_Suicide()
        {
            Suspect.Tasks.PlayAnimation(new AnimationDictionary("mp_suicide"), "pill", 5f, AnimationFlags.None);
            GameFiber.Wait(2500);
            Suspect.Kill();
        }

        internal static void Scenario_Pursuit()
        {
            SuspectVeh.IsEngineOn = true;
            SuspectVeh.EngineHealth = 1000;
            SuspectVeh.IsDriveable = true;
            SuspectVeh.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Off;
            Suspect.Tasks.EnterVehicle(SuspectVeh, -1);
            //maybe shoot their tires
            LHandle pursuit = HelperMethods.CreatePursuit(false, Suspect);
            while (KidnappingEvent.IsPursuitRunning(pursuit)) { GameFiber.Wait(0);}
        }

        internal static void Scenario_GiveUp()
        {
            Suspect.Tasks.PutHandsUp(5000, MainPlayer);
            Game.DisplaySubtitle("~r~Suspect:~w~ I give up. The person who told me to do this gave me this letter.");
            Suspect.Tasks.PlayAnimation("mp_common", "givetake1_b", 1f, AnimationFlags.Loop);
            GameFiber.Wait(1000);
            Rage.Object Mail = new Rage.Object("prop_cs_envolope_01", Vector3.Zero);
            Mail.IsPersistent = true;
            Mail.AttachTo(MainPlayer, MainPlayer.GetBoneIndex(PedBoneId.LeftHand), new Vector3(0.1490f, 0.0560f, -0.0100f), new Rotator(-17f, -142f, -151f));
            GameFiber.Wait(1000);
            Suspect.Tasks.Clear();
            PsychologyReport x = new PsychologyReport();
            ScaleformHandler.Start(x);
            while(!ScaleformHandler.GetActiveScaleFormStatus){GameFiber.Wait(0);}
        }
        

    }
}