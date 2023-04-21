using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using LSPD_First_Response.Mod.API;
using MysteriousCallouts.HelperSystems;
using Rage;
using Rage.Native;
using MysteriousCallouts.Callouts;

namespace MysteriousCallouts.Events
{
    internal static class KidnappingEvent
    {
        internal static Citizen Suspect;
        internal static Vehicle SuspectVehicle;
        internal static Citizen Hostage;
        internal static Ped MainPlayer => Game.LocalPlayer.Character;

        internal static string[] AnonymousTips = new string[]
        {
            "The butterfly has flown into the web. Free the butterfly.",
            "The snake has coiled around its prey. Protect the prey.",
            "Check. The king is in danger. Prevent his death.",
            "The snake has coiled around its prey. Protect the prey.",
            "The cat has caught the mouse. Free the mouse.",
            "The rat fell into the trap. Free the rat.",
            "The crows are waiting. Disperse the crows."
        };

        internal static string[] PhoneNumbers = new string[]
        {
            "213-599-4947",
            "213-947-4558",
            "213-645-4407",
            "213-944-4035",
            "213-683-4444",
            "213-383-5688",
            "213-942-8017",
            "213-258-3446",
            
        };

        internal static string[] vehicleModels = new string[]
        {
            "ASTEROPE", "PREMIER", "TAILGATER", "WASHINGTON", "BUFFALO", "CALICO", "KURUMA",
            "SULTAN", "SPEEDO","SPEEDO4","YOUGA3","SURFER","RUMPO","SCHWARZER","SADLER","SADLER2","REBEL2","SEMINOLE","DILETTANTE","RANCHERXL","MINIVAN","CAVALCADE2"
        };

        internal static string[] suspectModels = new string[]
        {
            "g_m_y_mexgoon_01",
            "g_m_y_armgoon_02",
            "g_m_y_lost_01",
            "g_m_y_lost_03",
            "g_m_y_lost_02",
            "g_m_y_pologoon_01",
            "g_m_y_pologoon_02",
            "g_f_y_lost_01",
            "g_f_y_vagos_01",
            "g_f_y_families_0",
            "g_f_importexport_01"
        };

        internal static string[] hostageModels = new string[]
        {
            "a_m_m_prolhost_01",
            "ig_fbisuit_01",
            "a_m_y_business_01",
            "a_m_y_business_02",
            "a_m_y_business_03",
            "a_f_y_bevhills_04",
            "a_f_y_bevhills_03",
            "a_f_y_bevhills_02",
            "a_f_y_bevhills_01",
            "a_f_y_business_01",
            "a_f_y_business_02",
            "a_f_y_business_03",
            "a_f_y_business_04",
            "a_m_m_hasjew_01"
        };
        internal static string[] weapons = //Weapons suspects can use
        {
            "WEAPON_PISTOL",
            "WEAPON_COMBATPISTOL",
            "WEAPON_APPISTOL",
            "WEAPON_PISTOL_MK2",
            "WEAPON_HEAVYPISTOL",
            "WEAPON_SNSPISTOL_MK2",
            "WEAPON_SMG",
            "WEAPON_SMG_MK2",
            "WEAPON_MICROSMG",
            "WEAPON_COMPACTRIFLE",
        };
        
        internal static Random rndm = new Random(DateTime.Now.Millisecond);

