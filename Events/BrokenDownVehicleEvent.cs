using System;
using MysteriousCallouts.Callouts;
using MysteriousCallouts.HelperSystems;
using Rage;
using Rage.Native;
namespace MysteriousCallouts.Events
{
    internal static class BrokenDownVehicleEvent
    {
        internal static Ped Player => Game.LocalPlayer.Character;
        internal static string[] VehicleModels = new string[] {"asbo", "blista", "dilettante", "panto", "prairie", "cogcabrio", "exemplar", "f620", "felon", "felon2", "jackal", "oracle", "oracle2", "sentinel", "sentinel2",
            "zion", "zion2", "baller", "baller2", "baller3", "cavalcade", "fq2", "granger", "gresley", "habanero", "huntley", "mesa", "radi", "rebla", "rocoto", "seminole", "serrano", "xls", "asea", "asterope",
            "emporor", "fugitive", "ingot", "intruder", "premier", "primo", "primo2", "regina", "stanier", "stratum", "surge", "tailgater", "washington", "bestiagts", "blista2", "buffalo", "schafter2", "euros",
            "sadler", "bison", "bison2", "bison3", "burrito", "burrito2", "minivan", "minivan2", "paradise", "pony"};

        internal static Random rndm = new Random(DateTime.Now.Millisecond);
        internal static Vector3 SpawnPoint;
        internal static Vehicle SuspectVeh;
        internal static Ped Suspect;
        internal static float VehicleHeading;
        internal static string[] suspectModels = KidnappingEvent.suspectModels;
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
            Suspect.WarpIntoVehicle(SuspectVeh,-1);
        }

        internal static void MainEvent()
        {
            GameFiber.StartNew(delegate
            {
                while (Player.DistanceTo(Suspect) >= 30f) GameFiber.Wait(0);
                Suspect.Tasks.LeaveVehicle(SuspectVeh, LeaveVehicleFlags.None).WaitForCompletion();
                NativeFunction.Natives.TASK_TURN_PED_TO_FACE_ENTITY(Suspect, Player, -1);
                    
                
                
            });
        }
        
        
    }
}