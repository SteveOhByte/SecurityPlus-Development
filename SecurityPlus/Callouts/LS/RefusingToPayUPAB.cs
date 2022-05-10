﻿using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace SecurityPlus.Callouts.LS
{
    [CalloutInfo("RefusingToPayUPAB", CalloutProbability.High)]
    
    public class RefusingToPayUPAB : Callout
    {
        private Vector3 calloutPosition;

        private Ped owner, suspect;

        private Blip oBlip, sBlip;

        private Vehicle susCar;

        private bool convoStarted;

        private int counter;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            Log.log("Starting Callout - RefusingToPayUPAB");
            
            calloutPosition = new Vector3(90.32795f, 299.1501f, 110.2102f);

            CalloutPosition = calloutPosition;
            ShowCalloutAreaBlipBeforeAccepting(calloutPosition, 20f);

            if (Main.distanceCheck)
            {
                AddMinimumDistanceCheck(30f, calloutPosition);
                AddMaximumDistanceCheck(500f, calloutPosition);
            }
            
            Log.log("Callout details assigned");
            
            CalloutMessage = "Consumer refusing to pay";
            CalloutAdvisory = "Security has been called after a consumer has refused to pay for their meal";
            
            Functions.PlayScannerAudio("consumer_refusing_to_pay");
            
            Log.log("Callout being displayed");
            
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Log.log("Callout accepted");
            
            owner = new Ped("s_m_y_waiter_01", calloutPosition, 332.0811f);
            
            suspect = new Ped(new Vector3(92.65351f, 303.0269f, 110.0125f), 138.8516f);

            suspect.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, -1, false);

            susCar = new Vehicle("BALLER", new Vector3(95.64912f, 307.315f, 110.0379f), 162.6415f);
            
            convoStarted = false;
            counter = 0;

            oBlip = new Blip(owner);
            oBlip.IsFriendly = true;
            oBlip.IsRouteEnabled = true;

            sBlip = new Blip(suspect);
            sBlip.IsFriendly = false;
            
            Log.log("Callout setup completed");
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            Log.log("Callout process start");
            
            if (Game.LocalPlayer.Character.DistanceTo(owner) < 10f)
            {
                if (!convoStarted)
                {
                    Game.DisplaySubtitle("Waiter: Over here! Thanks for arriving quickly. This individual ate a LOT of our food, and now is refusing to pay!");
                    Game.DisplayHelp("Press 'Y' to advance conversation");
                    convoStarted = true;
                }

                if (Game.IsKeyDown(Keys.Y))
                {
                    counter++;
                }

                if (counter == 1)
                {
                    Game.DisplaySubtitle(Main.Name + ": Alright, I'll go speak to them now.");
                }

                if (counter == 2)
                {
                    Game.DisplaySubtitle(Main.Name + ": What's going on? The waiter says you're refusing to pay? What gives?");
                }

                if (counter == 3)
                {
                    Game.DisplaySubtitle("Suspect: That food is terrible! I should be the one getting paid for managing to stomach it!");
                }

                if (counter == 4)
                {
                    Game.DisplaySubtitle(Main.Name + ": That's not how this works, and besides, the waiter says you were eating vast portions, if it was really bad why would you do that?");
                }

                if (counter == 5)
                {
                    Game.DisplaySubtitle("Suspect: Oh get lost mall cop, I don't have to explain myself to you!");
                }

                if (counter == 6)
                {
                    suspect.Tasks.EnterVehicle(susCar, -1).WaitForCompletion(10000);
                    if (susCar.Driver == suspect)
                    {
                        suspect.Tasks.CruiseWithVehicle(susCar, 10f, VehicleDrivingFlags.Normal);
                    }
                    else
                    {
                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                    }
                }
            }
            
            if (Game.IsKeyDown(Main.endKey) || Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed)
            {
                End();
            }
            
            Log.log("Callout process end");
        }

        public override void End()
        {
            base.End();
            
            Log.log("Callout end activated");
            
            if (oBlip.Exists())
                oBlip.Delete();
            if (sBlip.Exists())
                sBlip.Delete();
            if (owner.Exists())
                owner.Dismiss();
            if (suspect.Exists())
                suspect.Dismiss();
            if (susCar.Exists())
                susCar.Dismiss();
            
            Log.log("Security+ RefusingToPayUPAB callout has been cleaned up");
            Game.LogTrivial("Security+ RefusingToPayUPAB callout has been cleaned up");
        }
    }
}