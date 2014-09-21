using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{
    public class Texture
    {
        public string Name;
        public int Value;
        public int TexLibID;
        public int Index;

       
        public Texture(string texName, int texValue, int texLibID, int index)
        {
            Name = texName;
            Value = texValue;
            TexLibID = texLibID;
            Index = index;
        }
        public Texture(string texName, int texValue, int texLibID)
        {
            Name = texName;
            Value = texValue;
            TexLibID = texLibID;
            Index = 0;
        }
        public Texture(string texName, int texLibID)
        {
            Name = texName;
            Value = 0;
            TexLibID = texLibID;
            Index = 0;
        }
        public Texture()
        {
        }

        /*
         * 
         * 
         * 
         */

        private static bool _IsMapTextsLoaded = false;
        private static List<Texture> _DefaultMapTexts = new List<Texture>();
        public static List<Texture> DefaultMapTexts
        {
            get
            {
                if (!_IsMapTextsLoaded)
                {
                    _DefaultMapTexts = _LoadTextures(TextType.TileSet, "default");
                    _IsMapTextsLoaded = true;
                }
                return _DefaultMapTexts;
            }
        }

        private static bool _IsTreeTextsLoaded = false;
        private static List<Texture> _TreeTexts = new List<Texture>();
        public static List<Texture> TreeTexts
        {
            get
            {
                if (!_IsTreeTextsLoaded)
                {
                    _TreeTexts = Texture._LoadTextures(TextType.ThingSets, "trees");
                    _IsTreeTextsLoaded = true;
                }
                return _TreeTexts;
            }
        }

           

        public static List<Texture> _LoadTiles(string setType, string setName)
        {
            List<Texture> retValue = new List<Texture>();
            try
            {
                StreamReader sr = new StreamReader(string.Format("{0}/Sets/{1}/{2}.txt", FunGuy.Game.configPath, setType, setName));
                string fileContents = sr.ReadToEnd();

                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string[] numName = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string textureName = numName [1];
                    int mapValue = Int32.Parse(numName [0]);
                    string resourceID = string.Format("{0}/PNGs/{1}/{2}.png", FunGuy.Game.configPath, setType,textureName);
                    Console.WriteLine("_LoadTiles resourceID {0}", resourceID);
                    int texLibID = TexLib.CreateTextureFromFile(resourceID);
                    Texture texture = new Texture(textureName, mapValue, texLibID);
                    retValue.Add(texture);
                }
                Console.WriteLine("Loaded Tile Set: {0}", setName);
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", setName);
                return new List<Texture>();
            }
        }

        public static List<Texture> _LoadTextures(string setType, string setName)
        {
            List<Texture> retValue = new List<Texture>();
            try
            {
                StreamReader sr = new StreamReader(string.Format("{0}/Sets/{1}/{2}.txt", FunGuy.Game.configPath, setType, setName));
                string fileContents = sr.ReadToEnd();

                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string resourceID = string.Format("{0}/PNGs/{1}/{2}.png", FunGuy.Game.configPath, setType,line);
                    int texLibID = TexLib.CreateTextureFromFile(resourceID);
                    Texture texture = new Texture(line, texLibID);
                    retValue.Add(texture);
                }
                Console.WriteLine("Loaded Tile Set: {0}", setName);
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", setName);
                return new List<Texture>();
            }
        }
    }

}