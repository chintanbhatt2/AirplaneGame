using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;
using Quaternion = OpenTK.Mathematics.Quaternion;


namespace AirplaneGame
{
    public class Model
    {
        public Model(string path)
        {
            loadModel(path);
        }

        public Model(Assimp.Node node, Assimp.Scene scene)
        {
            
            processNode(node, scene);
            RootMesh = meshes[0];

        }


        public void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader, position);
            }
        }


        public List<Mesh> meshes = new List<Mesh>();
        public string directory;
        public bool gammaCorrection = false;
        public OpenTK.Mathematics.Quaternion rotationVector = new OpenTK.Mathematics.Quaternion(new Vector3(0, 0, 0));
        public Vector3 position = new Vector3(0f);
        protected Vector3 scale = new Vector3(1);
        protected Mesh RootMesh;
        protected Matrix4 ModelTransform = Matrix4.Identity;
        public Dictionary<string, Mesh> MeshLocations = new Dictionary<string, Mesh>();


        public void rotateModel(float xRotation, float yRotation, float zRotation)
        {
            Quaternion eAngles = Quaternion.FromEulerAngles(xRotation, yRotation, zRotation);
            rotationVector = rotationVector * eAngles;

            updateTransformation(RootMesh);
        }

        public void moveModel(float xMove, float yMove, float zMove)
        {
            position += new Vector3(xMove, yMove, zMove);

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

        public void rotateMesh(float xRotation, float yRotation, float zRotation, string name)
        {
            Quaternion eAngles = Quaternion.FromEulerAngles(xRotation, yRotation, zRotation);
            MeshLocations[name].localRotation = MeshLocations[name].localRotation * eAngles;


            updateTransformation(MeshLocations[name]);
        }

        protected void updateTransformation(Mesh mesh)
        {
            if (mesh.Parent != null)
            {
                mesh.transformScale = mesh.Parent.transformScale * mesh.localScale;
                mesh.transformRotation = mesh.Parent.transformRotation * mesh.localRotation;
                mesh.transformPosition = mesh.Parent.transformPosition + new Vector3(mesh.localPosition[0] * mesh.transformScale[0], mesh.localPosition[1] * mesh.transformScale[1], mesh.localPosition[2] * mesh.transformScale[2]) ;

            }
            else
            {
                mesh.transformScale = mesh.localScale * scale;
                mesh.transformRotation = mesh.localRotation * rotationVector;
                //mesh.transformPosition = mesh.localPosition + new Vector3(position[0] * mesh.transformScale[0], position[1] * mesh.transformScale[1], position[2] * mesh.transformScale[2]);
                //mesh.transformMatrix = mesh.transformMatrix * ModelTransform;
            }
            for (int i = 0; i < mesh.Children.Count; i++)
            {
                updateTransformation(mesh.Children[i]);
            }

        }
        protected void loadModel(string path)
        {
            var context = new Assimp.AssimpContext();
            Assimp.Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            directory = path.Substring(0, path.LastIndexOf(@"\"));

            processNode(aiScene.RootNode, aiScene);
            RootMesh = meshes[0];
        }
        protected void processNode(Assimp.Node node, Assimp.Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];

                this.meshes.Add(processMesh(mesh, node));

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

        protected Mesh processMesh(Assimp.Mesh mesh, Assimp.Node node)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indicies = new List<int>();
            List<Texture> textures = new List<Texture>();

            //Loops through every vertex in mesh and adds them to the lists
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();

                vertex.Position = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Vertices[i]);

                if (mesh.HasNormals)
                {
                    vertex.Normal = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Normals[i]);
                }

                switch (mesh.VertexColorChannelCount)
                {
                    case 0:
                        vertex.Color = new Vector4(0.5f);
                        break;
                    case 1:
                        vertex.Color = new Vector4(mesh.VertexColorChannels[0][i].R, mesh.VertexColorChannels[0][i].G, mesh.VertexColorChannels[0][i].B, mesh.VertexColorChannels[0][i].A);
                        break;
                }

                if (mesh.HasTextureCoords(0))
                {
                    vertex.TexCoord.X = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).X;
                    vertex.TexCoord.Y = mesh.TextureCoordinateChannels.ElementAt(0).ElementAt(i).Y;
                    if (!(mesh.Tangents.Count == 0))
                    {
                        vertex.Tangent = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.Tangents[i]);
                    }

                    if (!(mesh.Tangents.Count == 0))
                    {
                        vertex.Bitangent = ASSIMPHelper.convertAssimpToOpenGLVec3(mesh.BiTangents[i]);
                    }
                }
                else
                {
                    vertex.TexCoord = new Vector2(0.0f, 0.0f);
                }
                vertices.Add(vertex);
            }

            //Copies index list from mesh
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];

                for (int j = 0; j < face.IndexCount; j++)
                {
                    indicies.Add(face.Indices[j]);
                }
            }

            //Creates mesh to return
            Mesh returnMesh = new Mesh(vertices.ToArray(), indicies.ToArray(), textures.ToArray());

            returnMesh.MaterialIndex = mesh.MaterialIndex;

            Matrix4 localMatrix = ASSIMPHelper.convertASSIMPtoOpenGLMat(node.Transform);

            //copies over original transforms before travering the tree
            returnMesh.localPosition = new Vector3(localMatrix.Column3);
            returnMesh.localRotation = localMatrix.ExtractRotation();
            returnMesh.localScale = localMatrix.ExtractScale();

            //traverses the tree to get the official transform
            Assimp.Node currentNode = node;
            Assimp.Matrix4x4 assimpTransform = node.Transform ;

            while(node.Parent != null && node.Parent.Name != "Scene")
            {
                assimpTransform = assimpTransform * node.Parent.Transform;
                node = node.Parent;
            }

            node = currentNode;

            //Now that we have the official transform we can apply it to the transforms
            Matrix4 assimpMatrix = ASSIMPHelper.convertASSIMPtoOpenGLMat(assimpTransform);
            returnMesh.transformPosition = new Vector3(assimpMatrix.Column3);
            returnMesh.transformRotation = assimpMatrix.ExtractRotation();
            returnMesh.transformScale = assimpMatrix.ExtractScale();

            returnMesh.Name = mesh.Name;

            return returnMesh;

        }



    }


}
