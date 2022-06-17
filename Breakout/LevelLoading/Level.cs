using System.Collections.Generic;
using System;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using Breakout.Block;
using DIKUArcade.Math;
using DIKUArcade.Events;
using DIKUArcade.Timers;
using DIKUArcade.Physics;

namespace Breakout.LevelLoading {
    public class Level : IGameEventProcessor{
        public string Name;
        private string[] Mapinfo;
        public int ? Time = null;
        public char? Hardened;
        public char? PowerUp;
        public char? Unbreakable;
        public IDictionary<char, string> LegendData;
        public EntityContainer<Block.Block> Blocks;
        public int amountOfBlocks = default!;
        private Text timedisplay = default!;
        public Player player = default!;
        public EntityContainer<PowerUp> activepowerups;

        public Level (string name, string[] mapinfo) {
            Name = name;
            Mapinfo = mapinfo;
            LegendData = new Dictionary<char, string>();
            Blocks = new EntityContainer<Block.Block>(); 
            StaticTimer.RestartTimer();
            BreakoutBus.GetBus().Subscribe(GameEventType.StatusEvent, this);
            activepowerups = new EntityContainer<PowerUp>();
        }

        /// <summary>
        /// Renders both blocks and updates and displays time left as well as handling powerups if
        /// there are any.
        /// </summary>
        public void Render(){
            Blocks.RenderEntities();
            if (Time != null) { //if a time constraint exists in level
                UpdateTime();
                timedisplay.RenderText();
            }
            if (activepowerups.CountEntities() > 0) { //if there is at least one active powerup
                activepowerups.Iterate(activepowerup =>{
                    ProcessPowerUps();
                    activepowerup.RenderEntity();
                    activepowerup.CheckDeletion();
                    activepowerup.Shape.AsDynamicShape().Move();
                });
            }
        }
        
        /// <summary>
        /// Updates the text field with a new text containing the current remaining time.
        /// </summary>
        public void UpdateTime() {
            var timeleft = (Time-Math.Round(StaticTimer.GetElapsedSeconds())).ToString();
            timedisplay = new Text("TIME: " + timeleft, 
            new Vec2F (0.4125f, -0.12f), new Vec2F (0.25f, 0.18f)); //Time left rounded to whole int
            timedisplay.SetColor(255, 255, 255, 0); //yellow
            timedisplay.SetFont("Eras Bold ITC");
        }

        /// <summary>
        /// Adds 50 seconds to the timer.
        /// </summary>
        public void AddTime() {
            Time += 50;
        }

        /// <summary>
        /// Processes the map info passed on buy the ASCIIReader and places blocks in the entity
        /// container.
        /// </summary>
        public void ProcessMap() {
            for (int i = 0; i < Mapinfo.Length; i++) {
                for (int j = 0; j < Mapinfo[i].Length; j++) {
                    if (LegendData.ContainsKey((Mapinfo[i])[j])) {//if there is a match in dictionary
                        Image blockImage = (new Image (Path.Combine("..", "Breakout", "Assets", "Images", 
                                                                LegendData[(Mapinfo[i])[j]])));
                        var dot = (LegendData[(Mapinfo[i])[j]]).IndexOf("."); //used for 
                                                                //inserting "damaged" before .png                                        
                        //brokenimage for hardened block
                        Image brokenImage = (new Image (Path.Combine("..", "Breakout", "Assets", "Images", 
                            LegendData[(Mapinfo[i])[j]].Insert(dot, "-damaged"))));
                        StationaryShape blockShape = new StationaryShape((float)(j*0.083f), //xpos
                            1.0f - (float)(0.04*i), //ypos
                            0.083f, 0.04f); //extent
                        //Add hardened
                        if (Hardened == (Mapinfo[i])[j]) {
                        amountOfBlocks++; //Used to see if game is over
                        Blocks.AddEntity (new Block.HardenedBlock
                            (blockShape, blockImage, brokenImage));
                        //Add unbreakable
                        } else if (Unbreakable == (Mapinfo[i])[j]) {
                            Blocks.AddEntity(new Block.UnbreakableBlock
                            (blockShape, blockImage)); 
                        //Add powerup
                        } else if (PowerUp == (Mapinfo[i])[j]) {
                            amountOfBlocks++; //Used to see if game is over
                            Blocks.AddEntity(new Block.PowerUpBlock (blockShape, blockImage)); 
                        } else {
                        //Add normal block
                        amountOfBlocks++; //Used to see if game is over
                        Blocks.AddEntity(new Block.Block (blockShape, blockImage)); 
                        }
                    }  
                }
            }
        }

        /// <summary>
        /// Checks whether the time has elapsed or not
        /// </summary>
        /// <returns>True or false depending on whether the time has elapsed</returns>
        public bool TimeOut() {
            if (Time != null) {
                if(Time == Math.Round(StaticTimer.GetElapsedSeconds())) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
        
        /// <summary>
        /// Iterates the active powerups and checks whether the player collides with the powerup.
        /// If so it handles each powerup appropriately.
        /// </summary>
        public void ProcessPowerUps(){
            activepowerups.Iterate(activepowerup => {
                if (CollisionDetection.Aabb(activepowerup.Shape.AsDynamicShape(),
                                            player.Shape.AsStationaryShape()).Collision) {
                    activepowerup.Collide();
                if (activepowerup.collided) {
                    switch (activepowerup.name) {
                        case "More Time":
                        AddTime();
                        break;

                        case "Wide":
                        BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                                EventType = GameEventType.StatusEvent, 
                                Message = "WIDE",
                                From = this});
                        break;

                        case "Extra Life":
                        BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                                EventType = GameEventType.StatusEvent, 
                                Message = "EXTRA_LIFE",
                                From = this});
                        break;

                        case "Quick":
                        BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                                EventType = GameEventType.StatusEvent, 
                                Message = "QUICK",
                                From = this});
                        break;

                        case "Invincible":
                        BreakoutBus.GetBus().RegisterEvent(new GameEvent {
                                EventType = GameEventType.StatusEvent, 
                                Message = "INVINCIBLE",
                                From = this});
                        break;

                        default:
                        break;
                    }
                activepowerup.DeleteEntity();
                }
            }});
        }

        /// <summary>
        /// Processes the event of a block being hit or/and powerup being spawned from a 
        /// powerup block.
        /// </summary>
        /// <param name="gameEvent"></param>
        public void ProcessEvent (GameEvent gameEvent) {
            if (gameEvent.EventType == GameEventType.StatusEvent){
                if (gameEvent.From is PowerUpBlock) {
                activepowerups.AddEntity(new PowerUp ((DynamicShape)gameEvent.ObjectArg1, //shape
                    new Image (Path.Combine("..", "Breakout", "Assets", "Images", 
                    gameEvent.StringArg1)), //image
                    gameEvent.StringArg2)); //name
                } else if (gameEvent.From is Ball) {
                amountOfBlocks--;
                }
            }
        }
    }
} 