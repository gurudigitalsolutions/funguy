using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{
    public class Character : FunGuy.Texture
    {
        public float Height;
        public const int Down = 0;
        public const int Up = 1;
        public const int Left = 2;
        public const int Right = 3;

        public Character(string name, int mapValue, int texLibID, int index, float height)
        {
            Name = name;
            Value = mapValue;
            TexLibID = texLibID;
            Index = index;
            Height = height;
        }
        public Character(Texture texture, float height)
        {
            Name = texture.Name;
            Value = texture.Value;
            TexLibID = texture.TexLibID;
            Height = height;
        }



        public static List<Character> _LoadCharacters()
        {
            List<Character> retValue = new List<Character>();
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

