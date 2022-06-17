using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Input;
using DIKUArcade.Events;
using System.IO;
using DIKUArcade.Math;
using DIKUArcade.State;

namespace Breakout.BreakoutStates {
    /// <summary>
    /// Defines the game state MainMenu.
    /// </summary>
    public class MainMenu : IGameState {
        private static MainMenu instance = default!;
        private Entity backGroundImage = default!;
        private Text[] menuButtons = default!;
        private int activeMenuButton = default!;
        private int maxMenuButtons = 2;

        /// <summary>
        /// Creates an instance of the MainMenu.
        /// </summary>
        /// <returns> An instance of the MainMenu. </returns>
        public static MainMenu GetInstance() {
            if (MainMenu.instance == null) {
                MainMenu.instance = new MainMenu();
                MainMenu.instance.InitializeGameState();
            }
            return MainMenu.instance;
        }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        public void InitializeGameState() {
            activeMenuButton = 1;
            backGroundImage = new Entity( 
                new StationaryShape (new Vec2F (0.0f, 0.0f), new Vec2F (1.0f, 1.0f)),
                new Image (Path.Combine("..", "Breakout", "Assets", "Images", "shipit_titlescreen.png")));
            menuButtons = new Text[] 
            {new Text ("New Game", (new Vec2F(0.03f, -0.2f)), (new Vec2F(1.2f, 0.6f))),
            new Text ("Quit", (new Vec2F (0.3f, 0.01f)), (new Vec2F(1.2f, 0.6f)))};
            HighlightButton();
        }

        /// <summary>
        /// Resets the state as MainMenu.
        /// </summary>
        public void ResetState() {
            InitializeGameState();
        }

        public void UpdateState() {

        }

        /// <summary>
        /// Renders the MainMenu state with background and buttons.
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
                                From = this,
                                StringArg1 = "GAME_RUNNING",
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