        internal static void SetupVehicleWithHostage()
        {
            Vector3 spawn = World.GetNextPositionOnStreet(MainPlayer.Position.Around(1350f));
            NativeFunction.Natives.GetClosestVehicleNodeWithHeading(spawn, out Vector3 nodePosition,
                out float outheading, 1, 3.0f, 0);
            SuspectVehicle = new Vehicle(vehicleModels[rndm.Next(vehicleModels.Length)], spawn, outheading);
            SuspectVehicle.IsPersistent = true;
            Logger.Normal("SetupVehicleWithHostage() in KidnappingEvent.cs",$"SuspectVehicle model: {SuspectVehicle.Model.Name.ToString()}");
            Suspect = new Citizen(suspectModels[rndm.Next(suspectModels.Length)], Vector3.Zero);
            int num = rndm.Next(1, 101);
            if (num <= 50)
            {
                Suspect.Inventory.Weapons.Clear();
                Suspect.Inventory.GiveNewWeapon(weapons[rndm.Next(weapons.Length)], -1, true);
                Logger.Normal("SetupVehicleWithHostage() in KidnappingEvent.cs",$"Giving suspect gun");
            }
            Hostage = new Citizen(hostageModels[rndm.Next(hostageModels.Length)], Vector3.Zero);
            Suspect.BlockPermanentEvents = true;
            Hostage.BlockPermanentEvents = true;
            Suspect.IsPersistent = true;
            Hostage.IsPersistent = true;
            Hostage.WarpIntoVehicle(SuspectVehicle, 0);
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Tasks.CruiseWithVehicle(SuspectVehicle, 10f,
                VehicleDrivingFlags.Emergency | VehicleDrivingFlags.AllowMedianCrossing |
                VehicleDrivingFlags.RespectIntersections | VehicleDrivingFlags.AvoidHighways);
            if (SuspectVehicle.Exists())
            {
                Blip b = new Blip(SuspectVehicle);
                b.IsRouteEnabled = true;
                AnonymousTip.AllBlips.Add(b);
            }
            AnonymousTip.AllPeds.Add(Suspect);
            AnonymousTip.AllPeds.Add(Hostage);
            Logger.Normal("SetupVehicleWithHostage() in KidnappingEvent.cs","Waiting for player to be in range");
            //while(Player.DistanceTo(SuspectVehicle.Position) >= 10f) {GameFiber.Wait(0);}
            string chosenScenario = ScenarioChooser();
            while(!IsSuspectPulledOver()) {GameFiber.Wait(0);}
            RunScenario(chosenScenario);
        }

        internal static string ScenarioChooser()
        {
            
            DecisionNode suicideFastCarDecisionNode = new AttributeNode("Suicidal", true,
                new DecisionLeaf("commit"), new DecisionLeaf("surrender"));
            
            DecisionNode ShouldFleeOnFootNode = new AttributeNode("ShouldFlee", "true",
                new DecisionLeaf("foot_bail"), new DecisionLeaf("surrender"));
            
            DecisionNode fastArmedNode =
                new AttributeNode("Armed", true, suicideFastCarDecisionNode, new DecisionLeaf("surrender"));
            
            DecisionNode fastVehicleNode = new AttributeNode("ShouldFlee", true,new DecisionLeaf("pursuit"),fastArmedNode);
            
            DecisionNode suicideDecisionNode = new AttributeNode("Suicidal", true,
                new DecisionLeaf("commit"), ShouldFleeOnFootNode);

            DecisionNode ArmedNode =
                new AttributeNode("Armed", true, suicideDecisionNode, ShouldFleeOnFootNode);

            DecisionNode isVehicleSlowNode = new AttributeNode("VehicleType", true, ArmedNode,
                fastVehicleNode);
            
            Dictionary<string, object> inputs = new Dictionary<string, object>()
            {
                {"Armed",IsSuspectArmed()},
                {"VehicleType", IsVehicleSlow()},
                {"Suicidal",Suspect.IsSuspectSuicidal},
                {"ShouldFlee",Suspect.WillSuspectFlee}
            };
            string decision = isVehicleSlowNode.Evaluate(inputs);
            Logger.Normal("ScenarioChooser() in KidnappingEvent.cs",$"Decision: {IPHelper.Encrypt(decision)}");
            return decision;
        }

        internal static void RunScenario(string chosenScenario)
        {
            switch (chosenScenario)
            {
                case "shootout":
                    Scenario_Shootout();
                    break;
                case "pursuit":
                    Scenario_Pursuit();
                    break;
                case "commit":
                    Scenario_Commit();
                    break;
                case "foot_bail":
                    Scenario_FootBail();
                    break;
                default:
                    Scenario_Surrender();
                    break;
            }
        }
        
        internal static void Scenario_Pursuit()
        {
            
            Logger.Normal("Scenario_Pursuit() in KidnappingEvent.cs","Player pulled over Suspect. Starting pursuit");
            foreach (Blip b in AnonymousTip.AllBlips)
            {
                if (b.Entity.Equals(SuspectVehicle))
                {
                    if(b.Exists()) b.Delete();
                    break;
                }
            }
            LHandle Pursuit = HelperMethods.CreatePursuit(true, Suspect);
            while(IsPursuitRunning(Pursuit)) {GameFiber.Wait(0);}
            Logger.Normal("Scenario_Pursuit() in KidnappingEvent.cs","Pursuit is over");
        }
        
