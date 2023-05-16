using System.Collections.Generic;
using Rage;

namespace MysteriousCallouts.HelperSystems.Scaleforms
{
    internal static class ScaleformHandler
    {
        private static Dictionary<Movie, GameFiber> tests = new Dictionary<Movie, GameFiber> { };
        internal static bool IsThereAnActiveScaleForm = false;
        internal static void Start(Movie x)
        {
            Stop(x);
            GameFiber f = GameFiber.StartNew(delegate
            {
                x.LoadAndWait();
                Game.DisplayNotification("Started ~y~" + x.MovieName);
                x.TestStart();
                IsThereAnActiveScaleForm = true;
                while (true)
                {
                    GameFiber.Yield();
                    x.TestTick();
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) break;
                }
                x.TestEnd();
                x.Release();
                IsThereAnActiveScaleForm = false;
            }, $"Scaleform {x.MovieName}");
            tests[x] = f;
        }
        internal static void Stop(Movie x)
        {
            if (!tests.ContainsKey(x)) return;
            if (tests[x] != null && tests[x].IsAlive)
            {
                tests[x].Abort();
                x.TestEnd();
                x.Release();
                IsThereAnActiveScaleForm = false;
            }
            tests.Remove(x);
        }

        internal static bool GetActiveScaleFormStatus => IsThereAnActiveScaleForm;
    }
}