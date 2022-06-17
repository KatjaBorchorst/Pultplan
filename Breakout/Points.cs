using System;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using DIKUArcade.Entities;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("BreakoutTest")]

namespace Breakout {
    /// <summary>
    /// Defines the high-score a player has.
    /// </summary>
    public class Points : Entity {
        private int points = 0;
        private Text[] display; //Has to be an array since otherwise text is rendered behind image
        public int value = default!;
        /// <summary>
        /// The constructor of the points class.
        /// </summary>
        /// <param name="shape"> The shape of the points. </param>
        /// <param name="image"> The image/icon of the points. </param>
        public Points(DynamicShape shape, IBaseImage image)
        : base(shape, image) {
            display = new Text[] {new Text(points.ToString(), new Vec2F (0.055f, -0.117f), 
                                                              new Vec2F (0.25f, 0.175f))};
            display[0].SetColor(255, 255, 255, 0);
            display[0].SetFont("Eras Bold ITC");
    }

        /// <summary>
        /// Increases the points.
        /// </summary>
        internal void AddPoints(int Value) {
            points += Value;
            display[0].SetText(points.ToString());
        }

        public int GetPoints() {
            return points;
        }

        /// <summary>
        /// Renders both the image and array of the score.
        /// </summary>
        public void RenderPoints() {
            this.RenderEntity();
            display[0].RenderText();
        } 
    }
}