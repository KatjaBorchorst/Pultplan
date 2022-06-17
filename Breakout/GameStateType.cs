using System;
namespace Breakout {
    /// <summary>
    /// Enumerations of the gamestates.
    /// </summary>
    public enum GameStateType {
        GameRunning,
        GamePaused,
        MainMenu,
        GameOver,
        GameWon
    }

    /// <summary>
    /// Transforms the states.
    /// </summary>
    public class StateTransformer {
        /// <summary>
        /// Transforms the GameState from string to state.
        /// </summary>
        /// <param name="state"> The specific state. </param>
        /// <returns> The state of the GameState. </returns>
        public static GameStateType TransformStringToState (string state) {
            switch (state) {
                case "GAME_RUNNING":
                return GameStateType.GameRunning;

                case "GAME_PAUSED":
                return GameStateType.GamePaused;

                case "MAIN_MENU":
                return GameStateType.MainMenu;

                case "GAME_OVER":
                return GameStateType.GameOver;

                case "GAME_WON":
                return GameStateType.GameWon;

                default:
                throw new ArgumentException ("No Match");
            }
        }

        /// <summary>
        /// Transforms the Gamestate from state to string.
        /// </summary>
        /// <param name="state"> The specific state. </param>
        /// <returns> The string of the GameState. </returns>
        public static string TransformStateToString (GameStateType state) {
            switch (state) {
                case GameStateType.GameRunning:
                return "GAME_RUNNING";

                case GameStateType.GamePaused:
                return "GAME_PAUSED";

                case GameStateType.MainMenu:
                return "MAIN_MENU";

                case GameStateType.GameOver:
                return "GAME_OVER";

                case GameStateType.GameWon:
                return "GAME_WON";

               default:
                throw new ArgumentException ("No Match");
            }
        }
    }
}