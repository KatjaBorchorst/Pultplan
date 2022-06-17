using DIKUArcade;
using DIKUArcade.GUI;
using DIKUArcade.Input;
using DIKUArcade.Events;
using System.Collections.Generic;
using Breakout.BreakoutStates;
using Breakout.Block;

namespace Breakout {
    /// <summary>
    /// Inherits from DIKUGame and implements the IGameEventProcessor interface in ProcessEvent().
    /// The class controls game controls through the ProcessEvents method demanded by the interface, 
    /// registering events in the EventBus. The game's entities are also updated and rendered.
    /// </summary>
    public class Game : DIKUGame, IGameEventProcessor {
        public StateMachine statemachine;

        /// <summary>
        /// Constructor for the game class. Initializes different objects that are
        /// needed in methods.
        /// </summary>
        /// <param name="windowArgs"> Arguments for creating a DIKUArcade window. </param>
        /// <returns></returns>
        public Game (WindowArgs windowArgs) : base(windowArgs) {
            statemachine = new StateMachine();
            BreakoutBus.GetBus().InitializeEventBus (new List<GameEventType> { 
                GameEventType.PlayerEvent, 
                GameEventType.WindowEvent,
                GameEventType.GameStateEvent,
                GameEventType.InputEvent,
                GameEventType.StatusEvent
                });

            window.SetKeyEventHandler(HandleKeyEvent);  
            BreakoutBus.GetBus().Subscribe(GameEventType.WindowEvent, this);
            BreakoutBus.GetBus().Subscribe(GameEventType.GameStateEvent, statemachine);
        }

        /// <summary>
        /// The method for processing the closing event.
        /// </summary>
        /// <param name="gameEvent"> GameEvent to be processed. </param>
        public void ProcessEvent (GameEvent gameEvent) {
            if (gameEvent.EventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                    case "CLOSED_WINDOW":
                        window.CloseWindow ();
                        break;

                    default:
                        break;
                }
            }
        }

        public void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
            statemachine.ActiveState.HandleKeyEvent(action, key);
        }

        public override void Render(){
            statemachine.ActiveState.RenderState();
        }

        public override void Update(){
            BreakoutBus.GetBus().ProcessEventsSequentially();
            statemachine.ActiveState.UpdateState();
        }
    }
}