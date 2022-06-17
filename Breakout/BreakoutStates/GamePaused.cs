using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Input;
using DIKUArcade.Events;
using System.IO;
using DIKUArcade.Math;
using DIKUArcade.State;
using DIKUArcade.Timers;

namespace Breakout.BreakoutStates {
    /// <summary>
    /// Defines the game state GamePaused.
    /// </summary>
    public class GamePaused : IGameState {
        private static GamePaused instance = default!;
        private Entity backGroundImage = default!;
        private Text[] menuButtons = default!;
        private int activeMenuButton = default!;
        private int maxMenuButtons = 2;
        
        /// <summary>
        /// Gets an instance of the GamePaused.
        /// </summary>
        /// <returns> The instance of GamePaused. </returns>
        public static GamePaused GetInstance() {
            if (GamePaused.instance == null) {
                GamePaused.instance = new GamePaused();
                GamePaused.instance.InitializeGameState();
            }
            return GamePaused.instance;
        }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        public void InitializeGameState() {
            activeMenuButton = 1;
            backGroundImage = new Entity( 
                new StationaryShape (new Vec2F (0.0f, 0.0f), new Vec2F (1.0f, 1.0f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "BreakoutTitleScreen.png")));
            menuButtons = new Text[] 
            {new Text ("Continue", (new Vec2F(0.1f, -0.2f)), (new Vec2F(1.2f, 0.6f))),
            new Text ("Quit", (new Vec2F (0.3f, 0.01f)), (new Vec2F(1.2f, 0.6f)))};
            HighlightButton();
        }

        public void ResetState() {
            InitializeGameState();
        }

        public void UpdateState() {
            StaticTimer.PauseTimer();
        }
        
        /// <summary>
        /// Renders the state with background and buttons.
        /// </summary>
        public void RenderState() {
            backGroundImage.RenderEntity();
            foreach (Text button in menuButtons) {
                button.RenderText();
            }
        }

        /// <summary>
        /// Highlights the selected button.
        /// </summary>
        public void HighlightButton() {
            for (int i = 1; i <= maxMenuButtons; i++)
                    {
                        if (i == activeMenuButton) {
                            menuButtons[i-1].SetColor(
                                new Vec3F(0.238f, 0.75f, 0.43f));
                            menuButtons[i-1].SetFont("Impact");
                        } else {
                            menuButtons[i-1].SetColor(
                                new Vec3F(0.255f, 0.255f, 0.255f));
                            menuButtons[i-1].SetFont("Impact");
                        }
                    }
        }

        /// <summary>
        /// Activates a specific action based off which key is pressed.
        /// </summary>
        /// <param name="key"> The enter, up and down arrows on the keyboard. </param>
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
                                StringArg1 = "GAME_RUNNING",
                                StringArg2 = "CONTINUE"
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
        /// Handles the event based off which key's pressed.
        /// </summary>
        /// <param name="action"> The action with the key. </param>
        /// <param name="key"> Which key is presseed. </param>
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