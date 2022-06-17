using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Breakout;
using Breakout.Block;

namespace Breakout.LevelLoading {
    public class ASCIIReader { 
        public string[] map {get; private set;} = default!;
        public string meta {get; private set;} = default!;
        public string[] legend {get; private set;} = default!; 
        public Level level = default!;

        /// <summary>
        /// A method calling the single methods in correct order, such that no mistakes are made.
        /// </summary>
        /// <param name="filename"> name of the level to be loaded </param>
        public void LoadLevel (string filename) {
            ReadFile(filename);
            ProcessMeta();
            ProcessLegend();
            level.ProcessMap();
        }

        /// <summary>
        /// Reads the ASCII file and places data in appropriate data structures.
        /// </summary>
        /// <param name="filename"> The filename of the ASCII txt file of the chosen level. </param>
        private void ReadFile (string filename) {
            var Text = "";
            var r_map = new Regex (@"Map:([^']+)Map\/");
            var r_meta = new Regex (@"Meta:([^']+)Meta\/");
            var r_legend = new Regex (@"Legend:([^']+)Legend\/");
            try {
                var file = Path.Combine("..", "Breakout", "Assets", "Levels", filename); 
                StreamReader reader = new StreamReader(file);
                Text = reader.ReadToEnd();
                reader.Close();
                meta = r_meta.Match(Text).Value;
                var legendtext = r_legend.Match(Text).Value;
                legend = legendtext.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);           
                var maptext = r_map.Match(Text).Value;
                map = maptext.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);  
            } catch (IOException) {
                level = new Level("empty/invalid", new string[] {});
            }
            
        }

        /// <summary>
        /// Processes the meta data extracted by ReadFile() and passes on information to the level
        /// class.
        /// </summary>
        private void ProcessMeta (){
            Regex r = new Regex (@"Name:.+");
            if (r.Match(meta).Success == false) {
                level = new Level("empty/invalid", new string[] {});
            } else {
                string name = r.Match(meta).Value;
                level = new Level(name.Substring(6), map);
                List<Regex> LevelElements = new List<Regex>() {new Regex (@"Time:.+"), 
                new Regex(@"Hardened:.+"), new Regex(@"PowerUp:.+"),
                new Regex (@"Unbreakable:.+")};
                
                foreach (Regex element in LevelElements) {
                    if (element.Match(meta).Success){
                        switch (LevelElements.IndexOf(element)) {
                            case 0:
                            string extractedTime = (element.Match(meta).Value).Substring(6);
                            level.Time = Int32.Parse(extractedTime);
                            break;

                            case 1:
                            var extractedHardened = (element.Match(meta).Value).Substring(10, 1);
                            level.Hardened = Char.Parse(extractedHardened);
                            break;

                            case 2:
                            var extractedPowerup = (element.Match(meta).Value).Substring(9, 1);
                            level.PowerUp = Char.Parse(extractedPowerup);
                            break;

                            case 3: 
                            var extractedUnbreakable = (element.Match(meta).Value).Substring(13, 1);
                            level.Unbreakable = Char.Parse(extractedUnbreakable);
                            break;

                            default:
                            break;
                        }
                    }
                
                }
            }
        }
            
        /// <summary>
        /// Processes legend data extracted by ReadFile() and creates dictionary of blocks and 
        /// passes it to the level class.
        /// </summary>
        private void ProcessLegend (){
            foreach (string s in legend) {
                var parenthesis = s.IndexOf(")"); //index of parenthesis
                if (parenthesis > 0) {
                    level.LegendData.Add(Char.Parse(s.Substring(parenthesis-1, 1)), 
                                                    //dictionary key (char)
                                                    s.Substring(parenthesis+2)); 
                                                    //dictionary value (string)
                }
            }
        }
    }
}