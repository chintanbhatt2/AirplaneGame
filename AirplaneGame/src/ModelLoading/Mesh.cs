using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using System.Linq;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace AirplaneGame
{
    public class Mesh
    {

        public Matrix4 transformMatrix = Matrix4.Identity;
        public Vector3 transformPosition = Vector3.Zero;
        public Vector3 transformScale = Vector3.One;
        public Quaternion transformRotation = new Quaternion();

        public Matrix4 localMatrix = Matrix4.Identity;
        public Vector3 localPosition = Vector3.Zero;
        public Vector3 localScale = Vector3.One;
        public Quaternion localRotation = new Quaternion();


        public Vertex[] Vertices = { };
        public List<int>Indicies = new List<int>();
        
        List<Texture> Textures = new List<Texture>();
        private Material meshMaterial;
        
        public Mesh Parent;
        public List<Mesh> Children = new List<Mesh>();
        public string Name;
        public Vector3 RotationLock = new Vector3(0);
        public Material Materials;
        public int MaterialIndex;


        public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures, ref Mesh parent)
        {
            this.Vertices = vertices;
            this.Indicies = new List<int>(indicies);
            this.Textures = new List<Texture>(textures);

            Parent = parent;

            setupMesh();
        }

        public Mesh(Assimp.Mesh mesh, Assimp.Node node)
        {
            Vertices = new Vertex[mesh.VertexCount];
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex v = new Vertex();
                v.Position = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Vertices[i]);
                
                if (mesh.HasNormals) v.Normal = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Normals[i]);

                switch (mesh.VertexColorChannelCount)
                {
                    case 0:
                        v.Color = new Vector4(0.5f);
                        break;
                    case 1:
                        v.Color = new Vector4(mesh.VertexColorChannels[0][i].R, mesh.VertexColorChannels[0][i].G, mesh.VertexColorChannels[0][i].B, mesh.VertexColorChannels[0][i].A);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;

                }
                if (mesh.HasTextureCoords(0))
                {
                    Vector2 vec;

                    vec.X = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).X;
                    vec.Y = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).Y;
                    v.TexCoord = vec;

                    if (!(mesh.Tangents.Count == 0))
                    {
                        v.Tangent = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Tangents[i]);
                    }

                    if (!(mesh.BiTangents.Count == 0))
                    {
                        v.Bitangent = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.BiTangents[i]);
                    }
                }
                else
                {
                    v.TexCoord = new Vector2(0.0f, 0.0f);
                }
                Vertices[i] = v;
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];

                for (int j = 0; j < face.IndexCount; j++)
                {
                    Indicies.Add(face.Indices[j]);
                }
            }
        }

        public Mesh(Assimp.Mesh mesh)
        {

            for(int i = 0; i < mesh.VertexCount; i++)
            {

            }
        }
        public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures, Material materials, ref Mesh parent)
        {
            this.Vertices = vertices;
            this.Indicies = new List<int>(indicies);
            this.Textures = new List<Texture>(textures);

            Parent = parent;

            Materials = materials;

            setupMesh();
        }

        public Mesh(int width, int height)
        {
            this.Vertices = new Vertex[width * height];
            this.Indicies.Capacity = ((width - 1) * (height - 1)) * 6;

        }

        public Mesh(Vertex[] vertices, int[] indicies, Texture[] textures)
        {
            this.Vertices = vertices;
            this.Indicies = new List<int>(indicies);
            this.Textures = new List<Texture>(textures);




            setupMesh();
        }

        public Mesh(Vertex[] vertices, int[] indicies)
        {
            this.Vertices = vertices;
            this.Indicies = new List<int>(indicies);




            setupMesh();
        }

        private int VAO, VBO, EBO;

        private float[] getVertexArray()
        {
            float[] varray = new float[Vertices.Length * 12];

            for (int i = 0; i < Vertices.Length; i++)
            {
                varray[i * 12 + 0] = Vertices[i].Position.X;
                varray[i * 12 + 1] = Vertices[i].Position.Y;
                varray[i * 12 + 2] = Vertices[i].Position.Z;
                varray[i * 12 + 3] = Vertices[i].Normal.X;
                varray[i * 12 + 4] = Vertices[i].Normal.Y;
                varray[i * 12 + 5] = Vertices[i].Normal.Z;
                varray[i * 12 + 6] = Vertices[i].TexCoord.X;
                varray[i * 12 + 7] = Vertices[i].TexCoord.Y;
                varray[i * 12 + 8] = Vertices[i].Color.X;
                varray[i * 12 + 9] = Vertices[i].Color.Y;
                varray[i * 12 + 10] = Vertices[i].Color.Z;
                varray[i * 12 + 11] = Vertices[i].Color.W;
            }
            return varray;
        }


        public void CalculateVertexNormals()
        {

            for (int vertex = 0; vertex < Vertices.Length; vertex++)
                Vertices[vertex].Normal = Vector3.Zero;

            for (int index = 0; index < Indicies.Count; index += 3)
            {
                int vertexA = Indicies[index];
                int vertexB = Indicies[index + 1];
                int vertexC = Indicies[index + 2];


                var edgeAB = Vertices[vertexB].Position - Vertices[vertexA].Position;
                var edgeAC = Vertices[vertexC].Position - Vertices[vertexA].Position;

                var areaWeightedNormal = Vector3.Cross(edgeAB, edgeAC);


                Vertices[vertexA].Normal += areaWeightedNormal;
                Vertices[vertexB].Normal += areaWeightedNormal;
                Vertices[vertexC].Normal += areaWeightedNormal;
            }

            for (int vertex = 0; vertex < Vertices.Length; vertex++)
                Vertices[vertex].Normal = Vector3.Normalize(Vertices[vertex].Normal);
        }

        public void setupMesh() 
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
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indicies.Count * sizeof(int), Indicies.ToArray(), BufferUsageHint.StaticDraw);

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
        public void Draw(Shader shader, Vector3 globalPosition)
        {
            transformMatrix =  Matrix4.CreateScale(transformScale)  * Matrix4.CreateTranslation(transformPosition) * Matrix4.CreateFromQuaternion(transformRotation);
            transformMatrix = transformMatrix * Matrix4.CreateTranslation(globalPosition);
            shader.SetMatrix4("model", transformMatrix);
            
            if (Materials != null)
            {
                shader.SetVector3("mat.Ka", new Vector3(Materials.Ambient));
                shader.SetVector3("mat.Kd", new Vector3(Materials.Diffuse));
                shader.SetVector3("mat.Ks", new Vector3(Materials.Specular));
                shader.SetFloat("mat.Shininess", Materials.Shininess * Materials.ShininessStrength);
            }


            GL.BindVertexArray(VAO);

            GL.DrawElements(BeginMode.Triangles, Indicies.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

        }

        public void Draw(Shader shader, Camera cam)
        {
            transformMatrix = Matrix4.CreateFromQuaternion(transformRotation) * Matrix4.CreateScale(transformScale) * Matrix4.CreateTranslation(transformPosition);
            shader.SetMatrix4("model", transformMatrix);
            Matrix4 ModelView = transformMatrix * cam.GetViewMatrix();
            if (shader._uniformLocations.ContainsKey("normalMatrix"))
            {
                shader.SetMatrix3("normalMatrix", new Matrix3(Matrix4.Transpose(Matrix4.Invert(ModelView))));

            }

            if (Materials != null)
            {
                shader.SetVector3("mat.Ka", new Vector3(Materials.Ambient));
                shader.SetVector3("mat.Kd", new Vector3(Materials.Diffuse));
                shader.SetVector3("mat.Ks", new Vector3(Materials.Specular));
                shader.SetFloat("mat.Shininess", Materials.Shininess * Materials.ShininessStrength);
            }
            GL.BindVertexArray(VAO);

            GL.DrawElements(BeginMode.Triangles, Indicies.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

        }
    }
}
