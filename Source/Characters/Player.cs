using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FunGuy
{

    public class Player
    {

#if DEBUG 
        public int MoveInterval = 100;
#else 
        public int MoveInterval = 250;
#endif


        public string FirstName = "";
        public string LastName = "";
        public int Age = 21;
        public int Class = 0;
        public int Race = 0;
        public int Level = 0;
        public string Graphics = "";

        public int Direction = 0;

        public bool DirectionUp = false;
        public bool DirectionDown = true;
        public bool DirectionLeft = false;
        public bool DirectionRight = false;

        public Dictionary<string, int> Classes = new Dictionary<string, int>
        {
            {"warrior", 0},
            {"mage", 1},
            {"monk", 2}
        };

        public Dictionary<string, int> Races = new Dictionary<string, int>
        {
            {"human", 0},
            {"elf", 1},
            {"dwarf", 2},
            {"troll", 3}
        };

        public Player(string charactername)
        {
            CharacterName = charactername;
            ParseCharacterFile();
            MapX = Game.Engine.WorldMapX;
            MapY = Game.Engine.WorldMapY;
        }

        public string CharacterName = string.Empty;

        public  List<CharacterTexture> Characters
        {
            get
            {
                if (!_IsCharLoaded){
                    _Characters = LoadTileSet();
                    _IsCharLoaded = true;}
                return _Characters;
            }
        }
        private  List<CharacterTexture> _Characters;
        private  bool _IsCharLoaded = false;
        public  int X;
        public  int Y;
        public int MapX;
        public int MapY;
        public  int LastMovedTime;
        public  int AnimStep = 0;
        public  bool CanMove
        {
            get {
                if (Environment.TickCount - LastMovedTime > MoveInterval)
            {
                    LastMovedTime = Environment.TickCount;
                    return true;
            }
                return false;
            }
        }


        private void ParseCharacterFile()
        {
            StreamReader sr;
            
            try {
                sr = new StreamReader(string.Format("{0}/Characters/{1}.txt", FunGuy.Game.configPath, CharacterName));
                string fileContents = sr.ReadToEnd();
                foreach(string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if(line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string[] pv = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if(pv.GetLength(0) != 2) { continue;}

                    switch(pv[0].ToLower())
                    {
                        case "firstname":
                            FirstName = pv[1];
                            break;
                        case "lastname":
                            LastName = pv[1];
                            break;
                        case "age":
                            int.TryParse(pv[1], out Age);
                            break;
                        case "class":
                            Class = Classes[pv[1].ToLower()];
                            break;
                        case "race":
                            Race = Races[pv[1].ToLower()];
                            break;
                        case "level":
                            int.TryParse(pv[1], out Level);
                            break;
                        case "graphics":
                            Graphics = pv[1];
                            break;
                        default:
                            break;
                    }
                }

                sr.Close();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //  whoo
            }
        }


        private  List<CharacterTexture> LoadTileSet()
        {
            Console.WriteLine("Loading character tileset for {0}", this.FirstName);
            List<CharacterTexture> retValue = new List<CharacterTexture>();
            try
            {
                StreamReader sr = new StreamReader(string.Format("{0}/Sets/Characters/{1}.txt", FunGuy.Game.configPath, Graphics));
                string fileContents = sr.ReadToEnd();
                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    string[] valueNameIndex = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    string textureName = valueNameIndex [1];
                    int mapValue = Int32.Parse(valueNameIndex [0]);
                    int index = Int32.Parse(valueNameIndex [2]);
                    string resourceID = string.Format("{0}/PNGs/Characters/{1}.png", FunGuy.Game.configPath, textureName);
                    int texLibID = TexLib.CreateTextureFromFile(resourceID);
                    float height = 3.0F;
                    if (mapValue < 0)
                    {
                        height = 0.0F;
                    }

                    CharacterTexture character = new CharacterTexture(textureName, mapValue, texLibID, index, height);
                    retValue.Add(character);
                }
                Console.WriteLine("Loaded Tile Set: {0}", "Characters");
                return retValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Tile Set: {0}", "Characters");
                return new List<CharacterTexture>();
            }
        }

        public void Render()
        {
            CharacterTexture myChar = Characters.Find(c => c.Value == 0 && c.Index == Direction);
            //Console.WriteLine("MyChar: {0}", myChar.TexLibID);
            GL.BindTexture(TextureTarget.Texture2D, myChar.TexLibID);
            GL.Begin(BeginMode.Quads);
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            float ly = Y;
            float ry = Y;
            float lx = X;
            float rx = (float)(X + 1);

            if (MapX != Game.Engine.Party [0].MapX)
            {

                if (MapX < Game.Engine.Party [0].MapX)
                {
                    lx = lx - Game.Engine.TheMap.Width;
                    rx = rx - Game.Engine.TheMap.Width;
                }
                else
                {
                    lx = lx + Game.Engine.TheMap.Width;
                    rx = rx + Game.Engine.TheMap.Width;
                }
            }

            if (MapY != Game.Engine.Party [0].MapY)
            {

                if (MapY < Game.Engine.Party [0].MapY)
                {
                    ly = ly + Game.Engine.TheMap.Height - 1;
                    ry = ry + Game.Engine.TheMap.Height - 1;
                }
                else
                {
                    ly = ly - Game.Engine.TheMap.Height;
                    ry = ry - Game.Engine.TheMap.Height;
                }
            }

            if (Direction == CharacterTexture.Down || 
                Direction == CharacterTexture.DownTwo ||
                Direction == CharacterTexture.DownThree ||
                Direction == CharacterTexture.Up ||
                Direction == CharacterTexture.UpTwo ||
                Direction == CharacterTexture.UpThree)
            {
                ly = ly + 0.5f;
                ry = ry + 0.5f;
            }
            else if (Direction == CharacterTexture.Left ||
                     Direction == CharacterTexture.LeftTwo ||
                     Direction == CharacterTexture.LeftThree)
            {
                ly = ly + 0.8f;
                ry = ry + 0.2f;

                lx = lx + 0.3f;
                rx = rx - 0.3f;
            }
            else if (Direction == CharacterTexture.Right ||
                     Direction == CharacterTexture.RightTwo ||
                     Direction == CharacterTexture.RightThree)
            {
                ly = ly + 0.2f;
                ry = ry + 0.8f;

                lx = lx + 0.3f;
                rx = rx - 0.3f;
            }

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(lx, ly, 4.05f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(rx, ry, 4.05f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(rx, ry, 4.05f + myChar.Height);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(lx, ly, 4.05f + myChar.Height);

            GL.End();
        }
    }
}

