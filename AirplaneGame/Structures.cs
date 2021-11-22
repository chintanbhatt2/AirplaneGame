using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class Structures
    {
        private const int MAX_BONE_INFLUENCE = 4;
        public class Vertex
        {
            public Vector3 Position = new Vector3(0); 
            public Vector3 Normal = new Vector3(0); 
            public Vector2 TexCoord = new Vector2(0);
            public Vector3 Tangent = new Vector3(0);
            public Vector3 Bitangent = new Vector3(0);
            public Vector4 Color = new Vector4(0, 0, 0, 1);

            public Vertex()
            {
                Position = new Vector3(0);
                Normal = new Vector3(0);
                Color = new Vector4(0, 0, 0, 1);
            }
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
            public Matrix4 transformMatrix = Matrix4.CreateTranslation(0, 0, 0) * Matrix4.CreateRotationX(0) * Matrix4.CreateScale(1);
            public Matrix4 localMatrix = Matrix4.CreateTranslation(0, 0, 0) * Matrix4.CreateRotationX(0) * Matrix4.CreateScale(1);
            public Mesh Parent;
            public List<Mesh> Children = new List<Mesh>();
            public string Name;
            public Vector3 RotationLock = new Vector3(0);

            public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures, ref Mesh parent)
            {
                this.vertices = vertices;
                this.indicies = indicies;
                this.textures = textures;

                Parent = parent;


                setupMesh();
            }

            public Mesh(int width, int height)
            {
                this.vertices = new Vertex[width * height];
                this.indicies = new int[((width - 1) * (height - 1)) * 6];

            }

            public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures)
            {
                this.vertices = vertices;
                this.indicies = indicies;
                this.textures = textures;




                setupMesh();
            }

            public Mesh(Vertex[] vertices, int[] indicies)
            {
                this.vertices = vertices;
                this.indicies = indicies;




                setupMesh();
            }

            private int VAO, VBO, EBO;

            private float[] getVertexArray()
            {
                float[] varray = new float[vertices.Length*12];

                for (int i = 0; i < vertices.Length; i++)
                {
                    varray[i * 12 + 0] = vertices[i].Position.X;
                    varray[i * 12 + 1] = vertices[i].Position.Y;
                    varray[i * 12 + 2] = vertices[i].Position.Z;
                    varray[i * 12 + 3] = vertices[i].Normal.X;
                    varray[i * 12 + 4] = vertices[i].Normal.Y;
                    varray[i * 12 + 5] = vertices[i].Normal.Z;
                    varray[i * 12 + 6] = vertices[i].TexCoord.X;
                    varray[i * 12 + 7] = vertices[i].TexCoord.Y;
                    varray[i * 12 + 8] = vertices[i].Color.X;
                    varray[i * 12 + 9] = vertices[i].Color.Y;
                    varray[i * 12 + 10] = vertices[i].Color.Z;
                    varray[i * 12 + 11] = vertices[i].Color.W;
                }
                return varray;
            }


            public void CalculateVertexNormals()
            {

                for (int vertex = 0; vertex < vertices.Length; vertex++)
                    vertices[vertex].Normal = Vector3.Zero;

                for (int index = 0; index < indicies.Length; index += 3)
                {
                    int vertexA = indicies[index];
                    int vertexB = indicies[index + 1];
                    int vertexC = indicies[index + 2];

                    
                    var edgeAB = vertices[vertexB].Position - vertices[vertexA].Position;
                    var edgeAC = vertices[vertexC].Position - vertices[vertexA].Position;

                    var areaWeightedNormal = Vector3.Cross(edgeAB, edgeAC);


                    vertices[vertexA].Normal += areaWeightedNormal;
                    vertices[vertexB].Normal += areaWeightedNormal;
                    vertices[vertexC].Normal += areaWeightedNormal;
                }

                for (int vertex = 0; vertex < vertices.Length; vertex++)
                    vertices[vertex].Normal = Vector3.Normalize(vertices[vertex].Normal);
            }

            public void setupMesh() //TODO: update code for C# reference based
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
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12 * sizeof(float), 0);

                //normals
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 12 * sizeof(float), sizeof(float) * 3);

                //tex coords
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 12 * sizeof(float), sizeof(float) * 6);

                //vertex colors
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 12 * sizeof(float), sizeof(float) * 8);

                GL.BindVertexArray(0);
            }
            public void Draw(Shader shader)
            {
                int diffuseNo = 1;
                int specularNo = 1;
                shader.SetMatrix4("model", transformMatrix);
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
