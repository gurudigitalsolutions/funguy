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
    public class PineTree : Thing
    {
        public int TreeTexture = 0;

        public PineTree()
        {
            Width = 2;
            Depth = 2;
            Height = 5;
            IsSolid = false;
            TreeTexture = Texture.TreeTexts [0].TexLibID;
        }
        
        public PineTree(int treeTexture)
        {
            Console.WriteLine("TreeTexture (Load): {0}", treeTexture);
            TreeTexture = treeTexture;
        }

        public PineTree(SerializationInfo info, StreamingContext ctxt)
        {
            TreeTexture = (int)info.GetValue("TreeTexture", typeof(int));


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
            }
            catch (Exception)
            {
                //pfff
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
            info.AddValue("TreeTexture", TreeTexture);
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
            int[] texList = new int[1];
            texList [0] = TreeTexture;

            return texList;
		}

        public override void SetTextures(int[] texList)
		{
            TreeTexture = texList [0];
		}

        public override void Render()
		{
            //Console.WriteLine("TreeTexture (Render): {0}", TreeTexture);
            GL.BindTexture(TextureTarget.Texture2D, TreeTexture);
            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);


            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X + (Width / 2), Y, 4.01f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + (Width / 2), Y + Depth, 4.01f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + (Width / 2), Y + Depth, 4.01f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X + (Width / 2), Y, 4.01f + Height);
            GL.End();

            GL.Begin(BeginMode.Quads);

            GL.Normal3(-1.0f, 0.0f, 0.0f);


            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(X, Y + (Depth / 2), 4.01f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(X + Width, Y + (Depth / 2), 4.01f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(X + Width, Y + (Depth / 2), 4.01f + Height);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(X, Y + (Depth / 2), 4.01f + Height);
            GL.End();


		
		}

        public override void Update()
		{

		}
    }
}

