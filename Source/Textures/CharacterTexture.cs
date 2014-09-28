using System;
using System.Collections.Generic;
using System.IO;

namespace FunGuy
{
    public class CharacterTexture : FunGuy.Texture
    {
        public float Height;
        public const int Down = 0;
        public const int Up = 1;
        public const int Left = 2;
        public const int Right = 3;

        public const int DownTwo = 4;
        public const int DownThree = 5;
        public const int UpTwo = 6;
        public const int UpThree = 7;
        public const int LeftTwo = 8;
        public const int LeftThree = 9;
        public const int RightTwo = 10;
        public const int RightThree = 11;

        public static int NextUp(int currdir)
        {
            if (currdir == Up) {
                return UpTwo; }
            if (currdir == UpTwo) {
                return UpThree; }
           
            return Up;
        }

        public static int NextDown(int currdir)
        {
            if (currdir == Down) {
                return DownTwo; }
            if (currdir == DownTwo) {
                return DownThree; }

            return Down;
        }

        public static int NextLeft(int currdir)
        {
            if (currdir == Left) {
                return LeftTwo; }
            if (currdir == LeftTwo) {
                return LeftThree; }

            return Left;
        }

        public static int NextRight(int currdir)
        {
            if (currdir == Right) {
                return RightTwo; }
            if (currdir == RightTwo) {
                return RightThree; }

            return Right;
        }


        public CharacterTexture(string name, int mapValue, int texLibID, int index, float height)
        {
            Name = name;
            Value = mapValue;
            TexLibID = texLibID;
            Index = index;
            Height = height;
        }
        public CharacterTexture(Texture texture, float height)
        {
            Name = texture.Name;
            Value = texture.Value;
            TexLibID = texture.TexLibID;
            Height = height;
        }



        public static List<CharacterTexture> _LoadCharacters()
        {
            List<CharacterTexture> retValue = new List<CharacterTexture>();
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

                    CharacterTexture character = new CharacterTexture(textureName, mapValue, texLibID, index, height);
                    retValue.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return new List<CharacterTexture>();
            }
        }
    }
}

