using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using LSPD_First_Response.Mod.API;
using MysteriousCallouts.Callouts;
using MysteriousCallouts.HelperSystems;
using Rage;

namespace MysteriousCallouts
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }
        static void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
                GameFiber.StartNew(delegate
                {
                    Game.AddConsoleCommands();
                    RegisterCallouts();
                    Game.Console.Print();
                    Game.Console.Print("=============================================== MysteriousCallouts by Rohit685 ================================================");
                    Game.Console.Print();
                    Game.Console.Print("[LOG]: Callouts were loaded successfully.");
                    Game.Console.Print();
                    Game.Console.Print("=============================================== MysteriousCallouts by Rohit685 ================================================");
                    Game.Console.Print();

                    // You can find all textures/images in OpenIV
                    Game.DisplayNotification("MysteriousCallouts Successfully Loaded");
                    //PluginCheck.isUpdateAvailable();
                    //GameFiber.Wait(300);
                });
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.AnonymousTip));
        }
        
        public override void Finally()
        {
            Logger.Normal("Finally() in Main.cs","MysteriousCallouts has been cleaned up");
        }
    }
}