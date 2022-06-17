using DIKUArcade.Entities;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using DIKUArcade.Events;

namespace Breakout {

    public class PlayerHealth : Entity {
        private bool isDead = false;
        
        public PlayerHealth(StationaryShape shape, IBaseImage image) : base(shape, image){
        }

        /// <summary>
        /// Renders the entities of the game.
        /// </summary>
        public void Render() {
            this.RenderEntity();
        }   

        /// <summary>
        /// Checks whether the heart is empty.
        /// </summary>
        /// <returns> True if the life is empty. </returns>
        public bool IsTheLifeLost() {
            return isDead;
        }

        /// <summary>
        /// Changes the image of the heart to empty.
        /// </summary>
        public void LoseLife() {
            isDead = true;
            this.Image = new Image(Path.Combine ("..", "Breakout", "Assets", "Images", "heart_empty.png"));
        }
    }
}
