using LSPD_First_Response.Mod.API;
using Rage;
using System;
using MysteriousCallouts.Callouts;
using static MysteriousCallouts.Events.KidnappingEvent;
using STPFunctions = StopThePed.API.Functions;

namespace MysteriousCallouts.Events
{
    internal static class ActiveShooterEvent
    {
        internal static Citizen Suspect;
        internal static Ped MainPlayer => Game.LocalPlayer.Character;
        internal static Random rndm => new Random(DateTime.Now.Millisecond);
        internal static Vector3 SpawnPoint;

        internal static void SetupEvent()
        {
            SpawnPoint = World.GetNextPositionOnStreet(MainPlayer.Position.Around(600f));
            Suspect = new Citizen(suspectModels[rndm.Next(suspectModels.Length)], SpawnPoint);
            Suspect.IsPersistent = true;
            Suspect.Metadata.searchPed = "~y~Letter";
            STPFunctions.injectPedSearchItems(Suspect);
            Suspect.BlockPermanentEvents = true;
            Suspect.Tasks.Wander();
            Suspect.Inventory.GiveNewWeapon(weapons[rndm.Next(weapons.Length)], 200, true);
            Blip b = new Blip(Suspect);
            b.IsRouteEnabled = true;
            AnonymousTip.AllBlips.Add(b);
            Suspect.RelationshipGroup = RelationshipGroup.Gang1;
            Suspect.RelationshipGroup.SetRelationshipWith(Game.LocalPlayer.Character.RelationshipGroup, Relationship.Hate);
            Suspect.RelationshipGroup.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
        }

        internal static void MainEvent()
        {
            while(MainPlayer.DistanceTo(Suspect) >= 225f){GameFiber.Wait(0);}
            Suspect.Tasks.FightAgainstClosestHatedTarget(225f, -1);
            Suspect.IsInvincible = false;
            foreach (Blip b in AnonymousTip.AllBlips)
            {
                if (b.Entity.Equals(Suspect))
                {
                    if(b.Exists()) b.Delete();
                    break;
                }
            }
            while(!Suspect.IsDead) {GameFiber.Wait(0);}
            
        }
    }
}
