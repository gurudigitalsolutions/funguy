using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FunGuy
{
    [Serializable()]
    public class Tree : Thing
    {

        public int FrontBackTexture = 0;
        public int LeftRightTexture = 0;

        public Tree()
        {
            TextureSet = "pinetree";

            Width = 2;
            Depth = 2;
            Height = 5;
            IsSolid = false;
            try
            {
                FrontBackTexture = Texture.TreeTexts [0].TexLibID;
                LeftRightTexture = Texture.TreeTexts [0].TexLibID;
            }
            catch
                (Exception ex)
            {
                Console.WriteLine("Loading tree textures: {0}", ex.Message);
            }

            int[] texids = this.SkinSet(TextureSet);

            FrontBackTexture = texids [0];
            LeftRightTexture = texids [1];
        }
        
        public Tree(int treeTexture)
        {
            Console.WriteLine("TreeTexture (Load): {0}", treeTexture);
            FrontBackTexture = treeTexture;
            LeftRightTexture = treeTexture;
        }

        public Tree(string skinname)
        {
            TextureSet = skinname;

            int[] texids = this.SkinSet(skinname);

            FrontBackTexture = texids [0];
            LeftRightTexture = texids [1];
        }

        public Tree(SerializationInfo info, StreamingContext ctxt)
        {
            //TreeTexture = (int)info.GetValue("TreeTexture", typeof(int));


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

                FrontBackTexture = texids [0];
                LeftRightTexture = texids [1];
            }
            catch (Exception)
            {
                //pfff
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
            //info.AddValue("TreeTexture", TreeTexture);
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
            int[] texList = new int[2];
            texList [0] = FrontBackTexture;
            texList [1] = LeftRightTexture;

            return texList;
		}

        public override void SetTextures(int[] texList)
		{
            FrontBackTexture = texList [0];
            LeftRightTexture = texList [1];
		}

        public override void Render()
		{
            //Console.WriteLine("TreeTexture (Render): {0}", TreeTexture);

            // Front to back
            GL.BindTexture(TextureTarget.Texture2D, FrontBackTexture);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);


            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3((float)((float)X + ((float)Width / 2)), Y, 4.01f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3((float)((float)X + ((float)Width / 2)), Y + Depth, 4.01f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3((float)((float)X + ((float)Width / 2)), Y + Depth, 4.01f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3((float)((float)X + ((float)Width / 2)), Y, 4.01f + Height);
            GL.End();


            // Left to right
            GL.BindTexture(TextureTarget.Texture2D, LeftRightTexture);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);


            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, (float)((float)Y + ((float)Depth / 2)), 4.01f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + Width, (float)((float)Y + ((float)Depth / 2)), 4.01f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, (float)((float)Y + ((float)Depth / 2)), 4.01f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X, (float)((float)Y + ((float)Depth / 2)), 4.01f + Height);
            GL.End();


		
		}

        public override void Update()
		{

		}
    }
}

