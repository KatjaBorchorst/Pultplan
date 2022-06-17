using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Physics;
using System.Collections.Generic;

namespace Breakout.Block {
    /// <summary>
    /// Defines a block of the type unbreakable.
    /// </summary>
    public class UnbreakableBlock : Block {
        /// <summary>
        /// The constructor of the unbreakable-block class.
        /// </summary>
        /// <param name="shape"> The shape of the block. </param>
        /// <param name="image"> The image/icon of the block. </param>
        public UnbreakableBlock(StationaryShape shape, IBaseImage image)
        : base(shape, image) {
        }

        /// <summary>
        /// Method for hitting an unbreakable-block.
        /// </summary>
        public override void HittingBlock() { //nothing happens when an unbreakable block is hit.
        }
    }
}



