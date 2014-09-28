using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{

    public class Player
    {

#if DEBUG 
        public static int MoveInterval = 100;
#else 
        public int MoveInterval = 250;
#endif

        public Player(string charactername)
        {

        }

        public static List<Character> Characters
        {
            get
            {
                if (!_IsCharLoaded){
                    _Characters = LoadTileSet();
                    _IsCharLoaded = true;}
                return _Characters;
            }
        }
        private static List<Character> _Characters;
        private static bool _IsCharLoaded = false;
        public static int X;
        public static int Y;
        public static int LastMovedTime;
        public static int AnimStep = 0;
        public static bool CanMove
        {
            get {
                if (Environment.TickCount - LastMovedTime > MoveInterval)
            {
                    LastMovedTime = Environment.TickCount;
                    return true;
            }
                return false;
            }
        }


        private static List<Character> LoadTileSet()
        {
            List<Character> retValue = new List<Character>();
            try
            {
                StreamReader sr = new StreamReader(string.Format("{0}/Sets/Characters/characters.txt", FunGuy.Game.configPath));
                string fileContents = sr.ReadToEnd();
                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string[] valueNameIndex = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string textureName = valueNameIndex [1];
                    int mapValue = Int32.Parse(valueNameIndex [0]);
                    int index = Int32.Parse(valueNameIndex [2]);
                    string resourceID = string.Format("{0}/PNGs/Characters/{1}.png", FunGuy.Game.configPath, textureName);
                    int texLibID = TexLib.CreateTextureFromFile(resourceID);
                    float height = 3.0F;
                    if (mapValue < 0)
                    {
                        height = 0.0F;
                    }

                    Character character = new Character(textureName, mapValue, texLibID, index, height);
                    retValue.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return new List<Character>();
            }
        }
    }
}

