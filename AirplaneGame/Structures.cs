using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class Structures
    {
        private const int sizeOfVertex = sizeof(float) * 8;
        public struct Vertex
        {
            public Vector3 Position, Normal; 
            public Vector2 TexCoord;
        }

        public struct Tex
        {
            public int id;
            public string type;
            public int al;
        }

        public class Triangle
        {
            public List<Vertex> Verticies = new List<Vertex>();
            public Vector3 NormalVector;

            public UInt16 AttributeCount;

            public void AddVertex(Vertex vert)
            {
                this.Verticies.Add(vert);
            }
            public Triangle(Vertex x, Vertex y, Vertex z)
            {
                this.Verticies.Append(x);
                this.Verticies.Append(y);
                this.Verticies.Append(z);
            }
            public Triangle()
            {

            }
        }

        public class Mesh
        {
            
            public Vertex[] vertices = { };
            public int[] indicies = { };
            Tex[] textures = { };

            public Mesh(Vertex[] vertices, int[] indicies, Tex[] textures)
            {
                this.vertices = vertices;
                this.indicies = indicies;
                this.textures = textures;

                setupMesh();
            }

            private int VAO, VBO, EBO;
            private void setupMesh()
            {
                VAO = GL.GenVertexArray();
                VBO = GL.GenBuffer();
                EBO = GL.GenBuffer();

                

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeOfVertex, vertices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ArrayBuffer, indicies.Length * sizeof(int), indicies, BufferUsageHint.StaticDraw);

                //positions
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeOfVertex, 0);

                //normals
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeOfVertex, sizeof(float) * 3);

                //tex coords
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeOfVertex, sizeof(float) * 6);

                GL.BindVertexArray(0);
            }
            public void Draw(Shader shader)
            {
                int diffuseNo = 1;
                int specularNo = 1;

                for(int i = 0; i < textures.Length; i++)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + 1);

                    string num = "";
                    string name = textures[i].type;
                    if (name == "teture_diffuse")
                    {
                        num = diffuseNo++.ToString();
                    }
                    else if (name == "texture_specular")
                    {
                        num = specularNo++.ToString();
                    }

                    shader.SetFloat(("material." + name + num), i);
                    GL.BindTexture(TextureTarget.Texture2D, textures[i].id);
                }

                GL.ActiveTexture(TextureUnit.Texture0);

                GL.BindVertexArray(VAO);
                GL.DrawElements(BeginMode.Triangles, indicies.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
  
            }
        }
    }
}
