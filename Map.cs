using System;
using System.Collections.Generic;

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
		private Dictionary<string, int> Textures;

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
			System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("grass");
		}

	}
}

