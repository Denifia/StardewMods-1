﻿using System;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Machines.Objects
{
    /// <summary>A bee house that accepts input and provides output.</summary>
    internal class BeeHouseMachine : GenericMachine
    {
        /*********
        ** Properties
        *********/
        /// <summary>The location containing the machine.</summary>
        private readonly GameLocation Location;

        /// <summary>The machine's position in its location.</summary>
        private readonly Vector2 Tile;



        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="machine">The underlying machine.</param>
        /// <param name="location">The location containing the machine.</param>
        /// <param name="tile">The machine's position in its location.</param>
        public BeeHouseMachine(SObject machine, GameLocation location, Vector2 tile)
            : base(machine)
        {
            this.Location = location;
            this.Tile = tile;
        }

        /// <summary>Get the machine's processing state.</summary>
        public override MachineState GetState()
        {
            return Game1.currentSeason == "winter"
                ? MachineState.Disabled
                : base.GetState();
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack GetOutput()
        {
            // get raw output
            SObject output = this.Machine.heldObject;
            if (output == null)
                return null;

            // get flower data
#if SDV_1_2
            SObject.HoneyType type = SObject.HoneyType.Wild;
#endif
            string prefix = "Wild";
            int addedPrice = 0;
            if (this.Location is Farm)
            {
                Crop flower = Utility.findCloseFlower(this.Tile);
                if (flower != null)
                {
                    string[] flowerData = Game1.objectInformation[flower.indexOfHarvest].Split('/');
                    prefix = flowerData[0];
                    addedPrice = Convert.ToInt32(flowerData[1]) * 2;
#if SDV_1_2
                    switch (flower.indexOfHarvest)
                    {
                        case 376:
                            type = SObject.HoneyType.Poppy;
                            break;
                        case 591:
                            type = SObject.HoneyType.Tulip;
                            break;
                        case 593:
                            type = SObject.HoneyType.SummerSpangle;
                            break;
                        case 595:
                            type = SObject.HoneyType.FairyRose;
                            break;
                        case 597:
                            type = SObject.HoneyType.BlueJazz;
                            break;
                    }
#endif
                }
            }

            // build object
            SObject result = new SObject(output.parentSheetIndex, output.stack)
            {
                name = $"{prefix} Honey",
                price = output.price + addedPrice,
#if SDV_1_2
                honeyType = type
#endif
            };

            // yield
            return new TrackedItem(result, onEmpty: this.Reset);
        }

        /// <summary>Pull items from the connected pipes.</summary>
        /// <param name="pipes">The connected IO pipes.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool Pull(IPipe[] pipes)
        {
            return false; // no input needed
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Reset the machine so it's ready to accept a new input.</summary>
        /// <param name="item">The output item that was taken.</param>
        private void Reset(Item item)
        {
            SObject machine = this.Machine;

            machine.heldObject = new SObject(Vector2.Zero, 340, null, false, true, false, false);
            machine.minutesUntilReady = 2400 - Game1.timeOfDay + 4320;
            machine.readyForHarvest = false;
            machine.showNextIndex = false;
        }
    }
}
