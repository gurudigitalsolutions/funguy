using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FunGuy
{
	public class Map
	{
		public Map (string name, string tileSet, int width, int height)
		{
			Name = name;
			Width = width;
			Height = height;
			TileSet = tileSet;

			_Coordinates = new int[Width, Height];

			Textures = new Dictionary<string, int> ();
			TextureSetIDs = new Dictionary<int, int> ();
			LoadTileSet ();

		}

		public string Name { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
		public string TileSet { get; set; }
		public int[,] Coordinates {
			get {
				return _Coordinates;
			}
		}
		private int[,] _Coordinates;
		public Dictionary<string, int> Textures;
		public Dictionary<int, int> TextureSetIDs;

		public bool Load ()
		{
			return true;
		}

		public bool Save ()
		{
			return true;
		}

		private bool LoadTileSet ()
		{
			string resourcePath = string.Format ("FunGuy.Resources.PNGs.TileSets.{0}", TileSet);
			System.IO.StreamReader sr;
			sr = new System.IO.StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourcePath + ".textures.txt"));
			string fileContents = sr.ReadToEnd ();
			foreach (string item in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
				// load shit here
				string[] numname = item.Split (" ".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
				int texid = Int32.Parse (numname [0]);
				string texname = numname [1];

				Textures.Add (texname, TexLib.CreateTextureFromStream (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourcePath + "." + texname + ".png")));
				TextureSetIDs.Add (texid, Textures [texname]);

				Console.WriteLine ("TextureID {0} {1}", texname, Textures [texname]);
				Console.WriteLine ("Texture {0} {1}", texid, TexLib.CreateTextureFromStream (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourcePath + "." + texname + ".png")));
			}
			return true;
		}

	}
}

