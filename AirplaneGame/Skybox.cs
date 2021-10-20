using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace AirplaneGame
{
    class Skybox
    {
        List<Image> Faces = new List<Image>();
        PrimativeObjects.Cube Box = new PrimativeObjects.Cube(@"..\..\..\..\Blender Objects\Cube.dae");
        public int TextureID;
        
        public Skybox()
        {

        }
        public Skybox(string[] faces)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                Faces.Add(Image.FromFile(faces[i]));
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
                byte[] textureByte = (byte[])new ImageConverter().ConvertTo(Faces[i], typeof(byte[]));
                width = Faces[i].Width;
                height = Faces[i].Height;
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.Byte, textureByte);
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
            Box.Draw(shader);
        }
    }
}
