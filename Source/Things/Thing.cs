using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace FunGuy
{
    //[Serializeable()]
    public abstract class Thing : ISerializable
    {
        public int X = 0;
        public int Y = 0;
        public int Width = 1;
        public int Depth = 1;
        public int Height = 1;
        public bool IsSolid = true;
        public int Index = 0;
        //public string[] TextureSetsAvailable;

        public string TextureSet = "";
        
        private static bool _IsAllThingsLoaded = false;
        private static List<Thing> _AllThings = new List<Thing>();
        public static List<Thing> AllThings
        {
            get{
                if (!_IsAllThingsLoaded){
                    _AllThings = _LoadDefaultThings();
                    _IsAllThingsLoaded = true;
                }

                return _AllThings;
            }
        }

        private static List<Thing> _LoadDefaultThings()
        {
            List<Thing> retList = new List<Thing>();
            retList.Add(new Tree("pinetree"));
            retList.Add(new House("woodpanel"));
            retList.Add(new Fluid("water"));
            return retList;
        }

        public Thing()
        {
        }

        public Thing(SerializationInfo info, StreamingContext ctxt)
        {
            X = (int)info.GetValue("X", typeof(int));
            Y = (int)info.GetValue("Y", typeof(int));
            Width = (int)info.GetValue("Width", typeof(int));
            Depth = (int)info.GetValue("Depth", typeof(int));
            Height = (int)info.GetValue("Height", typeof(int));
            IsSolid = (bool)info.GetValue("IsSolid", typeof(bool));

            try
            {
                TextureSet = (string)info.GetValue("TextureSet", typeof(string));
            }
            catch (Exception)
            {
                //pfff
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Width", Width);
            info.AddValue("Depth", Depth);
            info.AddValue("Height", Height);
            info.AddValue("IsSolid", IsSolid);
            info.AddValue("TextureSet", TextureSet);
		}

        public abstract void Render();

        public abstract void Update();


     

        public virtual int[] TextureList()
		{
            int[] texList = new int[1];
            texList [0] = 0;

            return texList;
		}

        public virtual void SetTextures(int[] texList)
		{
            //
		}

        public int[] SkinSet(string skinname)
        {
            string resourcePath = string.Format("{0}/Sets/Things/{1}.txt", FunGuy.Game.configPath, this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf(".") + 1));
            string pngPath = string.Format("{0}/PNGs/Things/{1}/", FunGuy.Game.configPath, this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf(".") + 1));
            int[] returnTexIDs = new int[0];

            Console.WriteLine(resourcePath);
//            return new string[0];

            StreamReader sr;

            try
            {
                sr = new StreamReader(resourcePath);
                string fileContents = sr.ReadToEnd();

                foreach (string line in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }

                    string[] words = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                    if (words [0].ToLower() == skinname.ToLower())
                    {
                        returnTexIDs = new int[words.GetLength(0) - 1];
                        for (int i = 1; i < words.GetLength(0); i++)
                        {
                            returnTexIDs [i - 1] = TexLib.CreateTextureFromFile(string.Format("{0}{1}.png", pngPath, words [i]));
                        }

                    }

                }


                Console.WriteLine("Loaded Thing Skin: {0}", skinname);

                return returnTexIDs;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Unable to Load Thing Skins: {0}", skinname);
                return null;
            }

        }
    }
}

