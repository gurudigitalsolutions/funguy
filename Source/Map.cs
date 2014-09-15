using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;


using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FunGuy
{
	[Serializable()]
	public class Map:ISerializable
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
//			Console.WriteLine("Loading Map File: {0}", Load());
		}

		public Map()
		{
			new Map("", "default", 64, 64);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Name", Name);
			info.AddValue("TileSet", TileSet);
			info.AddValue("Width", Width);
			info.AddValue("Height", Height);
			info.AddValue("Coordinates", Coordinates);
			info.AddValue("StartX", StartX);
			info.AddValue("StartY", StartY);
			info.AddValue("Things", Things);
			info.AddValue("NextThingID", NextThingID);

		}
		
		public Map(SerializationInfo info, StreamingContext ctxt)
		{
			Name = (string)info.GetValue("Name", typeof(string));
			TileSet = (string)info.GetValue("TileSet", typeof(string));
			Width = (int)info.GetValue("Width", typeof(int));
			Height = (int)info.GetValue("Height", typeof(int));
			Coordinates = (int[,])info.GetValue("Coordinates", typeof(int[,]));
			StartX = (int)info.GetValue("StartX", typeof(int));
			StartY = (int)info.GetValue("StartY", typeof(int));

			try
			{
				Things = (List<Thing>)info.GetValue("Things", typeof(List<Thing>));
				Things.Sort((x, y) => y.Y.CompareTo(x.Y));
			}
			catch (Exception)
			{
				//haha
			}

			try
			{
				NextThingID = (int)info.GetValue("NextThingID", typeof(int));
			}catch (Exception)
			{
				// ajaj
			}
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
		public List<MyTexture> Textures = new List<MyTexture>();
		/// <summary>
		/// The things.
		/// </summary>
		public List<Thing> Things = new List<Thing>();
		/// <summary>
		/// The player x.
		/// </summary>
		public int StartX;
		/// <summary>
		/// The player y.
		/// </summary>
		public int StartY;

		public int WorldX;
		public int WorldY;

		public int NextThingID = 0;
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
				return string.Format("{0}/{1}_{2}.map", LocalConfigPath, WorldX, WorldY);
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
				//return string.Format("{0}/{1}/{2}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "FunGuy", "Maps");
				return string.Format("{0}/{1}", StarterKit.Game.configPath, "Maps");
			}
		}


		public static Map Loader(string mapFile)
		{
//			Console.WriteLine("Loading map {0}", mapFile);
			if (!File.Exists(mapFile))
			{
				Console.WriteLine("Map file {0} does not exist!", mapFile);
				return null;
			}

			Map returnMap;
			FileStream fs = File.Open(mapFile, FileMode.Open);
			BinaryFormatter bform = new BinaryFormatter();
			BinaryReader reader = new BinaryReader(fs);
			reader.BaseStream.Position = 0;
			returnMap = (Map)bform.Deserialize(fs);

			returnMap.Textures = new List<MyTexture>();

			returnMap.LoadPlayerPosition();
			returnMap.LoadTileSet();

			if (returnMap.Things != null && returnMap.Things.Count > 0)
			{
				for (int et = 0; et < returnMap.Things.Count; et++)
				{
					returnMap.Things [et].Index = et;
					returnMap.NextThingID = et + 1;
				}
			}
			return returnMap;
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
				//Console.WriteLine("Loaded coordinates from: {0}", MapFile);
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
//			if (!FindMapFile())
//			{
//				return false;
//			}

			FileStream fs;
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				fs = new FileStream(MapFile, FileMode.Create, FileAccess.Write);
				bf.Serialize(fs, this);
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
			string resourcePath = string.Format("{1}/PNGs/TileSets/{0}", TileSet, StarterKit.Game.configPath);
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
//				Console.WriteLine("Loaded Tile Set: {0}", TileSet);

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
			string resourcePath = StarterKit.Game.configPath;
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
//					Console.WriteLine("nameXY[0]: {0}", nameXY [0]);
					if (nameXY [0] == Name)
					{
						StartX = Int32.Parse(nameXY [1]);
						StartY = Int32.Parse(nameXY [2]);
//						Console.WriteLine("loaded player position for: {0} X: {1} Y: {1}", Name, StartX, StartY);
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

		public void UnloadTextures()
		{
			if (Textures.Count > 0)
			{
				foreach (MyTexture etex in Textures)
				{
					GL.DeleteTexture(etex.TexLibID);
				}
			}
			Console.WriteLine("Unloaded textures");
		}

		public void RenderThings(int miny, int maxy)
		{

			foreach (Thing ething in Things.FindAll(x => x.Y >= miny && x.Y < maxy))
			{
				ething.Render();
			}
		}

		public bool IsThingAt(int x, int y)
		{
//			Thing thing = Things.Find(a => (a.X >= x && a.X + a.Width <= x) && (a.Y >= y && a.Y + a.Depth <= y));
			Thing thing = Things.Find(a => (
				a.X + a.Width - 1 >= x
				&& a.X <= x
				&& a.Y + a.Depth - 1 >= y
				&& a.Y <= y)
			);
			if (thing == null)
			{
				return false;
			}

			return true;
		}

		public void AddThing(Thing thing)
		{

			thing.Index = NextThingID;
			Things.Add(thing);

			NextThingID++;

			for (int et = 0; et < Things.Count; et++)
			{
				Things [et].Index = et;
			}
		}
	}
}

