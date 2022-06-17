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

namespace Breakout.BreakoutStates {
    /// <summary>
    /// Defines the game state GameOver.
    /// </summary>
    public class GameOver : IGameState {
        private static GameOver instance = default!;
        private Entity backGroundImage = default!;
        private Text[] display = default!;
        private int activeMenuButton = default!;
        private int maxMenuButtons = 2;
    

        /// <summary>
        /// Gets an instance of the GameOver.
        /// /// </summary>
        /// /// <returns> The instance of GameOver. </returns>
        public static GameOver GetInstance() {
            if (GameOver.instance == null){
                GameOver.instance = new GameOver();
                GameOver.instance.InitializeGameState();
            }
            return GameOver.instance;
        }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        public void InitializeGameState() {
            activeMenuButton = 1;
            backGroundImage = new Entity( 
                new StationaryShape (new Vec2F (0.0f, 0.0f), new Vec2F (1.0f, 1.0f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "game_over.png")));
            display = new Text []
            {new Text ("Main Menu", (new Vec2F(0.025f, 0.05f)), (new Vec2F(1.2f, 0.6f))),
            new Text ("Quit", (new Vec2F (0.3f, 0.25f)), (new Vec2F(1.2f, 0.6f)))};
            HighlightButton();
        }

        /// <summary>
        /// Renders the state with background and buttons.
        /// </summary>
        public void RenderState() {
            backGroundImage.RenderEntity();
            foreach (Text button in display) {
                button.RenderText();
            }
        }

        /// <summary>
        /// Highlights the selected button.
        /// </summary>
        public void HighlightButton() {
            for (int i = 1; i <= maxMenuButtons; i++) {
                if (i == activeMenuButton) {
                    display[i-1].SetColor(
                        new Vec3F(0.90f, 0.255f, 0.00f));
                    display[i-1].SetFont("Impact");
                } else {
                    display[i-1].SetColor(
                        new Vec3F(0.255f, 0.255f, 0.255f));
                    display[i-1].SetFont("Impact");
                }
            }
        }

        /// <summary>
        /// A method for registering the event of a key being pressed.
        /// </summary>
        /// <param name="key"> The arrow-, enterkey or that is pressed. </param>
        public void KeyPress (KeyboardKey key) {
            switch (key) {
                case KeyboardKey.Up:
                    activeMenuButton = 2;
                    HighlightButton();
                    break;

                case KeyboardKey.Down:
                    activeMenuButton = 1;
                    HighlightButton();
                    break;
                    
                case KeyboardKey.Enter:
                    if (activeMenuButton == 1) {  
                        BreakoutBus.GetBus().RegisterEvent(
                            new GameEvent{
                                EventType = GameEventType.GameStateEvent,
                                Message = "CHANGE_STATE",
                                StringArg1 = "MAIN_MENU"
                            }
                        );
                    } else {
                        BreakoutBus.GetBus().RegisterEvent(
                            new GameEvent{
                                EventType = GameEventType.WindowEvent,
                                Message = "CLOSED_WINDOW"
                            }
                        );
                    }                    
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

                default:
                    break;
            }
        }

        public void UpdateState(){

        }
        
        public void ResetState(){
            InitializeGameState();
        }
    }
}