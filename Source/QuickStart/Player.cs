using System;
using System.Collections.Generic;

namespace FunGuy
{
	public class Player
	{
		public Player()
		{
			Textures = new Dictionary<string, int>();
			TextureSetIDs = new Dictionary<int, int>();
			LoadTileSet();
		}
		public Dictionary<string, int> Textures;
		public Dictionary<int, int> TextureSetIDs;
		public int WorldX { get; set; }
		public int WorldY { get; set; }
		public int LastMovedTime{ get; set; }
		public int MoveInterval = 100;

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
			string resourcePath = string.Format("FunGuy.Resources.PNGs.Characters");
			System.IO.StreamReader sr;
			Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly());
			sr = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath + ".textures.txt"));
			string fileContents = sr.ReadToEnd();
			foreach (string item in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
			{
				// load shit here
				string[] numname = item.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				int texid = Int32.Parse(numname[0]);
				string texname = numname[1];

				Textures.Add(texname, TexLib.CreateTextureFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath + "." + texname + ".png")));
				TextureSetIDs.Add(texid, Textures[texname]);

				Console.WriteLine("TextureID {0} {1}", texname, Textures[texname]);
				Console.WriteLine("Texture {0} {1}", texname, TexLib.CreateTextureFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath + "." + texname + ".png")));
			}
			return true;
		}

	}
}

