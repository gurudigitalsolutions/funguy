using System;
using System.IO;
using System.Collections.Generic;

namespace FunGuy
{

    public class MapSet: TextSet
    {
        public int Walkable = 0;

        public MapSet(string name, int texLibID, int walkable)
        {
            Name = name;
            TexLibID = texLibID;
            Walkable = walkable;
        }
        public MapSet(string name, int texLibID)
        {
            Name = name;
            TexLibID = texLibID;
        }



		private static bool _IsMapTextsLoaded = false;
		private static List<TextSet> _DefaultMapTexts = new List<TextSet>();
		public static List<TextSet> DefaultMapTexts
		{
			get
			{
				if (!_IsMapTextsLoaded)
				{
					_DefaultMapTexts = Load("default");
					_IsMapTextsLoaded = true;
				}
				return _DefaultMapTexts;
			}
		}


        public static List<TextSet> Load(string Name)
        {
            List<TextSet> retValue = new List<TextSet>();
            string resourcePath = string.Format("{0}/PNGs/MapSets", FunGuy.Game.configPath);
            Console.WriteLine(resourcePath);
            try
            {
                Console.WriteLine("{0}/{1}.txt", resourcePath, Name);
                StreamReader sr = new StreamReader(string.Format("{0}/{1}.txt", resourcePath, Name));
                string fileContents = sr.ReadToEnd();

                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string[] vn = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    int texLibID = TexLib.CreateTextureFromFile(string.Format("{0}/{1}.png", resourcePath, vn [1].Trim()));
					int val = 0;
					if (!int.TryParse(vn[0], out val)) { val = 0; }
                    MapSet tileSet = new MapSet(vn [1].Trim(), texLibID, val);
                    retValue.Add(tileSet);
                }
                Console.WriteLine("Loaded Tile Set: {0}", Name);
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", Name);
                return new List<TextSet>();
            }
        }

    }
}

