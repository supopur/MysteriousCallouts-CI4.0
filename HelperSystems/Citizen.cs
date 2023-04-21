using Rage;
using LSPD_First_Response.Mod.API;
using System;
using System.Collections.Generic;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace MysteriousCallouts
{
    class Citizen : Ped
    {
        //PED RELATED
        public string FullName { get; private set; }
        public string TimesStopped { get; private set; }

        public WantedInformation WantedInformation { get; private set; }
        public string Forename { get; private set; }
        public bool Wanted { get; private set; }
      
        public bool IsSuspectSuicidal { get; private set; }
        
        public bool WillSuspectFlee { get; private set; }
        public string Gender { get; private set; }
        Random monke = new Random();
        //GENERAL
      
        private Persona pedPersona;
        Random rndm = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// constructors..you know how those work catYes
        /// </summary>
        /// <param name="ped"></param>
        /// 

        public Citizen() : base()
        {
            pedPersona = Functions.GetPersonaForPed(this);
            FullName = pedPersona.FullName;
            Forename = pedPersona.Forename;
            TimesStopped = pedPersona.TimesStopped.ToString();
            Wanted = pedPersona.Wanted;
            WantedInformation = pedPersona.WantedInformation;
            Gender = pedPersona.Gender.ToString();
            IsSuspectSuicidal = this.IsMale ? rndm.Next(1, 101) <= 60 : rndm.Next(1, 101) <= 30;
            WillSuspectFlee = rndm.Next(1, 101) <= 50;
        }
        public Citizen(Model modelName, Vector3 spawnPoint) : base(spawnPoint, modelName)
        {
            pedPersona = Functions.GetPersonaForPed(this);
            FullName = pedPersona.FullName;
            Forename = pedPersona.Forename;
            TimesStopped = pedPersona.TimesStopped.ToString();
            Wanted = pedPersona.Wanted;
            WantedInformation = pedPersona.WantedInformation;
            Gender = pedPersona.Gender.ToString();
            IsSuspectSuicidal = this.IsMale ? rndm.Next(1, 101) <= 60 : rndm.Next(1, 101) <= 30;
            WillSuspectFlee = rndm.Next(1, 101) <= 50;
        }
        public Citizen(Vector3 spawnPoint, float heading) : base(spawnPoint, heading)
        {
            pedPersona = Functions.GetPersonaForPed(this);
            FullName = pedPersona.FullName;
            Forename = pedPersona.Forename;
            TimesStopped = pedPersona.TimesStopped.ToString();
            Wanted = pedPersona.Wanted;
            WantedInformation = pedPersona.WantedInformation;
            Gender = pedPersona.Gender.ToString();
            IsSuspectSuicidal = this.IsMale ? rndm.Next(1, 101) <= 60 : rndm.Next(1, 101) <= 30;
            WillSuspectFlee = rndm.Next(1, 101) <= 50;
        }
        public Citizen(Vector3 spawnPoint, Model modelName, float heading) : base(modelName, spawnPoint, heading)
        {
            pedPersona = Functions.GetPersonaForPed(this);
            FullName = pedPersona.FullName;
            Forename = pedPersona.Forename;
            TimesStopped = pedPersona.TimesStopped.ToString();
            Wanted = pedPersona.Wanted;
            WantedInformation = pedPersona.WantedInformation;
            Gender = pedPersona.Gender.ToString();
            IsSuspectSuicidal = this.IsMale ? rndm.Next(1, 101) <= 60 : rndm.Next(1, 101) <= 30;
            WillSuspectFlee = rndm.Next(1, 101) <= 50;
        }
        public Citizen(Vector3 spawnPoint) : base(spawnPoint)
        {
            pedPersona = Functions.GetPersonaForPed(this);
            FullName = pedPersona.FullName;
            Forename = pedPersona.Forename;
            TimesStopped = pedPersona.TimesStopped.ToString();
            Wanted = pedPersona.Wanted;
            WantedInformation = pedPersona.WantedInformation;
            Gender = pedPersona.Gender.ToString();
            IsSuspectSuicidal = this.IsMale ? rndm.Next(1, 101) <= 60 : rndm.Next(1, 101) <= 30;
            WillSuspectFlee = rndm.Next(1, 101) <= 50;
        }
        


    }
}