using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

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
        
        private static bool _IsAllThingsLoaded = false;
        private static List<Thing> _AllThings = new List<Thing>();
        public static List<Thing> AllThings
        {
            get{
                if (!_IsAllThingsLoaded){
                    _AllThings = _LoadDefaultThings();
                }
                return _AllThings;
            }
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
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Width", Width);
            info.AddValue("Depth", Depth);
            info.AddValue("Height", Height);
            info.AddValue("IsSolid", IsSolid);
		}

        public abstract void Render();

        public abstract void Update();


        private static List<Thing> _LoadDefaultThings()
        {
            List<Thing> retList = new List<Thing>();
            retList.Add(new PineTree());
            return retList;
        }

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
    }
}

