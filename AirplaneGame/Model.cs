using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace AirplaneGame
{
    class Model
    {
        public Model(string path)
        {
            loadModel(path);
        }
        public void Draw(Shader shader)
        {
            for(int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }


        public Structures.Texture[] textures_loaded = { };
        public List<Structures.Mesh> meshes = new List<Structures.Mesh>();
        public string directory;
        public bool gammaCorrection = false;

        private void loadModel(string path)
        {
            var context = new Assimp.AssimpContext();
            Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            directory = path.Substring(0, path.LastIndexOf(@"\"));

            processNode(aiScene.RootNode, aiScene);
        }
        private void processNode(Node node, Scene scene)
        {
            for(int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                this.meshes.Add(processMesh(mesh, scene));
            }

            for(int i = 0; i < node.ChildCount; i++)
            {
                processNode(node.Children[i], scene);
            }
        }

        private Structures.Mesh processMesh(Mesh mesh, Scene scene)
        {
            List<Structures.Vertex> vertices = new List<Structures.Vertex>();
            List<int> indicies = new List<int>();
            List<Structures.Texture> textures = new List<Structures.Texture>();

            for(int i = 0; i < mesh.VertexCount; i++)
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

                if (mesh.HasTextureCoords(0))
                {
                    Vector2 vec;

                    vec.X = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).X;
                    vec.Y = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).Y;
                    vertex.TexCoord = vec;

                    vector.X = mesh.Tangents[i].X;
                    vector.Y = mesh.Tangents[i].Y;
                    vector.Z = mesh.Tangents[i].Z;
                    vertex.Tangent = vector;

                    vector.X = mesh.BiTangents[i].X;
                    vector.Y = mesh.BiTangents[i].Y;
                    vector.Z = mesh.BiTangents[i].Z;
                    vertex.Bitangent = vector;
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


            return new Structures.Mesh(vertices.ToArray(), indicies.ToArray(), textures.ToArray());
            
        }

        private Structures.Texture[] loadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<Structures.Texture> textures = new List<Structures.Texture>();
            for(int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot str;
                mat.GetMaterialTexture(type, i, out str);

                bool skip = false;
                for (int j = 0; j < textures_loaded.Length; j++)
                {
                    if(textures_loaded[j].path == null)
                    {
                        textures.Add(textures_loaded[j]);
                        skip = true;
                        break;
                    }
                }

                if(!skip)
                {
                    //TODO: Fix this
                }
            }



            return textures.ToArray();
        }

        private int LoadTextureFromFile(string path, string directory, bool gamma)
        {
            string filename = path;
            filename = directory + '/' + filename;

            int textureID;
            textureID = GL.GenTexture();
            

            int width = 0, height = 0, nrComponents = 0;
            byte[] data = { };
            
            using (var ms = new System.IO.MemoryStream() )
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
