using System;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FunGuy
{
	public class Map
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunGuy.Map"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='tileSet'>
		/// Tile set.
		/// </param>
		/// <param name='width'>
		/// Width.
		/// </param>
		/// <param name='height'>
		/// Height.
		/// </param>
		public Map(string name, string tileSet, int width, int height)
		{
			Name = name;
			TileSet = tileSet;
			Width = width;
			Height = height;

			Coordinates = new int[width, height];
			Textures = new List<MyTexture>();

			LoadTileSet();
			LoadPlayerPosition();
			Console.WriteLine("Loading Map File: {0}", Load());
		}

		/// <summary>
		/// The name.
		/// </summary>
		public string Name;
		/// <summary>
		/// The tile set.
		/// </summary>
		public string TileSet;
		/// <summary>
		/// The width.
		/// </summary>
		public int Width;
		/// <summary>
		/// The height.
		/// </summary>
		public int Height;
		/// <summary>
		/// The coordinates.
		/// </summary>
		public int[,] Coordinates;
		/// <summary>
		/// The textures.
		/// </summary>
		public List<MyTexture> Textures;
		/// <summary>
		/// The player x.
		/// </summary>
		public int StartX;
		/// <summary>
		/// The player y.
		/// </summary>
		public int StartY;
		/// <summary>
		/// Gets the map file.
		/// </summary>
		/// <value>
		/// The map file.
		/// </value>
		public string MapFile
		{
			get
			{
				return string.Format("{0}/{1}.bin", LocalConfigPath, Name);
			}
		}
		/// <summary>
		/// Gets the local config path.
		/// </summary>
		/// <value>
		/// The local config path.
		/// </value>
		public	string LocalConfigPath
		{
			get
			{
				return string.Format("{0}/{1}/{2}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "FunGuy", "Maps");
			}
		}


		/// <summary>
		/// Load this instance.
		/// </summary>
		public bool Load()
		{
			if (!File.Exists(MapFile))
			{
				if (!CreateDefaultMap())
				{
					return false;
				}
			}

			FileStream fs;
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				fs = new FileStream(MapFile, FileMode.Open, FileAccess.Read);
				Coordinates = (int[,])bf.Deserialize(fs);
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						Console.Write("{0} ", Coordinates [x, y]);
					}
					Console.WriteLine();
				}
				Console.WriteLine("Loaded coordinates from: {0}", MapFile);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Unable to load coordinates from: {0}", MapFile);
				return false;
			}
			finally
			{
				fs.Close();
			}

		}

		/// <summary>
		/// Save this instance.
		/// </summary>
		public bool Save()
		{
			if (!FindMapFile())
			{
				return false;
			}

			Coordinates [StartX, StartY] = 0;

			FileStream fs;
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				fs = new FileStream(MapFile, FileMode.Create, FileAccess.Write);
				bf.Serialize(fs, Coordinates);
				Console.WriteLine("Saved coordinate to file: {0}", MapFile);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Error Serializing to: {0}", MapFile);
				return false;
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// Loads the tile set.
		/// </summary>
		/// <returns>
		/// The tile set loaded.
		/// </returns>
		private bool LoadTileSet()
		{
			string resourcePath = string.Format("Resources/PNGs/TileSets/{0}", TileSet);
			int editID = 1;
			StreamReader sr;

			try
			{
				sr = new StreamReader(resourcePath + "/textures.txt");
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
					string resourceID = string.Format("{0}/{1}.png", resourcePath, textureName);
					int texLibID = TexLib.CreateTextureFromFile(resourceID);
					MyTexture texture = new MyTexture(textureName, mapValue, texLibID, editID);
					Textures.Add(texture);
					editID++;
				}
				Console.WriteLine("Loaded Tile Set: {0}", TileSet);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Unable to Load Tile Set: {0}", TileSet);
				return false;
			}
		}

		/// <summary>
		/// Loads the player position.
		/// </summary>
		/// <returns>
		/// The player position.
		/// </returns>
		private bool LoadPlayerPosition()
		{
			// Resource path from TileSet
			string resourcePath = "Resources";
			StreamReader sr;

			try
			{
				sr = new StreamReader(resourcePath + "/maps.txt");

				// reads stream
				string fileContents = sr.ReadToEnd();

				// Read each line of the textures.txt file
				foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
				{
					// Ignores # commented lines in texture file
					if (line.Trim().Substring(0, 1) == "#")
					{
						continue;
					}
					// Split map value from name in line
					string[] nameXY = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					Console.WriteLine("nameXY[0]: {0}", nameXY [0]);
					if (nameXY [0] == Name)
					{
						StartX = Int32.Parse(nameXY [1]);
						StartY = Int32.Parse(nameXY [2]);
						Console.WriteLine("loaded player position for: {0} X: {1} Y: {1}", Name, StartX, StartY);
						return true;
					}

				}
				Console.WriteLine("Unable to load player position for: {0}", Name);
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Failed reading file: {0}", resourcePath);
				return false;
			}
		}

		/// <summary>
		/// create default mapping.
		/// </summary>
		/// <returns>
		/// The create default mapping.
		/// </returns>
		private bool CreateDefaultMap()
		{
			try
			{
				if (!Directory.Exists(LocalConfigPath))
				{
					Console.WriteLine("Created local map directory in: {0}", LocalConfigPath);
					Directory.CreateDirectory(LocalConfigPath);
				}
				File.WriteAllText(MapFile, string.Empty);
				Console.WriteLine("Created {0} Map File", MapFile);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Unable to create map file: {0}", MapFile);
			}
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Coordinates [x, y] = 0;
				}
			}
			return Save();
		}

		/// <summary>
		/// Finds the map file.
		/// </summary>
		/// <returns>
		/// The map file.
		/// </returns>
		private bool FindMapFile()
		{
			foreach (string file in Directory.GetFiles(LocalConfigPath))
			{
				int E = file.Length - 4;
				string find = string.Format("{0}/{1}.bin", LocalConfigPath, Name);
				if (string.Format("{0}{1}", file.Substring(0, E), file.Substring(E, 4).ToLower()) == find)
				{
					return true;
				}
			}
			Console.WriteLine("Unable to Find Map File in: {0}", LocalConfigPath);
			return false;
		}

	}
}

