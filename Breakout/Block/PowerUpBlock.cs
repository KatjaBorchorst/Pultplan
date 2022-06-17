using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using System.Collections.Generic;
using DIKUArcade.Utilities;
using DIKUArcade.Math;
using DIKUArcade.Timers;
using DIKUArcade.Events;

namespace Breakout.Block {
    /// <summary>
    /// Defines a block of the type PowerUp.
    /// </summary>
    public class PowerUpBlock : Block {
        public new int value {get; private set;} = 10 ;
        public new int health {get; private set;} = 2;
        public IDictionary<string, string> powerups;

        /// <summary>
        /// The constructor of the PowerUp-block class.
        /// /// </summary>
        /// <param name="shape"> The shape of the block. </param>
        /// <param name="image"> The image/icon of the block. </param>
        public PowerUpBlock(StationaryShape shape, IBaseImage image)
        : base(shape, image) {
            powerups = new Dictionary<string, string>(){
                {"LifePickUp.png", "Extra Life"},
                {"WidePowerUp.png", "Wide"},
                {"Time.png", "More Time"},
                {"SpeedPickUp.png", "Quick"},
                {"Invincible.png", "Invincible"}
            };
        }

        /// <summary>
        /// Method for choosing a random PowerUp.
        /// </summary>
        /// <returns> The given PowerUp's filename </returns>
        public string RandomSelectPowerup() {
            string chosenPowerup;
            var randomnumber = RandomGenerator.Generator.Next(powerups.Count());
            chosenPowerup = powerups.ElementAt(randomnumber).Key;
            return chosenPowerup;
        }

        /// <summary>
        /// Method for hitting a Powerup-block.
        /// </summary>
        public override void HittingBlock() { 
            health--;
            if (health <= 0) {
                var randomPowerUp = RandomSelectPowerup();
                BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                    EventType = GameEventType.StatusEvent, 
                    From = this,
                    StringArg1 = randomPowerUp, //filename
                    StringArg2 = powerups[randomPowerUp], //name
                    ObjectArg1 = new DynamicShape(new Vec2F (this.Shape.Position.X, //xpos
                                                this.Shape.Position.Y), //ypos
                                                new Vec2F (0.083f, 0.04f), //extent
                                                new Vec2F (0.00005f, -0.015f)) //direction
                                                });
                this.DeleteEntity();
            }
        }
    }
}



