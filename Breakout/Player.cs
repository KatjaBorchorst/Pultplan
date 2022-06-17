using DIKUArcade.Entities;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using DIKUArcade.Events;
using DIKUArcade.Timers;
using System.Collections.Generic;
using System;
using Breakout.LevelLoading;

namespace Breakout {
    public class Player : Entity, IGameEventProcessor {
        private float moveLeft = 0.0f;
        private float moveRight = 0.0f;
        private float MOVEMENT_SPEED = 0.02f;
        public EntityContainer<PlayerHealth> playerHealth = default!;
        public Points points = new Points (
            new DynamicShape (new Vec2F(0.0f, 0.02f), new Vec2F (0.2f, 0.05f)),
            new Image (Path.Combine("..", "Breakout", "Assets", "Images", "emptyPoint.png")));
        public int numberOfLives = 0;
        private bool iswide = false;
        private bool isQuick = false;
        public bool isInvincible = false;
        private Entity Invincibility = default!;
        private int? timeStampWide = null;
        private int? timeStampQuick = null;
        private int? timeStampInvincible = null;

        /// <summary>
        /// The constructor of the player class.
        /// </summary>
        /// <param name="shape"> The shape of the player. </param>
        /// <param name="image"> The image/icon of the player. </param>
        public Player(DynamicShape shape, IBaseImage image)
        : base(shape, image){
            BreakoutBus.GetBus().Subscribe(GameEventType.PlayerEvent, this);
            BreakoutBus.GetBus().Subscribe(GameEventType.StatusEvent, this);
            playerHealth = new EntityContainer<PlayerHealth> ();
            AddLife(3, 0); //adds three lives to the container
        }

        /// <summary>
        /// Renders the entities of the game.
        /// </summary>
        public void Render() {
            this.RenderEntity();
            playerHealth.RenderEntities();
            points.RenderPoints();
            if (isInvincible) {
                Invincibility.RenderEntity();
            }
        }   

        /// <summary>
        /// Sequentially keeps track of whether the timed powerups have elapsed.
        /// </summary>
        public void Update() {
            var timelimitWide = 10; //for maintainability
            var timelimitSpeed = 15;
            var timelimitInvincible = 15;
            var newTime = Math.Round(StaticTimer.GetElapsedSeconds());
            if (iswide) {
                if (newTime-timelimitWide >= timeStampWide) {
                    PlayerNormalSize();
                }
            }
            if (isQuick) {
                if (newTime-timelimitSpeed >= timeStampQuick) {
                    PlayerNormalSpeed();
                }
            }
            if (isInvincible) {
                if (newTime-timelimitInvincible >= timeStampInvincible) {
                    PlayerNormalInvincible();
                }
            }
            Move();
        }

        /// <summary>
        /// Moves the shape inside the margins. 
        /// </summary>
        public void Move () {
            this.Shape.Move ();
            if (this.Shape.Position.X > 1.0f - + this.Shape.Extent.X) {
                this.Shape.Position.X = 1.0f -+ this.Shape.Extent.X;
            } else if (this.Shape.Position.X < 0.0f) {
                this.Shape.Position.X = 0.0f;
            }
        }

        /// <summary>
        /// Adds lives to the playerHealth entitycontainer.
        /// </summary>
        /// <param name="amountToAdd"> The amount of lives to add. Will usually be 3 to begin with
        /// and 1 later on. </param>
        /// <param name="amountVisible"> Amount of hearts currently visible</param>
        public void AddLife(int amountToAdd, int amountVisible) {
            var imagewidth = 0.05f; //for maintainability
            for (int i = 1; i < amountToAdd+1; i++) {
                if (numberOfLives < 5) { //global maximum of lives
                    playerHealth.AddEntity (new PlayerHealth (new StationaryShape ( 
                    new Vec2F (1.0f - (float) (amountVisible+i) * imagewidth, 0.01f), 
                    new Vec2F(imagewidth, imagewidth)), 
                    new Image(Path.Combine("..", "Breakout", "Assets", "Images", "heart_filled.png"))));
                    numberOfLives ++;
                }
            }
        }

        /// <summary>
        /// Makes the player invincible and initializes a lock to be rendered above of the lives.
        /// </summary>
        public void PlayerInvincible() {
            if (isInvincible == false) {
                isInvincible = true;
                timeStampInvincible= (int) Math.Round(StaticTimer.GetElapsedSeconds());
                Invincibility = new Entity (new StationaryShape (
                    new Vec2F(0.95f, 0.06f),
                    new Vec2F(0.05f, 0.05f)),
                    new Image(Path.Combine("Assets", "Images", "lock.png"))
                );
            } else {
                timeStampInvincible = (int) Math.Round(StaticTimer.GetElapsedSeconds());;
            }
        }

