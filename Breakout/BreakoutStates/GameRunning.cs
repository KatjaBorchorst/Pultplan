using DIKUArcade;
using DIKUArcade.Math;
using DIKUArcade.Entities;
using DIKUArcade.GUI;
using DIKUArcade.Graphics;
using DIKUArcade.Input;
using DIKUArcade.Events;
using DIKUArcade.Physics;
using System.IO;
using System.Collections.Generic;
using DIKUArcade.Utilities;
using DIKUArcade.State;
using Breakout.Block;
using DIKUArcade.Timers;
using Breakout.LevelLoading;

namespace Breakout.BreakoutStates {
    /// <summary>
    /// Defines the game state GameRunning.
    /// </summary>
    public class GameRunning : IGameState {
        private Player player = default!;
        private Ball ball = default!;
        private ASCIIReader readASCII = default!;
        private static GameRunning instance = default!;
        private int LevelNumber = 1;
        private bool killSuccess = false; //only kill one life

        /// <summary>
        /// Gets the instance of GameRunning.
        /// </summary>
        /// <returns> The instance of GameRunning. </returns>
        public static GameRunning GetInstance() {
            if (instance == null) {
                instance = new GameRunning();
                instance.InitializeGameState("level1.txt");
            }
            return instance;
        }


        /// <summary>
        /// Initializes the game state with images of the player, level and score.
        /// </summary>
        public void InitializeGameState(string level) {
            player = new Player ( 
                new DynamicShape (new Vec2F(0.459f, 0.1f), new Vec2F (0.15f, 0.04f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "player.png")));

            readASCII = new ASCIIReader();
            readASCII.LoadLevel(level);

            ball = new Ball (
                new DynamicShape (new Vec2F(0.5f, 0.15f), new Vec2F (0.05f, 0.05f),
                new Vec2F (0.00005f, 0.015f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "ball.png")),
                readASCII.level.Blocks, player);
            LevelNumber = 1;
            readASCII.level.player = player;
        }

        /// <summary>
        /// Resets the state.
        /// </summary>
        public void ResetState() {
            InitializeGameState(ChooseLevel(1));
        }

        /// <summary>
        /// Updates/moves the different entities of the game
        /// </summary>
        public void UpdateState() { 
            BreakoutBus.GetBus().ProcessEvents();  
            player.Update();
            ball.Move();
            isBallDead();
            IsNextLevel();
            IsGameOver();
            IsGameWon(); 
        }

        /// <summary>
        /// Renders the different entities of the game.
        /// </summary>
        public void RenderState() {
            player.Render();
            ball.Render();
            readASCII.level.Render();
            BreakoutBus.GetBus().ProcessEventsSequentially();
        }
        
        /// <summary>
        /// Inserts the correct number after level in the a .txt filename.
        /// </summary>
        /// <param name="levelnr"> The number of the current level. </param>
        /// <returns> The filename corresponding to the level of the levelnr</returns>
        public string ChooseLevel(int levelnr) {
            var lvl = "level.txt";
            var dot = lvl.IndexOf(".");
            lvl = lvl.Insert(dot, levelnr.ToString());
            return lvl;
        }

        /// <summary>
        /// Checks whether the entitycontainer is empty, and initializes the next level if so.
        /// </summary>
        public void IsNextLevel() {
            if (readASCII.level.amountOfBlocks == 0) {
                LevelNumber++;
                readASCII = new ASCIIReader();
                readASCII.LoadLevel(ChooseLevel(LevelNumber));
                ball = new Ball (
                    new DynamicShape (new Vec2F(0.5f, 0.15f), new Vec2F (0.05f, 0.05f),
                    new Vec2F (0.00005f, 0.015f)),
                    new Image (Path.Combine("..", "Breakout", "Assets", "Images", "ball.png")),
                    readASCII.level.Blocks, player);
                readASCII.level.player = player;
                StaticTimer.RestartTimer();
            }
        }

