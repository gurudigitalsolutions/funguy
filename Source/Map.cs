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
        
        public string Name;
        public string TileSet;
        public int Width;
        public int Height;
        public int[,] Coordinates;
        public List<Texture> Textures = new List<Texture>();
        public List<Thing> Things = new List<Thing>();
        public int StartX;
        public int StartY;
        public int WorldX;
        public int WorldY;
        public int NextThingID = 0;

        public Map(string name, string tileSet, int width, int height)
        {
            Name = name;
            TileSet = tileSet;
            Width = width;
            Height = height;

            Coordinates = new int[width, height];
            Textures = new List<Texture>();

            LoadTileSet();
            LoadPlayerPosition();
        }


        public Map()
        {
            new Map("", "default", 64, 64);
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
                ;
            }


            try
            {
                NextThingID = (int)info.GetValue("NextThingID", typeof(int));
            }
            catch (Exception)
            {
                // ajaj
            }
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
            try 
            {
                info.AddValue("Things", Things);
            }
            catch (Exception ex)
            {
                Console.Write("Things didn't load correctly: \n{0}", ex.Message);
            }
            info.AddValue("NextThingID", NextThingID);

        }



        public string MapFile
        {
            get
            {
                return string.Format("{0}/{1}/{2}.map", LocalConfigPath, WorldX, WorldY);
            }
        }


        public  string LocalConfigPath
        {
            get
            {
                return string.Format("{0}/{1}", FunGuy.Game.configPath, "Maps");
            }
        }


        public static Map Loader(string mapFile)
        {
            if (!File.Exists(mapFile))
            {
                Console.WriteLine("Map file {0} does not exist!", mapFile);
                return null;
            }

            Map retValue;
            FileStream fs = File.Open(mapFile, FileMode.Open);
            BinaryFormatter bform = new BinaryFormatter();
            BinaryReader reader = new BinaryReader(fs);
            reader.BaseStream.Position = 0;
            retValue = (Map)bform.Deserialize(fs);

            retValue.Textures = new List<Texture>();

            retValue.LoadPlayerPosition();
            retValue.LoadTileSet();


            //retValue.Things = new List<Thing>();

            return retValue;
        }


        public bool Save()
        {
            FileStream fs = new FileStream(MapFile, FileMode.Create, FileAccess.Write);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, this);
                Console.WriteLine("Saved coordinate to file: {0}", MapFile);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error Serializing to: {0}", MapFile);
                fs.Close();
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
            string resourcePath = string.Format("{0}/PNGs/TileSets", FunGuy.Game.configPath);
            StreamReader sr;

            try
            {
                sr = new StreamReader(string.Format("{0}/{1}.txt", resourcePath, TileSet));
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
                    Texture texture = new Texture(textureName, mapValue, texLibID);
                    Textures.Add(texture);
                }
//              Console.WriteLine("Loaded Tile Set: {0}", TileSet);

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
            string resourcePath = FunGuy.Game.configPath;
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
//                  Console.WriteLine("nameXY[0]: {0}", nameXY [0]);
                    if (nameXY [0] == Name)
                    {
                        StartX = Int32.Parse(nameXY [1]);
                        StartY = Int32.Parse(nameXY [2]);
//                      Console.WriteLine("loaded player position for: {0} X: {1} Y: {1}", Name, StartX, StartY);
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


        public static void UnloadTextures(List<Texture> textures)
        {
            if (textures.Count < 1)
            {
                return;
            }
            foreach (Texture etex in textures)
            {
                GL.DeleteTexture(etex.TexLibID);
            }
            Console.WriteLine("Unloaded textures");
        }


        public void UnloadTextures()
        {
            UnloadTextures(Textures);
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
//          Thing thing = Things.Find(a => (a.X >= x && a.X + a.Width <= x) && (a.Y >= y && a.Y + a.Depth <= y));
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
            if (Things.Count == 0)
            {
                Things.Add(thing);
            }
            else
            {
                int highestindex = 0;
                for (int et = 0; et < Things.Count; et++)
                {
                    if (Things [et].Index > highestindex) {
                        highestindex = Things [et].Index; }
                }

                thing.Index = highestindex + 1;
                Things.Add(thing);
            }
        }
    }
}

