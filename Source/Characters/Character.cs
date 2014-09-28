using System;
using System.Collections.Generic;

namespace FunGuy
{

    public class Character
    {
        public int MoveInterval = 100;

        public Character()
        {
            ;
        }

        public Character(string charactername)
        {
            ;
        }

        private List<CharSet> _charSets;
        private bool _IsCharLoaded = false;
        public int X;
        public int Y;
        public int LastMovedTime;
        public int AnimStep = 0;

        public static List<CharSet> charSets
        {
            get
            {
                if (!_IsCharLoaded){
                    _charSets = LoadTileSet();
                    _IsCharLoaded = true;}
                return _charSets;
            }
        }

        public bool CanMove
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


        private static List<CharSet> LoadTileSet()
        {
            List<CharSet> retValue = new List<CharSet>();
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

                    CharSet character = new CharSet(textureName, mapValue, texLibID, index, height);
                    retValue.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return new List<CharSet>();
            }
        }

    }
}