        /// <summary>
        /// Checks whether the ball on screen is dea, and takes away a life. Also moves PlayerHealth
        /// entities to the right to "fill up"
        /// </summary>
        public void isBallDead(){
            if (ball.IsBallDead()) {
                foreach (PlayerHealth health in player.playerHealth) {
                    if (!health.IsTheLifeLost() && !killSuccess && !player.isInvincible) {
                        health.LoseLife();
                        player.numberOfLives -= 1;
                        killSuccess = true; 
                        foreach (PlayerHealth newhealth in player.playerHealth) { //Moves lives one
                            newhealth.Shape.Position.X += 0.05f;            // 'place' to the right
                        }
                    }   
                } 
                ball.DeleteEntity();
                ball = new Ball (
                    new DynamicShape (new Vec2F(0.5f, 0.15f), new Vec2F (0.05f, 0.05f),
                    new Vec2F (0.00005f, 0.015f)),
                    new Image (Path.Combine("..", "Breakout", "Assets", "Images", "ball.png")),
                    readASCII.level.Blocks, player);
                killSuccess = false;
            }
        }

        /// <summary>
        /// Asserts whether the game is over. This can happen because the player has no more lives
        /// or because there is no more time left in the level.
        /// </summary>
        public void IsGameOver() {
            if (player.IsPlayerDead() || readASCII.level.TimeOut()) {
                BreakoutBus.GetBus().RegisterEvent(
                    new GameEvent{
                        EventType = GameEventType.GameStateEvent,
                        Message = "CHANGE_STATE",
                        StringArg1 = "GAME_OVER"
                    }
                );
            } 
        }   

        /// <summary>
        /// Checks whether LevelNumber is greater than 4. In this case, there are no more 
        /// numbered levels.
        /// </summary>
        public void IsGameWon() {
            if (LevelNumber > 4) {
                BreakoutBus.GetBus().RegisterEvent(
                    new GameEvent{
                        EventType = GameEventType.GameStateEvent,
                        Message = "CHANGE_STATE",
                        StringArg1 = "GAME_WON"
                    }
                );
            }
        }

        /// <summary>
        /// A method for registering the event of an arrow-key being released.
        /// </summary>
        /// <param name="key"> The arrow-key that is released. </param>
        public void KeyRelease (KeyboardKey key) {
            switch (key) {
                case KeyboardKey.Left:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_LEFT_RELEASE"});
                    break;

                case KeyboardKey.Right:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_RIGHT_RELEASE"});
                    break;
                
                case KeyboardKey.A:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_LEFT_RELEASE"});
                    break;

                case KeyboardKey.D:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_RIGHT_RELEASE"});
                    break;
            }
        }

        /// <summary>
        /// A method for registering the event of a key being pressed.
        /// </summary>
        /// <param name="key"> The arrow-, space- or escape-key that is pressed. </param>
        public void KeyPress (KeyboardKey key) {
            switch (key) {
                case KeyboardKey.Escape:
                    BreakoutBus.GetBus().RegisterEvent(
                        new GameEvent{
                            EventType = GameEventType.GameStateEvent,
                            Message = "CHANGE_STATE",
                            StringArg1 = "GAME_PAUSED"  
                        }
                        );
                    break;

                case KeyboardKey.Left:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_LEFT_PRESS"});
                    break;
                    
                case KeyboardKey.Right:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_RIGHT_PRESS"});
                    break;
                
                case KeyboardKey.A:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_LEFT_PRESS"});
                    break;

                case KeyboardKey.D:
                    BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                        EventType = GameEventType.PlayerEvent, 
                        From = this,
                        Message = "MOVE_RIGHT_PRESS"});
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles whether it is KeyPress or Keyrelease that should be called for a certain key
        /// </summary>
        /// <param name="action"> A KeyboardAction - press or release. </param>
        /// <param name="key"> Key to be transmitted as argument for the relevant 
        /// function. </param>
        public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
            switch (action) {
                case KeyboardAction.KeyPress:
                KeyPress (key);
                    break;

                case KeyboardAction.KeyRelease:
                KeyRelease (key);
                    break;

                default:
                    break;
            }
        }
    }
}