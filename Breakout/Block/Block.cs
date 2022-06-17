using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using System.Collections.Generic;

namespace Breakout.Block {
    /// <summary>
    /// Defines a block and its methods.
    /// </summary>
    public class Block : Entity {
        public int value {get; private set;} = 10;
        public int health {get; private set;} = 2;

        /// <summary>
        /// The constructor of the block class.
        /// </summary>
        /// <param name="shape"> The shape of a block. </param>
        /// <param name="image"> The image/icon of a block. </param>
        public Block(StationaryShape shape, IBaseImage image)
        : base(shape, image) {
        }

        /// <summary>
        /// Renders the block in the game.
        /// </summary>
        virtual public void Render() {
            this.RenderEntity();
        }

        /// <summary>
        /// Decrements the health of a block and deletes it when it's equal to 0.
        /// </summary>
        virtual public void HittingBlock() {
            health--;
            if (health <= 0) {
                this.DeleteEntity();
            }
        }

        /// <summary>
        /// Returns the points the block is worth.
        /// </summary>
        /// <returns>The amount of points to be rewarded when the block is destroyed</returns>
        virtual public int pointValue() {
            return value;
        }
    }
}