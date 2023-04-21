using System;
using System.Collections.Generic;
using System.Linq;
using MysteriousCallouts.Callouts;
using Rage;
using Rage.Attributes;

namespace MysteriousCallouts.HelperSystems
{
    internal class IPHelper
    {
        internal static List<string> ListOfIPS = new List<string>()
        {
                "32.174.197.198","76.89.247.234","36.43.35.39","231.166.111.6","218.109.37.22","25.252.71.168","244.12.86.50","188.199.185.203","241.219.83.130","57.20.157.15","210.231.46.133","95.7.54.74","63.128.205.151","132.97.131.150","135.252.66.120","44.19.11.222","156.192.24.220","178.137.14.163","101.222.66.34","20.62.81.40","158.211.129.52","245.85.223.221","244.75.211.28","196.132.107.93","197.214.211.229","168.221.152.192","230.163.175.14","124.219.24.38","38.15.166.130","180.41.135.192","43.124.140.207","24.57.214.107","8.251.19.130","181.14.60.75","164.130.175.210","84.166.185.84","171.62.47.64","210.109.59.65","11.20.196.14","173.134.84.222","253.170.169.150","88.151.173.56","214.179.85.149","248.84.198.251","172.13.66.96","189.88.207.228","221.221.115.251","85.94.140.202","220.69.219.119","131.93.228.94",    
        };

        internal static List<string> UsedIPS = new List<string>();
        internal static string GetEncryptedIP()
        {
            
            return Encrypt(ListOfIPS[new Random().Next(ListOfIPS.Count)]);
        }

        internal static string Encrypt(string originalstring)
        {
            return ToBase64Encode(originalstring);
        }
        [ConsoleCommand]
        private static void Decrypt(string ScrambledIP)
        {
            string IPAddress = ToBase64Decode(ScrambledIP);
            Game.SetClipboardText(IPAddress);
            Logger.Normal("Decrypt() in IPHelper.cs",$"IPAddress: {IPAddress}");
            Game.DisplayNotification($"Successfully decrypted {ScrambledIP}. Returned {IPAddress}");
            Game.DisplayNotification("Saving IP Address to PC clipboard.");
            AnonymousTip.SuccessfulDecryption = true;
        }
        [ConsoleCommand]
        private static void PingIP(string givenIP)
        {
            if (ListOfIPS.Contains(givenIP))
            {
                Game.DisplayNotification("~g~IP Successfully Pinged. ~w~Sending coordinates to your gps.");
                AnonymousTip.SuccessfulIPPing = true;
            }
            else
            {
                Game.DisplayNotification("~r~IP Ping Not Successful. ~y~Try Again.");
                AnonymousTip.SuccessfulIPPing = false;
            }
        }


        internal static void HelpWithDecryption()
        {
            Game.DisplaySubtitle("~y~Stop you car", 2500);
            Game.DisplaySubtitle("~g~Open your RAGE Console. Type in Decrypt. ADD SPACE. Paste in encrypted IP address(automatically copied for you).",3500);
        }

        internal static void HelpWithIPPing()
        {
            Game.DisplaySubtitle("Stop you car", 2500);
            Game.DisplaySubtitle("~g~Open your RAGE Console. Type in PingIP. ADD SPACE. Paste in decrypted IP address(automatically copied for you).",3500);
        }


        internal static string ToBase64Encode(string text)
        {
            if (String.IsNullOrEmpty(text)) {
                return text;
            }
 
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }
 
        internal static string ToBase64Decode(string base64EncodedText)
        {
            if (String.IsNullOrEmpty(base64EncodedText)) {
                return base64EncodedText;
            }
 
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedText);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}