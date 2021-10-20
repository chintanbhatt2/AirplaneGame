using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;


namespace AirplaneGame
{
    public class Model
    {
        public Model(string path)
        {
            loadModel(path);
        }
        public void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }


        public Structures.Texture[] textures_loaded = { };
        public List<Structures.Mesh> meshes = new List<Structures.Mesh>();
        public string directory;
        public bool gammaCorrection = false;
        public OpenTK.Mathematics.Quaternion rotationVector = new OpenTK.Mathematics.Quaternion(new Vector3(0, 0, 0));
        protected Vector3 position = new Vector3(0f);
        protected Vector3 scale = new Vector3(1);
        protected Structures.Mesh RootMesh;
        protected Matrix4 ModelTransform = Matrix4.Identity;
        protected Dictionary<string, Structures.Mesh> MeshLocations = new Dictionary<string, Structures.Mesh>();


        public void rotateModel(float xRotation, float yRotation, float zRotation)
        {
            rotationVector = new OpenTK.Mathematics.Quaternion(MathHelper.DegreesToRadians(xRotation), MathHelper.DegreesToRadians(yRotation), MathHelper.DegreesToRadians(zRotation));
            ModelTransform = Matrix4.CreateFromQuaternion(rotationVector) * Matrix4.CreateTranslation(position) * Matrix4.CreateScale(scale);
            rotationVector.Normalize();


            updateTransformation(RootMesh);
        }


        public Matrix4 getModelTransform()
        {
            return ModelTransform;
        }

        public OpenTK.Mathematics.Quaternion getRotationVector()
        {
            return rotationVector;
        }
        public void lockMeshRotation(bool x, bool y, bool z, string name)
        {
            if (x)
            {
                MeshLocations[name].RotationLock.X = 1.0f;
            }
            if (y)
            {
                MeshLocations[name].RotationLock.Y = 1.0f;
            }
            if (z)
            {
                MeshLocations[name].RotationLock.Z = 1.0f;
            }
        }

        public void rotateMesh(float xRotation, float yRotation, float zRotation, string name)
        {
            Matrix4 xRot = Matrix4.Identity, yRot = Matrix4.Identity, zRot = Matrix4.Identity, totalRotation = Matrix4.Identity;

            if (MeshLocations[name].RotationLock.X == 0)
            {
                Matrix4.CreateRotationX(xRotation, out xRot);

            }
            if (MeshLocations[name].RotationLock.Y == 0)
            {
                Matrix4.CreateRotationY(yRotation, out yRot);

            }
            if (MeshLocations[name].RotationLock.Z == 0)
            {
                Matrix4.CreateRotationZ(zRotation, out zRot);
            }

            totalRotation = xRot * yRot * zRot;


            MeshLocations[name].localMatrix *= totalRotation;
            updateTransformation(MeshLocations[name]);
        }

        public void setMeshAngle(Vector3 eulerAngles, string name)
        {
            Matrix4 xRot = new Matrix4(), yRot = Matrix4.Identity, zRot = Matrix4.Identity, totalRotation = Matrix4.Identity;

            if (MeshLocations[name].RotationLock.X == 0)
            {
                Matrix4.CreateRotationX(eulerAngles.X, out xRot);

            }
            if (MeshLocations[name].RotationLock.Y == 0)
            {
                Matrix4.CreateRotationY(eulerAngles.Y, out yRot);

            }
            if (MeshLocations[name].RotationLock.Z == 0)
            {
                Matrix4.CreateRotationZ(eulerAngles.Z, out zRot);
            }

            totalRotation = xRot * yRot * zRot;
            totalRotation.Normalize();

            totalRotation.Column3 = MeshLocations[name].localMatrix.Column3;
            MeshLocations[name].localMatrix = totalRotation;
            updateTransformation(MeshLocations[name]);
        }

        public void resetMeshRotation(string name)
        {
            MeshLocations[name].transformMatrix = Matrix4.Identity;
        }

