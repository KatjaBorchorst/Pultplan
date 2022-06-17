using DIKUArcade.Events;
using DIKUArcade.State;
using DIKUArcade.Timers;

namespace Breakout.BreakoutStates {
    /// <summary>
    /// Defines the states in a game.
    /// </summary>
    public class StateMachine : IGameEventProcessor {
        
        public IGameState ActiveState { get; private set; }
        
        /// <summary>
        /// The constructor of StateMachine.
        /// </summary>
        public StateMachine() {
            BreakoutBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            BreakoutBus.GetBus().Subscribe(GameEventType.InputEvent, this);
            ActiveState = MainMenu.GetInstance();
        }

        /// <summary>
        /// Switches the gamestates.
        /// </summary>
        /// <param name="stateType"> A specific statetype: GamePaused, GameRunning
        /// and MainMenu. </param>
        private void SwitchState (GameStateType stateType) {
            switch (stateType) {
                case GameStateType.GameRunning:
                    if (!object.ReferenceEquals(ActiveState, GameRunning.GetInstance())) {
                        ActiveState = GameRunning.GetInstance();
                    }
                    StaticTimer.ResumeTimer();  
                    break;
                
                case GameStateType.GamePaused:
                    if (!object.ReferenceEquals(ActiveState, GamePaused.GetInstance())) {
                        ActiveState = GamePaused.GetInstance();
                    }
                    break;
                
                case GameStateType.MainMenu:
                    if (!object.ReferenceEquals(ActiveState, MainMenu.GetInstance())) {
                        ActiveState = MainMenu.GetInstance();
                    }
                    break;

                case GameStateType.GameOver:
                    if (!object.ReferenceEquals(ActiveState, GameOver.GetInstance())) {
                        ActiveState = GameOver.GetInstance();
                    }
                    break;

                case GameStateType.GameWon:
                    if (!object.ReferenceEquals(ActiveState, GameWon.GetInstance())) {
                        ActiveState = GameWon.GetInstance();
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Processes the event of a specific game event.
        /// </summary>
        /// <param name="gameEvent"> The selected game event. </param>
        public void ProcessEvent(GameEvent gameEvent) {
            if (gameEvent.EventType == GameEventType.GameStateEvent) {
                switch (gameEvent.Message) {
                    case "GAME_RUNNING":
                        ActiveState.UpdateState();                        
                        ActiveState.RenderState();
                        break;
                    
                    case "GAME_PAUSED":
                        ActiveState.UpdateState();
                        ActiveState.RenderState();
                        break;

                    case "GAME_OVER":
                        ActiveState.UpdateState();
                        ActiveState.RenderState();
                        break;

                    case "GAME_WON":
                        ActiveState.UpdateState();
                        ActiveState.RenderState();
                        break;
                    
                    case "MAIN_MENU":
                        ActiveState.UpdateState();
                        ActiveState.RenderState();
                        break;

                    case "CLOSED WINDOW":
                        break;

                    case "CHANGE_STATE":
                        switch (gameEvent.StringArg1) {
                            case "GAME_RUNNING":
                                SwitchState(GameStateType.GameRunning);
                               if (gameEvent.From == MainMenu.GetInstance()) {
                               ActiveState.ResetState();
                               }
                                break;
                            
                            case "GAME_PAUSED":
                                SwitchState(GameStateType.GamePaused);
                                break;

                            case "GAME_OVER":
                                SwitchState(GameStateType.GameOver);
                                break;
                            
                            case "GAME_WON":
                                SwitchState(GameStateType.GameWon);
                                break;

                            case "MAIN_MENU":
                                SwitchState(GameStateType.MainMenu);
                                break;
                            
                            default:
                                break;
                        }
                        break;
                }
            }
        }
    }
}