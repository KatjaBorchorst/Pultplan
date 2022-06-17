using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using System.Collections.Generic;
using DIKUArcade.Utilities;
using DIKUArcade.Events;

namespace Breakout {


    public class PowerUp : Entity {
        public bool collided {private set; get;}
        public string name;

        public PowerUp (DynamicShape shape, IBaseImage image, string Name)
        : base(shape, image) {
            collided = false;
            name = Name;
        }

        /// <summary>
        /// Renders the powerup.
        /// </summary>
        public void Render() {
            this.RenderEntity();
        }

        /// <summary>
        /// Sets collided to true.
        /// </summary>
        public void Collide() {
            collided = true;
        }

        /// <summary>
        /// Chekcs whether the entity is outside the window and deletes it if so.
        /// </summary>
        public void CheckDeletion() {
            if (this.Shape.Position.Y < 0.0f) { 
                this.DeleteEntity();
            }
        }
    }
}