        /// <summary>
        /// Doubles the player's speed.
        /// </summary>
        public void PlayerQuick() {
            if (isQuick == false) {
                MOVEMENT_SPEED *= 2.0f;
                timeStampQuick = (int) Math.Round(StaticTimer.GetElapsedSeconds());
                isQuick = true;
            } else {
                timeStampQuick = (int) Math.Round(StaticTimer.GetElapsedSeconds());;
            }
        }

        /// <summary>
        /// Doubles the player's width.
        /// </summary>
        public void PlayerWide() {
            if (iswide == false) {
                this.Shape.ScaleX(2.0f);
                timeStampWide = (int) Math.Round(StaticTimer.GetElapsedSeconds());
                iswide = true;
            } else {
                timeStampWide = (int) Math.Round(StaticTimer.GetElapsedSeconds()); //restart time 
            }                                                          
        }                                                              

        /// <summary>
        /// Sets the player's size back to normal.
        /// </summary>
        public void PlayerNormalSize() {
            this.Shape.ScaleX(0.5f);
            iswide = false;
            timeStampWide = null;
        }

        /// <summary>
        /// Sets the player's speed back to normal.
        /// </summary>
        public void PlayerNormalSpeed() {
            MOVEMENT_SPEED *= 0.5f;
            isQuick = false;
            timeStampQuick = null;
        }

        /// <summary>
        /// Sets the player back to being vincible.
        /// </summary>
        public void PlayerNormalInvincible() {
            isInvincible = false;
            timeStampInvincible = null;
            Invincibility.DeleteEntity();
        }

        /// <summary>
        /// Checks if the player has lives left.
        /// </summary>
        /// <returns>True if the player has no lives else false. </returns>
        public bool IsPlayerDead() {
            var count = 0;
            foreach (PlayerHealth health in playerHealth){
                if (health.IsTheLifeLost()) {
                    count++;
                }
            }
            if (playerHealth.CountEntities() == count){
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Moves the shape left.
        /// </summary>
        /// <param name="val"> True or false depending whether the shape should be moved. </param>
        public void SetMoveLeft (bool val) {
            if (val) {
                moveLeft = -MOVEMENT_SPEED;
            } else {
                moveLeft = 0.0f;
            }
            UpdateDirection ();
        }
        
        /// <summary>
        /// Moves the shape right.
        /// </summary>
        /// <param name="val"> True or false dependig whether the shape should be moved. </param>
        public void SetMoveRight (bool val) {
            if (val) {
                moveRight = MOVEMENT_SPEED;
            } else {
                moveRight = 0.0f;
            }
            UpdateDirection ();
        }
        
        /// <summary>
        /// Updates the direction of the shape.
        /// </summary>
        private void UpdateDirection () {
            this.Shape.AsDynamicShape().Direction.X = moveLeft + moveRight;
        }
        /// <summary>
        /// Gets the postion of the shape as a vector.
        /// </summary>
        /// <returns> Returns the position of the shape as a vector. </returns>
        public Vec2F GetPosition () {
            return new Vec2F (this.Shape.Position.X, this.Shape.Position.Y);
        }

        /// <summary>
        /// The method for processing the different events registered in the Key-methods.
        /// </summary>
        /// <param name="gameEvent"> GameEvent to be processed. </param>
        public void ProcessEvent (GameEvent gameEvent) {  
            if (gameEvent.EventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message) { //moves player
                    case "MOVE_LEFT_PRESS":
                        SetMoveLeft (true);
                        break;

                    case "MOVE_LEFT_RELEASE":
                        SetMoveLeft (false);
                        break;

                    case "MOVE_RIGHT_PRESS":
                        SetMoveRight (true);
                        break;

                    case "MOVE_RIGHT_RELEASE":
                        SetMoveRight (false);
                        break;

                    default:
                        break;
                    }
            } else if (gameEvent.EventType == GameEventType.StatusEvent) {
                if (gameEvent.From is Level)
                    switch (gameEvent.Message) {
                        case "WIDE":
                            PlayerWide();
                            break;
                        
                        case "EXTRA_LIFE":
                            AddLife(1, numberOfLives);
                            break;
                        
                        case "QUICK":
                            PlayerQuick();
                            break;

                        case "INVINCIBLE":
                            PlayerInvincible();
                            break;

                        default:
                        break;
                } else if (gameEvent.From is Ball) { //adds points
                    points.AddPoints(Int32.Parse(gameEvent.Message));
                }
            }
        }
    }
}
