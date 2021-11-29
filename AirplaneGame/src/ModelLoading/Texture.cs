using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using Assimp;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;
using System.IO;

namespace AirplaneGame
{
    public class Texture
    {
        public int id;
        public string type;
        public string path;

        public int Handle;
        public static int TextureCount = 0;

        public Texture LoadFromFile(string path)
        {

            Handle = GL.GenTexture();


            GL.ActiveTexture(TextureUnit.Texture0 + TextureCount);
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            using (var image = new Bitmap(path))
            {

                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    image.Width,
                    image.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            TextureCount++;
            return new Texture(Handle);
        }

        public Texture(int glHandle)
        {
            Handle = glHandle;

        }

        public Texture(Assimp.EmbeddedTexture tex)
        {
            Handle = GL.GenTexture();

            

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            byte[] PixelList;
            if (tex.IsCompressed && tex.HasCompressedData)
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(tex.CompressedData))
                {
                    bmp = new Bitmap(ms);
                    
                }
                var data = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(  TextureTarget.Texture2D,
                                0,
                                PixelInternalFormat.Rgba,
                                bmp.Width,
                                bmp.Height,
                                0,
                                PixelFormat.Bgra,
                                PixelType.UnsignedByte,
                                data.Scan0);

            }
            else if (tex.HasNonCompressedData)
            {

                PixelList = new byte[tex.NonCompressedDataSize * 4];
                for (int i = 0; i < tex.NonCompressedDataSize; i++)
                {
                    PixelList[i * 4] = tex.NonCompressedData[i].R;
                    PixelList[i * 4 + 1] = tex.NonCompressedData[i].R;
                    PixelList[i * 4 + 2] = tex.NonCompressedData[i].R;
                    PixelList[i * 4 + 3] = tex.NonCompressedData[i].R;
                }



                GL.TexImage2D(TextureTarget.Texture2D,
                                0,
                                PixelInternalFormat.Rgba,
                                tex.Width,
                                tex.Height,
                                0,
                                PixelFormat.Bgra,
                                PixelType.UnsignedByte,
                                PixelList);
            }
            else
            {
                TextureCount--;
                return;
            }


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            TextureCount++;
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
