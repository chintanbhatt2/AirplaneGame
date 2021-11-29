using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;

namespace AirplaneGame
{
    class Skybox
    {
        List<Bitmap> Faces = new List<Bitmap>();
        PrimativeObjects.Cube Box = new PrimativeObjects.Cube(@"..\..\..\..\Blender Objects\Cube.dae");
        public int TextureID;
        
        public Skybox()
        {

        }
        public Skybox(string[] faces)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                Faces.Add((Bitmap)Image.FromFile(faces[i]));
            }

            TextureID = loadCubeMap();
        }

        int loadCubeMap()
        {
            int texid = 0;
            texid = GL.GenTexture();

            GL.BindTexture(TextureTarget.TextureCubeMap, texid);


            int width = 0, height = 0, nrChannels = 0;

            for (int i = 0; i < Faces.Count; i++)
            {
                var data = Faces[i].LockBits(new Rectangle(0, 0, Faces[i].Width, Faces[i].Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Faces[i].PixelFormat);


                width = Faces[i].Width;
                height = Faces[i].Height;

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                Faces[i].UnlockBits(data);

            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);



            return texid;
        }

        public void Draw(Shader shader)
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, this.TextureID);
            
            Box.Draw(shader);
        }
    }
}
