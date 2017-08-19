﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace MyEngine
{
    public class Texture2D : Texture, IUnloadable
    {


        static public Texture2D whiteTexture { private set; get; }
        static public Texture2D greyTexture { private set; get; }
        static public Texture2D blackTexture { private set; get; }


        int textureHandle;


        internal Texture2D(int textureHandle)
        {
            this.textureHandle = textureHandle;
        }
        internal Texture2D(Resource resource)
        {
            this.Load(resource);
        }

        internal static void InitTexture2D() {
            whiteTexture = new Texture2D("internal/white.png");
            greyTexture = new Texture2D("internal/grey.png");
            blackTexture = new Texture2D("internal/black.png");
        }


        public void Unload()
        {
            GL.DeleteTexture(textureHandle);
        }

        void Load(Resource resource)
        {
            
            // better performance: 2d array, 2d texture buffer

            textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            bool useMimMaps = true;

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GetTextureMinFilter(useMimMaps));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GetTextureMagFilter(useMimMaps));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GetTextureWrapMode());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GetTextureWrapMode());

            // ???
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, this.anisoLevel);

            Bitmap bmp = new Bitmap(resource);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
            
            bmp.UnlockBits(bmp_data);

            if (useMimMaps)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            

        }
        public bool IsOnGpu()
        {
            var yes = new bool[1];
            GL.AreTexturesResident(1, new int[] { textureHandle }, yes);
            return yes[0];
        }
        public int GetNativeTextureID()
        {
            return textureHandle;
        }
        
    }
}