        internal static void Scenario_Shootout()
        {
            Logger.Normal("Scenario_Shootout() in KidnappingEvent.cs","Player pulled over Suspect. Starting shootout");
            Functions.ForceEndCurrentPullover();
            Suspect.Tasks.ParkVehicle(SuspectVehicle, SuspectVehicle.Position, SuspectVehicle.Heading).WaitForCompletion();
            Suspect.Tasks.LeaveVehicle(SuspectVehicle, LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion();
            Suspect.RelationshipGroup.SetRelationshipWith(MainPlayer.RelationshipGroup, Relationship.Hate);
            Suspect.RelationshipGroup.SetRelationshipWith(Hostage.RelationshipGroup, Relationship.Hate);
            Suspect.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            Suspect.Tasks.FightAgainstClosestHatedTarget(100f, -1);
            while (!Suspect.IsCuffed && !Suspect.IsDead) { GameFiber.Wait(0); }
            Logger.Normal("Scenario_Shootout() in KidnappingEvent.cs","Shootout Over");
        }

        internal static void Scenario_Commit()
        {
            Logger.Normal("Scenario_Commit() in KidnappingEvent.cs","Player pulled over Suspect. Starting commit");
            Functions.ForceEndCurrentPullover();
            Suspect.Tasks.FireWeaponAt(Hostage, 3, FiringPattern.FullAutomatic).WaitForCompletion();
            Suspect.Tasks.PlayAnimation(new AnimationDictionary("mp_suicide"), "pill", 5f, AnimationFlags.None);
            GameFiber.Wait(2500);
            Suspect.Kill();
            Logger.Normal("Scenario_Commit() in KidnappingEvent.cs","Player pulled over Suspect. Suspect committed");
        }

        internal static void Scenario_FootBail()
        {
            Logger.Normal("Scenario_FootBail() in KidnappingEvent.cs","Player pulled over Suspect. Starting foot bail");
            Functions.ForceEndCurrentPullover();
            Suspect.Tasks.ParkVehicle(SuspectVehicle, SuspectVehicle.Position, SuspectVehicle.Heading).WaitForCompletion();
            Suspect.Tasks.LeaveVehicle(SuspectVehicle, LeaveVehicleFlags.LeaveDoorOpen);
            Hostage.Tasks.LeaveVehicle(SuspectVehicle, LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion();
            Hostage.Tasks.PutHandsUp(-1,MainPlayer);
            Suspect.ClearLastVehicle();
            LHandle Pursuit = HelperMethods.CreatePursuit(false, Suspect);
            while(IsPursuitRunning(Pursuit)) {GameFiber.Wait(0);}
            Logger.Normal("Scenario_FootBail() in KidnappingEvent.cs","Ending foot bail");
        }

        internal static void Scenario_Surrender()
        {
            Logger.Normal("Scenario_Surrender in KidnappingEvent.cs","Player pulled over Suspect. Suspect surrendering");
            Functions.ForceEndCurrentPullover();
            Suspect.Tasks.ParkVehicle(SuspectVehicle, SuspectVehicle.Position, SuspectVehicle.Heading).WaitForCompletion();
            Suspect.Tasks.LeaveVehicle(SuspectVehicle, LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion();
            Suspect.Tasks.PutHandsUp(-1, MainPlayer);
            while (!Suspect.IsCuffed && !Suspect.IsDead) { GameFiber.Wait(0); }
            Logger.Normal("Scenario_Surrender in KidnappingEvent.cs","Ending surrender scenario");
        }

        internal static string GetRandomTip() => AnonymousTips[rndm.Next(AnonymousTips.Length)];

        internal static string GetRandomPhoneNumber() => PhoneNumbers[rndm.Next(PhoneNumbers.Length)];

        internal static bool IsPursuitRunning(LHandle handle) => Functions.IsPursuitStillRunning(handle);
        
        internal static bool IsSuspectArmed() => Suspect.Inventory.EquippedWeapon == null ? true : false;
        internal static bool IsVehicleSlow()
        {
            List<VehicleClass> SlowVehicleClasses = new List<VehicleClass>()
            {
                VehicleClass.Van,
                VehicleClass.Utility,
                VehicleClass.SUV,
                VehicleClass.OffRoad
            };
            return SlowVehicleClasses.Contains(SuspectVehicle.Class);
        }

        internal static bool IsSuspectPulledOver()
        {
            if (Functions.IsPlayerPerformingPullover())
            {
                return Functions.GetPulloverSuspect(Functions.GetCurrentPullover()).Equals(Suspect);
            }
            return false;
        }

    }
}