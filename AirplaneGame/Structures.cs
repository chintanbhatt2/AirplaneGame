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
        private const int MAX_BONE_INFLUENCE = 4;
        public class Vertex
        {
            public Vector3 Position, Normal; 
            public Vector2 TexCoord;
            public Vector3 Tangent, Bitangent;
            int[] m_BoneIDs;
            float[] m_Weights;
        }

        public struct Texture
        {
            public int id;
            public string type;
            public string path;
        }


        public class Mesh
        {
            
            public Vertex[] vertices = { };
            public int[] indicies = { };
            Texture[] textures = { };
            public Matrix4 transformMatrix = Matrix4.Identity;
            public Matrix4 localMatrix = Matrix4.Identity;
            public Mesh Parent;
            public Mesh[] Children;
            public string Name;

            public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures, ref Mesh parent)
            {
                this.vertices = vertices;
                this.indicies = indicies;
                this.textures = textures;

                Parent = parent;


                setupMesh();
            }

            public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures)
            {
                this.vertices = vertices;
                this.indicies = indicies;
                this.textures = textures;




                setupMesh();
            }

            private int VAO, VBO, EBO;

            private float[] getVertexArray()
            {
                float[] varray = new float[vertices.Length*3 + vertices.Length * 3 + vertices.Length * 2];

                for (int i = 0; i < vertices.Length; i++)
                {
                    varray[i * 8 + 0] = vertices[i].Position.X;
                    varray[i * 8 + 1] = vertices[i].Position.Y;
                    varray[i * 8 + 2] = vertices[i].Position.Z;
                    varray[i * 8 + 3] = vertices[i].Normal.X;
                    varray[i * 8 + 4] = vertices[i].Normal.Y;
                    varray[i * 8 + 5] = vertices[i].Normal.Z;
                    varray[i * 8 + 6] = vertices[i].TexCoord.X;
                    varray[i * 8 + 7] = vertices[i].TexCoord.Y;
                }
                return varray;
            }

            private void setupMesh() //TODO: update code for C# reference based
            {
                VAO = GL.GenVertexArray();
                VBO = GL.GenBuffer();
                EBO = GL.GenBuffer();
                float[] vertexArray = getVertexArray();

                int vertexArraySize = vertexArray.Length * sizeof(float);

                

                GL.BindVertexArray(VAO);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexArraySize, vertexArray, BufferUsageHint.StaticDraw);


                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Length * sizeof(int), indicies, BufferUsageHint.StaticDraw);

                //positions
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                //normals
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), sizeof(float) * 3);

                //tex coords
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), sizeof(float) * 6);

                GL.BindVertexArray(0);
            }
            public void Draw(Shader shader)
            {
                int diffuseNo = 1;
                int specularNo = 1;
                shader.SetMatrix4("model", transformMatrix);
                //TODO: Figure out how to set transform matrix
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


                GL.BindVertexArray(VAO);

                GL.DrawElements(BeginMode.Triangles, indicies.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.ActiveTexture(TextureUnit.Texture0);
  
            }
        }
    }
}
