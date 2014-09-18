using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{

    public class Player
    {
#if DEBUG 
        public int MoveInterval = 100;
#else 
		public int MoveInterval = 250;
#endif

        public Player()
        {
            Textures = new List<MyTexture>();
            Characters = new List<MyCharacter>();
            LoadTileSet();
        }

        public List<MyTexture> Textures;
        public List<MyCharacter> Characters;
        public int PosX;
        public int PosY;
        public int LastMovedTime;

        public static List<MyCharacter> myCharacters;


        public bool CanMove()
        {
            if (Environment.TickCount - LastMovedTime > MoveInterval)
            {
                LastMovedTime = Environment.TickCount;
                return true;
            }
            return false;
        }


        private bool LoadTileSet()
        {
            string resourcePath = string.Format("{0}/PNGs/Characters", FunGuy.Game.configPath);
            try
            {
                StreamReader sr = new StreamReader(resourcePath + "/characters.txt");
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
                    string resourceID = string.Format("{0}/{1}.png", resourcePath, textureName);
                    int texLibID = TexLib.CreateTextureFromFile(resourceID);
                    float height = 1.0F;
                    if (mapValue < 0)
                    {
                        height = 0.0F;
                    }

                    MyCharacter character = new MyCharacter(textureName, mapValue, texLibID, index, height);
                    Console.WriteLine("Added Character {0}, {1}, {2}, {3}", character.Name, character.Value, character.TexLibID, character.Index);
                    Characters.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return false;
            }
        }

    }
}

