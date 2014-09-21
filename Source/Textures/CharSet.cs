using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


namespace FunGuy
{

    public class CharSet : TextSet
    {
        public float Height = 1.0f;
        public int Direction = Character.Down;// Character.Direction.Down;
        

        public CharSet(string name, int value, int texLibID, int direction, float height)
        {
            Name = name;
            Value = value;
            TexLibID = texLibID;
            Height = height;
        }
        public CharSet(string name, int value, int texLibID, int direction)
        {
            Name = name;
            Value = value;
            TexLibID = texLibID;
			Direction = direction;
        }
        public CharSet(string name, int value, int texLibID)
        {
            Name = name;
            Value = value;
            TexLibID = texLibID;
        }
        public CharSet(string name, int texLibID)
        {
            Name = name;
            Value = 0;
            TexLibID = texLibID;
            Height = 1;
        }
        
        public CharSet(TextSet texture, float height)
        {
            Name = texture.Name;
            Value = texture.Value;
            TexLibID = texture.TexLibID;
            Height = height;
        }

        public CharSet()
        {
        }

/*      
		public CharSet(SerializationInfo info, StreamingContext ctxt)
        {
            //X = (int)info.GetValue("X", typeof(int));

        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
//            info.AddValue("X", X);

        }
*/



        public static List<TextSet> Load(string setName)
        {
            List<TextSet> retValue = new List<TextSet>();
            string resourcePath = string.Format("{0}/PNGs/CharSets", FunGuy.Game.configPath);
            try
            {
                StreamReader sr = new StreamReader(string.Format("{0}/{1}.txt", resourcePath, setName));
                string fileContents = sr.ReadToEnd();
                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string textureName = string.Empty;
                    int mapValue = 0;
                    string[] numName = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (1 < numName.GetLength(0))
                    {
                        textureName = numName [1];
                        mapValue = Int32.Parse(numName [0]);
                    }
                    else
                    {
                        textureName = line;
                    }
                    int texLibID = TexLib.CreateTextureFromFile(string.Format("{0}/{1}.png", resourcePath, textureName));
                    CharSet character = new CharSet(textureName, mapValue, texLibID);
                    retValue.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return new List<TextSet>();
            }
        }

		public static List<CharSet> AsCharSet(List<TextSet> textSet)
		{
			List<CharSet> retValue = new List<CharSet>();
			foreach (TextSet text in textSet)
			{
				try
				{
					CharSet charSet = (CharSet)text;
					retValue.Add(charSet);
				}
				catch (Exception ex) { Console.WriteLine(ex.Message); }
			}
			return retValue;
		}
    }
}

