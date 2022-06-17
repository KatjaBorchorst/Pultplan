using DIKUArcade.Entities;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using DIKUArcade.Events;
using DIKUArcade.Physics;
using System.Collections.Generic;
using Breakout.Block;

namespace Breakout {
    /// <summary>
    /// Defines a ball and it's movement.
    /// </summary>
    public class Ball : Entity {
        private Player thePlayer = default!;
        private EntityContainer<Block.Block> theBlocks = default!;
        
        /// <summary>
        /// The constructor of the ball class.
        /// </summary>
        /// <param name="shape"> The shape of the ball. </param>
        /// <param name="image"> The image/icon of the ball. </param>
        /// <param name="blocks"> The entity-container with blocks. </param>
        /// <param name="player"> The player in the game. </param>
        /// <returns> The shape and image of the ball. </returns>
        public Ball (DynamicShape shape, IBaseImage image, EntityContainer<Block.Block> blocks,
        Player player) : base(shape, image){
            theBlocks = blocks;
            thePlayer = player;
        } 
        
        /// <summary>
        /// Moves the ball within the screen.
        /// </summary>
        public void Move() {
            CollideWithPlayer();
            CollideWithBlock();

            this.Shape.Move();
            if (this.Shape.Position.X > 1.0f - this.Shape.Extent.X) { //right side of the screen.
                this.Shape.Position.X = 1.0f - this.Shape.Extent.X;
                this.Shape.AsDynamicShape().Direction.X *= -1.0f;
            } else if (this.Shape.Position.X < 0.0f) { //left side of the screen.
                this.Shape.Position.X = 0.0f;
                this.Shape.AsDynamicShape().Direction.X *= -1.0f;
            } else if (this.Shape.Position.Y > 1.0f - this.Shape.Extent.Y) { //top of the screen.
                this.Shape.AsDynamicShape().Direction.Y *= -1.0f;
            }
        }

        /// <summary>
        /// Checks whether the ball has exited the screen at the bottom.
        /// </summary>
        /// <returns></returns>
        public bool IsBallDead() {
            if (this.Shape.Position.Y <= 0.018f) {//bottom of the screen.
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Renders the ball.
        /// </summary>
        public void Render() {
            this.RenderEntity();
        }  

        /// <summary>
        /// Makes the ball change direction when colliding with the player.
        /// </summary>
        public void CollideWithPlayer() {
            float playerpos = thePlayer.GetPosition().X;
            float ballpos = this.Shape.Position.X + (this.Shape.Extent.X/2.0f);
            float playerextent = thePlayer.Shape.Extent.X;

            //a variable for any collision between the ball and player.
            var aaBB = CollisionDetection.Aabb(this.Shape.AsDynamicShape(), 
            thePlayer.Shape.AsStationaryShape());

            if (aaBB.Collision) {
                if (playerpos <= ballpos && ballpos <= (playerpos + (playerextent/4.0f))) { 
                    //ball hits left quarter of player
                    this.Shape.AsDynamicShape().Direction = new Vec2F(-0.005f, 0.015f);
                } else if (playerpos + (3.0f/4.0f * playerextent) <= ballpos && ballpos <=
                                                                      playerpos + playerextent) { 
                    //ball hits right quarter of player
                    this.Shape.AsDynamicShape().Direction = new Vec2F(0.005f, 0.015f);
                } else {
                    ///ball hits the player generally.
                    this.Shape.AsDynamicShape().Direction.Y *= -1.0f;
                }
            }
        }

        /// <summary>
        /// Collision between a ball and block.
        /// </summary>
        public void CollideWithBlock() {
            theBlocks.Iterate(block => { //iterates through all blocks in the level.
                //a variable for any collision between the ball and blocks.
                var aaBB = CollisionDetection.Aabb(this.Shape.AsDynamicShape(), block.Shape);
                if (aaBB.Collision) {
                    if (aaBB.CollisionDir == CollisionDirection.CollisionDirDown) {
                        block.HittingBlock();
                        this.Shape.AsDynamicShape().Direction.Y *= -1.0f;
                    } else if (aaBB.CollisionDir == CollisionDirection.CollisionDirUp) {
                        block.HittingBlock();
                        this.Shape.AsDynamicShape().Direction.Y *= -1.0f;
                    } else if (aaBB.CollisionDir == CollisionDirection.CollisionDirLeft) {
                        block.HittingBlock();
                        this.Shape.AsDynamicShape().Direction.X *= -1.0f;
                    } else if (aaBB.CollisionDir == CollisionDirection.CollisionDirRight) {
                        block.HittingBlock();
                        this.Shape.AsDynamicShape().Direction.X *= -1.0f;
                    }
                }
                if (block.IsDeleted()) {
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                            EventType = GameEventType.StatusEvent, 
                            From = this,
                            Message = (block.pointValue()).ToString()});
                }
            });
        }
    }
}