        protected void updateTransformation(Structures.Mesh mesh)
        {

            if (mesh.Parent != null)
            {
                mesh.transformMatrix = mesh.Parent.transformMatrix * mesh.localMatrix;
            }
            else
            {
                mesh.transformMatrix = mesh.transformMatrix * ModelTransform;
            }
            for (int i = 0; i < mesh.Children.Count; i++)
            {
                updateTransformation(mesh.Children[i]);
            }

        }
        protected void loadModel(string path)
        {
            var context = new Assimp.AssimpContext();
            Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            directory = path.Substring(0, path.LastIndexOf(@"\"));

            processNode(aiScene.RootNode, aiScene);
            RootMesh = meshes[0];
        }
        protected void processNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];

                this.meshes.Add(processMesh(mesh, scene, node));

                try
                {
                MeshLocations.Add(node.Name, meshes[^1]);

                }
                catch(System.ArgumentException)
                {

                }
                if (node.Parent.Name != "Scene")
                {
                    MeshLocations[node.Parent.Name].Children.Add(meshes[^1]);
                    meshes[^1].Parent = MeshLocations[node.Parent.Name];
                }
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                processNode(node.Children[i], scene);

            }
        }

        protected Structures.Mesh processMesh(Mesh mesh, Scene scene, Node node)
        {
            List<Structures.Vertex> vertices = new List<Structures.Vertex>();
            List<int> indicies = new List<int>();
            List<Structures.Texture> textures = new List<Structures.Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Structures.Vertex vertex = new Structures.Vertex();
                Vector3 vector;

                vector.X = mesh.Vertices[i].X;
                vector.Y = mesh.Vertices[i].Y;
                vector.Z = mesh.Vertices[i].Z;

                vertex.Position = vector;

                if (mesh.HasNormals)
                {
                    vector.X = mesh.Normals[i].X;
                    vector.Y = mesh.Normals[i].Y;
                    vector.Z = mesh.Normals[i].Z;

                    vertex.Normal = vector;
                }

                switch (mesh.VertexColorChannelCount)
                {
                    case 0:
                        vertex.Color = new Vector4(0.5f);
                        break;
                    case 1:
                        vertex.Color = new Vector4(mesh.VertexColorChannels[0][i].R, mesh.VertexColorChannels[0][i].G, mesh.VertexColorChannels[0][i].B, mesh.VertexColorChannels[0][i].A);
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
                    vertex.TexCoord = vec;

                    if (!(mesh.Tangents.Count == 0))
                    {
                        vector.X = mesh.Tangents[i].X;
                        vector.Y = mesh.Tangents[i].Y;
                        vector.Z = mesh.Tangents[i].Z;
                        vertex.Tangent = vector;
                    }

                    if (!(mesh.Tangents.Count == 0))
                    {
                        vector.X = mesh.BiTangents[i].X;
                        vector.Y = mesh.BiTangents[i].Y;
                        vector.Z = mesh.BiTangents[i].Z;
                        vertex.Bitangent = vector;
                    }
                }
                else
                {
                    vertex.TexCoord = new Vector2(0.0f, 0.0f);
                }
                vertices.Add(vertex);
            }
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];

                for (int j = 0; j < face.IndexCount; j++)
                {
                    indicies.Add(face.Indices[j]);
                }
            }

            Material material = scene.Materials[mesh.MaterialIndex];

            Structures.Texture[] diffuseMaps = loadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            textures.Concat(diffuseMaps);

            Structures.Texture[] specularMaps = loadMaterialTextures(material, TextureType.Specular, "texture_specular");
            textures.Concat(specularMaps);

            Structures.Texture[] normalMaps = loadMaterialTextures(material, TextureType.Normals, "texture_normal");
            textures.Concat(normalMaps);

            Structures.Texture[] heightMaps = loadMaterialTextures(material, TextureType.Height, "texture_height");
            textures.Concat(heightMaps);

            Structures.Mesh returnMesh = new Structures.Mesh(vertices.ToArray(), indicies.ToArray(), textures.ToArray());

            Matrix4x4 assimpTransform = node.Transform;
            Node previousNode = node;
            assimpTransform = node.Transform;
            returnMesh.localMatrix = convertASSIMPtoOpenGLMat(node.Transform);
            while (node.Parent != null && node.Parent.Name != "Scene")
            {
                assimpTransform = assimpTransform * node.Parent.Transform;
                node = node.Parent;
            }
            node = previousNode;
            returnMesh.transformMatrix = convertASSIMPtoOpenGLMat(assimpTransform);
            returnMesh.Name = mesh.Name;
            return returnMesh;
        }

        protected Matrix4 convertASSIMPtoOpenGLMat(Matrix4x4 assimp)
        {
            //  It's ugly but what can you do ¯\_(ツ)_/¯
            return new Matrix4(assimp.A1, assimp.A2, assimp.A3, assimp.A4,
                                assimp.B1, assimp.B2, assimp.B3, assimp.B4,
                                assimp.C1, assimp.C2, assimp.C3, assimp.C4,
                                assimp.D1, assimp.D2, assimp.D3, assimp.D4);
        }

        protected Structures.Texture[] loadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<Structures.Texture> textures = new List<Structures.Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot str;
                mat.GetMaterialTexture(type, i, out str);

                bool skip = false;
                for (int j = 0; j < textures_loaded.Length; j++)
                {
                    if (textures_loaded[j].path == null)
                    {
                        textures.Add(textures_loaded[j]);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    //TODO: Fix this
                }
            }



            return textures.ToArray();
        }

        protected int LoadTextureFromFile(string path, string directory, bool gamma)
        {
            string filename = path;
            filename = directory + '/' + filename;

            int textureID;
            textureID = GL.GenTexture();


            int width = 0, height = 0, nrComponents = 0;
            byte[] data = { };

            using (var ms = new System.IO.MemoryStream())
            {
                System.Drawing.Image imin = System.Drawing.Image.FromFile(path);
                imin.Save(ms, imin.RawFormat);
                data = ms.ToArray();
            }

            if (data != null)
            {

                PixelInternalFormat iformat = new PixelInternalFormat();
                PixelFormat format = new PixelFormat();


                if (nrComponents == 1)
                {
                    iformat = PixelInternalFormat.R16f;
                    format = PixelFormat.Red;
                }

                else if (nrComponents == 3)
                {
                    iformat = PixelInternalFormat.Rgb;
                    format = PixelFormat.Rgb;
                }
                else if (nrComponents == 4)
                {
                    iformat = PixelInternalFormat.Rgba;
                    format = PixelFormat.Rgba;
                }

                GL.BindTexture(TextureTarget.Texture2D, textureID);

                GL.TexImage2D(TextureTarget.Texture2D, 0, iformat, width, height, 0, format, PixelType.UnsignedByte, data); //TODO: Fix this
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                int[] GL_REPEAT = { (int)OpenTK.Graphics.ES20.TextureWrapMode.Repeat };

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GL_REPEAT);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GL_REPEAT);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GL_REPEAT);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }
            else
            {
                System.Console.WriteLine("Texture failed to load at path", path);
            }

            return textureID;
        }
    }


}
