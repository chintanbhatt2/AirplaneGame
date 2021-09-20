using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK.Mathematics;


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
            for(int i = 0; i < meshes.Length; i++)
            {
                meshes[i].Draw(shader);
            }
        }


        public Structures.Tex[] textures_loaded = { };
        public Structures.Mesh[] meshes = {};
        public string directory;
        public bool gammaCorrection;

        private void loadModel(string path)
        {
            var context = new Assimp.AssimpContext();
            Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            directory = path.Substring(0, path.LastIndexOf("/"));

            processNode(aiScene.RootNode, aiScene);
        }
        private void processNode(Node node, Scene scene)
        {
            for(int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Append(processMesh(mesh, scene));
            }

            for(int i = 0; i < node.ChildCount; i++)
            {
                processNode(node.Children[i], scene);
            }
        }

        private Structures.Mesh processMesh(Mesh mesh, Scene scene)
        {
            Structures.Vertex[] vertices = { };
            int[] indicies = { };
            Structures.Tex[] textures = { };

            for(int i = 0; i < mesh.VertexCount; i++)
            {
                Structures.Vertex vertex;
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

                    vector.X = mesh.BiTangents[i].X;
                    vector.Y = mesh.BiTangents[i].Y;
                    vector.Z = mesh.BiTangents[i].Z;
                }
                else
                    vertex.TexCoord = new Vector2(0.0f, 0.0f);

                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    Face face = mesh.Faces[i];

                    for (int j = 0; j < face.IndexCount; j++)
                    {
                        indicies.Append(face.Indices[j]);
                    }
                }

                Material material = scene.Materials[mesh.MaterialIndex];

                Structures.Tex[] diffuseMaps = loadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                textures.Concat(diffuseMaps);

                Structures.Tex[] specularMaps = loadMaterialTextures(material, TextureType.Specular, "texture_specular");
                textures.Concat(specularMaps);

                Structures.Tex[] normalMaps = loadMaterialTextures(material, TextureType.Normals, "texture_normal");
                textures.Concat(normalMaps);

                Structures.Tex[] heightMaps = loadMaterialTextures(material, TextureType.Height, "texture_height");
                textures.Concat(heightMaps);


                return new Structures.Mesh(vertices, indicies, textures);
            }

            return new Structures.Mesh(vertices, indicies, textures);
        }

        private Structures.Tex[] loadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            Structures.Tex[] textures;
            for(int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot str;
                mat.GetMaterialTexture(type, i, out str);

                bool skip = false;
                for (int j = 0; j < textures_loaded.Length; j++)
                {
                    if(textures_loaded[j])
                }
            }
        }


    }
}
