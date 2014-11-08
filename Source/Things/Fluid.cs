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
    public class Fluid : Thing
    {
        int PondFloor = 0;
        int PondSurface = 0;
        int AnimStep = 0;
        float AnimLowPoint = 3.01f;
        float AnimZOff = 3.51f;
        float AnimYOff = 0.0f;
        bool StepUp = true;
        int LastUpdate = 0;

        public Fluid()
        {
            TextureSet = "water";
            Width = 3;
            Depth = 3;
            Height = 1;

            int[] texids = this.SkinSet(TextureSet);
            PondFloor = texids [0];
            PondSurface = texids [1];
        }

        public Fluid(SerializationInfo info, StreamingContext ctxt)
        {
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

                PondFloor = texids [0];
                PondSurface = texids [1];
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to load texture set: \n{0}", ex.Message);
            }
        }

        public Fluid(string skinname)
        {
            TextureSet = skinname;
            int[] texids = this.SkinSet(skinname);

            PondFloor = texids [0];
            PondSurface = texids [1];
         

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
            texList [0] = PondFloor;
            texList [1] = PondSurface;
          

            return texList;
        }

        public override void SetTextures(int[] texList)
        {
            PondFloor = texList [0];
            PondSurface = texList [1];
          
        }

        public override void Render()
        {

            GL.BindTexture(TextureTarget.Texture2D, PondFloor);
            GL.Begin(BeginMode.Quads);

            for (int ex = 0; ex < Width; ex++)
            {
                for (int ey = 0; ey < Depth; ey++)
                {
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(X + ex, Y + ey, 4.01f);

                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(X + ex + 1, Y + ey, 4.01f);

                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(X + ex + 1, Y + ey + 1, 4.01f);

                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(X + ex, Y + ey + 1, 4.01f);
                }
            }

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, PondSurface);
            GL.Begin(BeginMode.Quads);

            for (int ex = 0; ex < Width; ex++)
            {
                for (int ey = Depth - 1; ey > -1; ey--)
                {
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(X + ex + 0.5f, Y + ey + AnimYOff + 0.5f, AnimZOff + 1f);

                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(X + ex + 1, Y + ey + AnimYOff + 0.5f, AnimZOff + 0.5f);

                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(X + ex + 0.5f, Y + ey + AnimYOff + 0.5f, AnimZOff);

                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(X + ex, Y + ey + AnimYOff + 0.5f, AnimZOff + 0.5f);


                    ///////

                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(X + ex + 0.5f, Y + ey + AnimYOff, AnimZOff + 1f);

                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(X + ex + 1, Y + ey + AnimYOff, AnimZOff + 0.5f);

                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(X + ex + 0.5f, Y + ey + AnimYOff, AnimZOff);

                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(X + ex, Y + ey + AnimYOff, AnimZOff + 0.5f);


                   
                }

            }

            GL.End();
        }

        public override void Update()
        {
            if (LastUpdate == 0) {
                LastUpdate = Game.Engine.LastTimeStamp; }

            AnimStep++;
            if (AnimStep >= 50) {
                AnimStep = 0; }

            AnimYOff = AnimStep / 100f;
        }
    }
}

