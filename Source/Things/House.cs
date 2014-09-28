using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace FunGuy
{
    [Serializable()]
    public class House : Thing
    {
        public int FrontWall = 0;
        public int RearWall = 0;
        public int Floor = 0;
        public int Roof = 0;
        public int LeftWall = 0;
        public int RightWall = 0;


        public House()
        {
            TextureSet = "woodpanel";

            Width = 4;
            Depth = 4;
            Height = 1;
            Index = 0;
            IsSolid = true;
            FrontWall = 10;
            RearWall = 10;
            Floor = 2;
            Roof = 9;
            LeftWall = 10;
            RightWall = 10;

            int[] texids = this.SkinSet("woodpanel");

            FrontWall = texids [0];
            RearWall = texids [1];
            LeftWall = texids [2];
            RightWall = texids [3];
            Floor = texids [4];
            Roof = texids [5];
        }

        public House(SerializationInfo info, StreamingContext ctxt)
        {
//            FrontWall = (int)info.GetValue("FrontWall", typeof(int));
//            RearWall = (int)info.GetValue("RearWall", typeof(int));
//            Floor = (int)info.GetValue("Floor", typeof(int));
//            Roof = (int)info.GetValue("Roof", typeof(int));
//            LeftWall = (int)info.GetValue("LeftWall", typeof(int));
//            RightWall = (int)info.GetValue("RightWall", typeof(int));


            // from thing
            X = (int)info.GetValue("X", typeof(int));
            Y = (int)info.GetValue("Y", typeof(int));
            Width = (int)info.GetValue("Width", typeof(int));
            Depth = (int)info.GetValue("Depth", typeof(int));
            Height = (int)info.GetValue("Height", typeof(int));
            IsSolid = (bool)info.GetValue("IsSolid", typeof(bool));

            try
            {
                TextureSet = (string)info.GetValue("TextureSet", typeof(string));

                int[] texids = this.SkinSet(TextureSet);

                FrontWall = texids [0];
                RearWall = texids [1];
                LeftWall = texids [2];
                RightWall = texids [3];
                Floor = texids [4];
                Roof = texids [5];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to load texture set: \n{0}", ex.Message);
            }
          
            
           
        }

//        public House(int frontWall, int rearWall, int leftWall, int rightWall, int roof, int floor)
//        {
//            FrontWall = frontWall;
//            RearWall = rearWall;
//            LeftWall = leftWall;
//            RightWall = rightWall;
//            Roof = roof;
//            Floor = floor;
//        }

        public House(string skinname)
        {
            TextureSet = skinname;
            int[] texids = this.SkinSet(skinname);

            FrontWall = texids [0];
            RearWall = texids [1];
            LeftWall = texids [2];
            RightWall = texids [3];
            Floor = texids [4];
            Roof = texids [5];
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
//            info.AddValue("FrontWall", FrontWall);
//            info.AddValue("RearWall", RearWall);
//            info.AddValue("Floor", Floor);
//            info.AddValue("Roof", Roof);
//            info.AddValue("LeftWall", LeftWall);
//            info.AddValue("RightWall", RightWall);
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Width", Width);
            info.AddValue("Depth", Depth);
            info.AddValue("Height", Height);
            info.AddValue("IsSolid", IsSolid);
            info.AddValue("TextureSet", TextureSet);
		}

        public override int[] TextureList()
		{
            int[] texList = new int[6];
            texList [0] = FrontWall;
            texList [1] = RearWall;
            texList [2] = LeftWall;
            texList [3] = RightWall;
            texList [4] = Roof;
            texList [5] = Floor;

            return texList;
		}

        public override void SetTextures(int[] texList)
		{
            FrontWall = texList [0];
            RearWall = texList [1];
            LeftWall = texList [2];
            RightWall = texList [3];
            Roof = texList [4];
            Floor = texList [5];
		}

        public override void Render()
		{
            // Flea-zord
            GL.BindTexture(TextureTarget.Texture2D, Floor);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);


            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y, 4.01f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X, Y + Depth, 4.01f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y + Depth, 4.01f);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X + Width, Y, 4.01f);
            GL.End();


            //	Front wall
            GL.BindTexture(TextureTarget.Texture2D, FrontWall);
            GL.Begin(BeginMode.Quads);
			
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y, 4.0f + Height);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + Width, Y, 4.0f + Height);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y, 4.0f);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X, Y, 4.0f);

            GL.End();

            // left wall
            GL.BindTexture(TextureTarget.Texture2D, LeftWall);
            GL.Begin(BeginMode.Quads);
			
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y, 4.0f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X, Y + Depth, 4.0f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X, Y + Depth, 4.0f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X, Y, 4.0f + Height);

            GL.End();

            // right wall
            GL.BindTexture(TextureTarget.Texture2D, RightWall);
            GL.Begin(BeginMode.Quads);
			
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X + Width, Y, 4.0f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + Width, Y + Depth, 4.0f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y + Depth, 4.0f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X + Width, Y, 4.0f + Height);

            GL.End();

            //	Back wall
            GL.BindTexture(TextureTarget.Texture2D, RearWall);
            GL.Begin(BeginMode.Quads);
			
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y + Depth, 4.0f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X, Y + Depth, 4.0f + Height);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y + Depth, 4.0f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X + Width, Y + Depth, 4.0f);

            GL.End();

            // Roof
            GL.BindTexture(TextureTarget.Texture2D, Roof);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);

            // Top left
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y + Depth, 4f + Height);
            // Top right
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + Width, Y + Depth, 4f + Height);
            // Bottotm left
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y, 4f + Height);
            // Bottom right
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X, Y, 4f + Height);
            GL.End();

		}

        public override void Update()
		{

		}
        /*
                    Thing pineTree = new PineTree(TheMap.Textures.Find(x => x.Name == "pine1").TexLibID);
            pineTree.Depth = 2;
            pineTree.Width = 2;
            pineTree.Height = 4;
            pineTree.X = 30;
            pineTree.Y = 30;

            Thing.AllThings.Add(pineTree);
//          TheMap.AddThing(pineTree);

            Thing defaultHouse = new DefaultHouse(
                TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
                TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
                TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
                TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
                TheMap.Textures.Find(x => x.Name == "thatch1").TexLibID, 
                TheMap.Textures.Find(x => x.Name == "rockroad1").TexLibID);
            defaultHouse.Depth = 2;
            defaultHouse.Height = 2;
            defaultHouse.Width = 2;
            defaultHouse.X = 25;
            defaultHouse.Y = 25;

            Thing.AllThings.Add(defaultHouse);
//          TheMap.AddThing(defaultHouse);
*/
    }
}

