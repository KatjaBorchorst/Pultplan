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
    /// Defines the game state GameWon.
    /// </summary>
    public class GameWon : IGameState {
        private static GameWon instance = default!;
        private Entity backGroundImage = default!;
        private Text[] display = default!;
        private int maxMenuButtons = 2;
        private int activeMenuButton = 1;
    

        /// <summary>
        /// Gets an instance of GameWon if one such exists.
        /// </summary>
        /// <returns> The instance of GameWon. </returns>
        public static GameWon GetInstance() {
            if (GameWon.instance == null){
             GameWon.instance = new GameWon();
             GameWon.instance.InitializeGameState();
            }
            return GameWon.instance;
        }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        public void InitializeGameState() {
            activeMenuButton = 1;
            backGroundImage = new Entity( 
                new StationaryShape (new Vec2F (0.0f, 0.0f), new Vec2F (1.0f, 1.0f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "you_win.png")));
            display = new Text[] {new Text ("Main Menu", (new Vec2F(0.15f, 0.05f)), (new Vec2F(1.2f, 0.6f))),
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

        public void UpdateState(){

        }

        public void ResetState(){
            InitializeGameState();
        }

        /// <summary>
        /// Highlights the selected button.
        /// </summary>
        public void HighlightButton() {
            for (int i = 1; i <= maxMenuButtons; i++)
                    {
                        if (i == activeMenuButton) {
                            display[i-1].SetColor(
                                new Vec3F(0.238f, 0.75f, 0.43f));
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
        /// <param name="key"> The arrow-, space- and escape-key that is pressed. </param>
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

    }
}