using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FunGuy
{
	[Serializable()]
	public class Map : ISerializable
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

			YellowFadeTextures = new int[64];
			for(int eyell = 0; eyell < 32; eyell++)
			{
				YellowFadeTextures[eyell] = TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, (byte)(eyell + 64),
																		255, 255, 0, (byte)(eyell + 64),
																		255, 255, 0, (byte)(eyell + 64),
					255, 255, 0, (byte)(eyell + 64)});
			}

			for(int eyell = 32; eyell < 64; eyell++)
			{
				YellowFadeTextures[eyell] = TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, (byte)(128 - eyell),
																		255, 255, 0, (byte)(128 - eyell),
																		255, 255, 0, (byte)(128 - eyell),
					255, 255, 0, (byte)(128 - eyell)});
			}

			Textures.Add("alpha", TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, 128,
																				255, 255, 0, 128,
																				0, 255, 0, 128,
																				0, 255, 0, 128}));

		}

		public Map (SerializationInfo info, StreamingContext ctxt)
		{
			Name = (string)info.GetValue("Name", typeof(string));
			Width = (int)info.GetValue("Width", typeof(int));
			Height = (int)info.GetValue("Height", typeof(int));
			TileSet = (string)info.GetValue("TileSet", typeof(string));
			Coordinates = (int[,])info.GetValue("Coordinates", typeof(int[,]));

			Textures = new Dictionary<string, int> ();
			TextureSetIDs = new Dictionary<int, int> ();
			LoadTileSet ();

			YellowFadeTextures = new int[64];
			for(int eyell = 0; eyell < 32; eyell++)
			{
				YellowFadeTextures[eyell] = TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, (byte)(eyell + 64),
																		255, 255, 0, (byte)(eyell + 64),
																		255, 255, 0, (byte)(eyell + 64),
					255, 255, 0, (byte)(eyell + 64)});
			}

			for(int eyell = 32; eyell < 64; eyell++)
			{
				YellowFadeTextures[eyell] = TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, (byte)(128 - eyell),
																		255, 255, 0, (byte)(128 - eyell),
																		255, 255, 0, (byte)(128 - eyell),
					255, 255, 0, (byte)(128 - eyell)});
			}

			Textures.Add("alpha", TexLib.CreateRGBATexture(2, 2, new byte[]{255, 255, 0, 128,
																				255, 255, 0, 128,
																				0, 255, 0, 128,
																				0, 255, 0, 128}));
		}

		public string Name { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
		public string TileSet { get; set; }
		public int[,] Coordinates {
			get {
				return _Coordinates;
			}
			set {
				_Coordinates = value;
			}
		}
		private int[,] _Coordinates;
		public Dictionary<string, int> Textures;
		public Dictionary<int, int> TextureSetIDs;
		public int[] YellowFadeTextures;

		public static Map Load (string filename)
		{
			FileStream fs = File.Open(filename, FileMode.Open);
			BinaryFormatter bform = new BinaryFormatter();

			Map loadedmap = (Map)bform.Deserialize(fs);
			fs.Close();

			return loadedmap;
		}

		public bool Save ()
		{

			FileStream fs = File.Create("/tmp/funguymap.map");
			BinaryFormatter formatter = new BinaryFormatter();


			formatter.Serialize(fs, this);
			fs.Close();

			return true;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Name", Name);
			info.AddValue("Width", Width);
			info.AddValue("Height", Height);
			info.AddValue("TileSet", TileSet);
			info.AddValue("Coordinates", Coordinates);


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
			}
			return true;
		}

	}
}

