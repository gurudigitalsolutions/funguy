using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Drawing;
using Img = System.Drawing.Imaging;

namespace FunGuy
{
    public class TexLib
    {
        public TexLib()
        {
        }

        public static void InitTexturing()
		{
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
		}

        public static int CreateRGBTexture(int width, int height, byte[] rgb)
		{
            return CreateTexture(width, height, false, rgb);
		}

        public static int CreateRGBATexture(int width, int height, byte[] rgba)
		{
            return CreateTexture(width, height, true, rgba);
		}

        public static int CreateTextureFromBitmap(Bitmap bitmap)
		{
            Img.BitmapData data = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				Img.ImageLockMode.ReadOnly,
				Img.PixelFormat.Format32bppArgb);
            var tex = GiveMeATexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba,
				data.Width, data.Height,
				0,
				PixelFormat.Bgra,
				PixelType.UnsignedByte,
				data.Scan0);
            bitmap.UnlockBits(data);
            SetParameters();
            return tex;
		}

        public static int CreateTextureFromFile(string path)
		{
            return CreateTextureFromBitmap(new Bitmap(Bitmap.FromFile(path)));
		}

        public static int CreateTextureFromStream(System.IO.Stream stream)
		{
            return CreateTextureFromBitmap(new Bitmap(Bitmap.FromStream(stream)));
		}

        public static int CreateTexture(int width, int height, bool alpha, byte[] bytes)
		{
            //int expectedBytes = width * height * (alpha ? 4 : 3);
            //Debug.Assert (expectedBytes == bytes.Length);
            int tex = GiveMeATexture();
            Upload(width, height, alpha, bytes);
            SetParameters();
            return tex;
		}

        public static int GiveMeATexture()
		{
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            return tex;
		}

        public static void SetParameters()
		{
            GL.TexParameter(
				TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
			                 TextureParameterName.TextureMagFilter,
			                 (int)TextureMagFilter.Linear);
		}


        public static void Upload(int width, int height, bool alpha, byte[] bytes)
		{
            var internalFormat = alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb;
            var format = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;
            try{
                GL.TexImage2D<byte>(
				TextureTarget.Texture2D,
				0,
				internalFormat,
				width, height, 
				0,
				format,
				PixelType.UnsignedByte,
				bytes);
            }
            catch (Exception ex){
                Console.WriteLine(ex.Message);}
		}
    }
}

