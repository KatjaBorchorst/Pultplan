using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using System.Collections.Generic;

namespace Breakout.Block {
    /// <summary>
    /// Defines a block of the typed hardened
    /// </summary>
    public class HardenedBlock : Block {
        public new int value {get; private set;} = 20;
        public new int health {get; private set;} = 4;
        private IBaseImage brokenBlock = default!;
        /// <summary>
        /// The constructor of the hardened-block class.
        /// </summary>
        /// <param name="shape"> The shape of the block. </param>
        /// <param name="image"> The image/icon of the block. </param>
        /// <param name="broken"> The image/icon of a broken block. </param>
        public HardenedBlock(StationaryShape shape, IBaseImage image, IBaseImage broken)
        : base(shape, image) {
            brokenBlock = broken;
        }

        /// <summary>
        /// Decrements the health of a block and deletes it when it's equal to 0.
        /// </summary>
        public override void HittingBlock() {
            health--;
            if (0 < health && health <= 2) {
                Image = brokenBlock;
            } else if (health <= 0) {  
                this.DeleteEntity();
            }
        }

    }
}
