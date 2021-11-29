using System;
using System.Collections.Generic;
using System.Text;
using Assimp;
using OpenTK.Mathematics;


namespace AirplaneGame
{
    public class Scene
    {
        public List<Model> Models = new List<Model>();
        public List<Mesh> MainModels = new List<Mesh>();
        public List<Light> Lights = new List<Light>();
        public List<Material> Materials = new List<Material>();
        public List<Texture> Textures = new List<Texture>();

        private Assimp.Scene aiScene;
        private Assimp.Node rootNode;


        public Scene(string path)
        {
            Assimp.AssimpContext context = new Assimp.AssimpContext();
            aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            rootNode = aiScene.RootNode;

            if(aiScene.HasLights)
            {
                for(int i = 0; i < aiScene.LightCount; i++)
                {
                    Lights.Add(new Light(aiScene.Lights[i]));
                }
            }

            if (aiScene.HasMaterials)
            {
                for(int i = 0; i < aiScene.MaterialCount; i++)
                {
                    Materials.Add(new Material(aiScene.Materials[i]));
                }
            }

            if (aiScene.HasTextures)
            {
                for(int i = 0; i < aiScene.TextureCount; i++)
                {
                    Textures.Add(new Texture(aiScene.Textures[i]));
                }
            }

            if (aiScene.HasMeshes)
            {
                for (int i = 0; i < aiScene.RootNode.ChildCount; i++)
                {
                    if (aiScene.RootNode.Children[i].HasMeshes)
                    {
                        Models.Add(new Model(aiScene.RootNode.Children[i], aiScene));
                    }
                }
            }

            for (int i = 0; i < Models.Count; i++)
            {
                foreach(KeyValuePair<string, Mesh> entry in Models[i].MeshLocations)
                {
                    entry.Value.Materials = Materials[entry.Value.MaterialIndex];
                }
            }

        }

    }


}
