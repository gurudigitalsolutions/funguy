using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


namespace FunGuy
{

    public abstract class TextSet
    {
        public string Name = string.Empty;
		public int TexLibID = 0;
		public int Value = 0;



		public TextSet(string name, int texLibID, int value)
		{
			Name = name;
			TexLibID = texLibID;
			Value = value;
		}
        public TextSet(string name, int texLibID)
        {
            Name = name;
            TexLibID = texLibID;
        }

        public TextSet()
        {
        }

        /*
        public TextSet(SerializationInfo info, StreamingContext ctxt)
        {
            //X = (int)info.GetValue("X", typeof(int));
            setName = (string)info.GetValue("setName", typeof(string));
            Value = (int)info.GetValue("Value", typeof(int));
            TexLibID = (int)info.GetValue("TexLibID", typeof(int));
            Index = (int)info.GetValue("Index", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("setName", setName);
            info.AddValue("Value", Value);
            info.AddValue("TexLibID", TexLib);
            info.AddValue("Index", Index);
        }
        */


		




		//static virtual List<TextSet> Load(string textType, string Name);
/*        
		{
            List<TextSet> retValue = new List<TextSet>();
            string resourcePath = string.Format("{0}/PNGs/{1}", FunGuy.Game.configPath, textureType);
            Console.WriteLine(resourcePath);
            try
            {
                Console.WriteLine("{0}/{1}.txt", resourcePath, setName);
                StreamReader sr = new StreamReader(string.Format("{0}/{1}.txt", resourcePath, setName));
                string fileContents = sr.ReadToEnd();

                foreach (string setName in fileContents.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (setName.Trim().Substring(0, 1) == "#")
                    {
                        continue;
                    }
                    int texLibID = TexLib.CreateTextureFromFile(string.Format("{0}/{1}.png", resourcePath, setName));
                    TextSet texture = new TextSet(setName, texLibID);
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
*/    
    }

}