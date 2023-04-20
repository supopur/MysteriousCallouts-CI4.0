using LSPD_First_Response.Mod.API;
using Rage;

namespace MysteriousCallouts
{
    internal class HelperMethods
    {
        internal static LHandle CreatePursuit(bool IsSuspectsPulledOver, params Ped[] Suspects)
        {
            if (IsSuspectsPulledOver)
            {
                Functions.ForceEndCurrentPullover();
            }
            LHandle PursuitLHandle = Functions.CreatePursuit();

            Functions.SetPursuitIsActiveForPlayer(PursuitLHandle, true);

            foreach (Ped Suspect in Suspects)
            {
                GameFiber.Yield();
                Functions.AddPedToPursuit(PursuitLHandle, Suspect);
            }
            return PursuitLHandle;
        }
        internal static float MphToMps(float speed) => MathHelper.ConvertMilesPerHourToMetersPerSecond(speed);
             
        
    }
}