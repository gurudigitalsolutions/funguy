using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FunGuy
{
	public class ThingSet : TextSet
	{
		public ThingSet()
		{

		}

		private static bool _IsTreeTextsLoaded = false;
		private static List<TextSet> _TreeTexts = new List<TextSet>();
		public static List<TextSet> TreeTexts
		{
			get
			{
				if (!_IsTreeTextsLoaded)
				{
					_TreeTexts = Load("ThingSets", "trees");
					_IsTreeTextsLoaded = true;
				}
				return _TreeTexts;
			}
		}

		public static List<TextSet> Load(string textureType, string setName)
		{
			List<TextSet> retValue = new List<TextSet>();
			string resourcePath = string.Format("{0}/PNGs/ThingSets", FunGuy.Game.configPath);
			Console.WriteLine(resourcePath);
			try
			{
				Console.WriteLine("{0}/{1}.txt", resourcePath, setName);
				StreamReader sr = new StreamReader(string.Format("{0}/{1}.txt", resourcePath, setName));
				string fileContents = sr.ReadToEnd();

				foreach (string name in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
				{
					if (name.Trim().Substring(0, 1) == "#")
					{
						continue;
					}
					int texLibID = TexLib.CreateTextureFromFile(string.Format("{0}/{1}.png", resourcePath, name));
					TextSet texture = new MapSet(name, texLibID);
					retValue.Add(texture);

				}
				Console.WriteLine("Loaded Tile Set: {0}", setName);
				return retValue;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Unable to Load Tile Set: {0}", setName);
				return new List<TextSet>();
			}
		}
	}
}
