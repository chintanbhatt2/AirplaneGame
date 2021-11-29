using System.Collections;
using System;
using System.Collections.Generic;
using SharpNoise;
using OpenTK.Mathematics;


namespace AirplaneGame
{
    public class Terrain
    {
        const float viewerMoveThresholdForChunkUpdate = 25f;
        const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
        Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
        List<TerrainChunk> terrainChunkVis = new List<TerrainChunk>();
        public TerrainChunk[] Ter = new TerrainChunk[9];
        public int seed, oct; 
        public double freq;
        public Vector2 viewerPosition;
        private Vector2 viewerPositionOld;
        private int chunksInView = 0;
        private float maxViewDist;

        public Terrain(Vector3 pos, int s, int o, double f)
        {
            seed = s;
            oct = o;
            freq = f;
            maxViewDist = 6;
            chunksInView = 2;

            updateVisibleChunks(pos);
        }


        public void Update(Vector3 camPosition)
        {
            viewerPosition = new Vector2(camPosition.X, camPosition.Z);
            if ((viewerPositionOld - viewerPosition).LengthSquared > sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                updateVisibleChunks(camPosition);
            }
        }

        public void updateVisibleChunks(Vector3 camPosition)
        {
            for (int i = 0; i < terrainChunkVis.Count; i++)
            {
                terrainChunkVis[i].isVisible = false;
            }

            terrainChunkVis.Clear();

            int currentChunkCoordX = (int)MathF.Round(viewerPosition.X / TerrainChunk.xSize);
            int currentChunkCoordZ = (int)MathF.Round(viewerPosition.Y / TerrainChunk.zSize);

            for (int yOffset = -chunksInView; yOffset <= chunksInView; yOffset++)
            {
                for (int xOffset = -chunksInView; xOffset <= chunksInView; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordZ + yOffset);

                    if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                    {
                        //terrainChunkDict[viewedChunkCoord].Update(new Vector2 (camPosition.X, camPosition.Z));
                        //if (terrainChunkDict[viewedChunkCoord].isVisible)
                        //{
                            terrainChunkVis.Add(terrainChunkDict[viewedChunkCoord]);
                        //}

                    }
                    else
                    {
                        terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(seed, oct, freq, viewedChunkCoord.X * TerrainChunk.xSize, viewedChunkCoord.Y * TerrainChunk.zSize, 0));
                    }
                }
            }

        }

        public void UpdateChunks(Vector3 position)
        {
            float xPosition = ((int)(position.X / TerrainChunk.xSize) * TerrainChunk.xSize) + (TerrainChunk.xSize);
            float zPosition = ((int)(position.Z / TerrainChunk.zSize) * TerrainChunk.zSize) + (TerrainChunk.zSize);
        }
        public void Draw(Shader shader)
        {
            for(int i = 0; i < terrainChunkVis.Count; i++)
            {
                terrainChunkVis[i].terrainMesh.Draw(shader, new Vector3(0, 0,0));
            }
        }
        public void Draw(Shader shader, Camera cam)
        {
            for (int i = 0; i < terrainChunkVis.Count; i++)
            {
                terrainChunkVis[i].terrainMesh.Draw(shader, new Vector3(0f));
            }
        }

    }
    public class TerrainChunk
    {

        public static int xSize = 241;
        public static int zSize = 241;
        public static int heightMultiplier = 30;

        public int levelOfDetail;

        private SharpNoise.Modules.Perlin perlin;
        private SharpNoise.NoiseMap noiseMap;
        private SharpNoise.Builders.PlaneNoiseMapBuilder noiseMapBuilder;
        private int[] singleMeshIndicies;
        public Mesh terrainMesh;
        private int xLocation = 0;
        private int zLocation = 0;
        private int indexCounter = 0;
        private int simp = 0;
        private int xSizeAdjusted;
        private int zSizeAdjusted;
        public bool isVisible;
        private float maxViewDistance = 1000;


        public static Vector4[] colorMap = {
            new Vector4(  0,   0, 128, 255)/255, // deeps
            new Vector4(  0,   0, 255, 255)/255, // shallow
            new Vector4(  0, 128, 255, 255)/255, // shore
            new Vector4(240, 240,  64, 255)/255, // sand
            new Vector4( 32, 160,   0, 255)/255, // grass
            new Vector4(224, 224,   0, 255)/255, // dirt
            new Vector4(128, 128, 128, 255)/255, // rock
            new Vector4(255, 255, 255, 255)/255, // snow
        };

        public TerrainChunk(int seed, int oct, double freq, float xL, float zL, int lod)
        {
            isVisible = false;
            levelOfDetail = lod;
            xLocation = (int)System.Math.Round((xL / xSize) * xSize);
            zLocation = (int)(zL / zSize) * zSize;
            simp = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            xSizeAdjusted = ((xSize - 1) / simp) + 1;
            zSizeAdjusted = ((zSize - 1) / simp) + 1;

            int indexSize = (xSizeAdjusted -1) * (zSizeAdjusted -1) * 6;
            singleMeshIndicies = new int[indexSize];  



            perlin = new SharpNoise.Modules.Perlin
            {
                Seed = seed,
                Frequency = freq,
                OctaveCount = oct,
                Lacunarity = 2.0,
                Persistence = 0.5,
            };
            noiseMap = new NoiseMap();
            noiseMapBuilder = new SharpNoise.Builders.PlaneNoiseMapBuilder
            {
                DestNoiseMap = noiseMap,
                SourceModule = perlin,
                EnableSeamless = false,
                
            };

            noiseMapBuilder.SetDestSize(xSize, zSize);
            noiseMapBuilder.SetBounds(xLocation, xLocation + xSize , zLocation - zSize, zLocation) ;
            noiseMapBuilder.Build();

            terrainMesh = generateMeshTerrain();
            
            terrainMesh.CalculateVertexNormals();
            terrainMesh.setupMesh();
        }

        private void addTriangle(int a, int b, int c)
        {
            singleMeshIndicies[indexCounter] = a;
            singleMeshIndicies[indexCounter + 1] = b;
            singleMeshIndicies[indexCounter + 2] = c;
            indexCounter += 3;
        }

        private float distanceFromSquare(Vector2 rectCenter, float width, float height, Vector2 pos)
        {
            float dx = Math.Max(Math.Abs(pos.X - rectCenter.X) - width / 2, 0);
            float dy = Math.Max(Math.Abs(pos.Y - rectCenter.Y) - height / 2, 0);
            return dx * dx + dy * dy;
        }

        public void Update(Vector2 playerPosition)
        {
            Vector2 rectCenter = new Vector2((xLocation + xSize) / 2, (zLocation - zSize) / 2);
            float viewerDstFromNearestEdge = distanceFromSquare(rectCenter, xSize, zSize, playerPosition);
            isVisible = viewerDstFromNearestEdge <= maxViewDistance;
        }

        public Mesh generateMeshTerrain()
        {

            float topLeftX = (xSize - 1) / -2f;
            float topLeftZ = (zSize - 1) / 2f;
            //float topLeftX = xLocation;
            //float topLeftZ = zLocation;

            Mesh meshData = new Mesh(xSizeAdjusted, zSizeAdjusted);

            meshData.Materials = new Material();
            meshData.Materials.Ambient = new Vector4(0.4f);
            meshData.Materials.Diffuse = new Vector4(0.1f);
            meshData.Materials.Specular = new Vector4(0.6f);
            meshData.Materials.Shininess = 10f;
            meshData.Materials.ShininessStrength = 1f;
            int vertexIndex = 0;
            meshData.transformRotation = new Quaternion(0, 0, 0, 1f);
            

            for (int y = 0; y < zSize; y+= simp)
            {
                for (int x = 0; x < xSize; x += simp)
                {
                    meshData.Vertices[vertexIndex] = new Vertex();
                    //meshData.vertices[vertexIndex].Position = new Vector3(topLeftX + x + xLocation, noiseMap[x, y] * heightMultiplier, topLeftZ - y - zLocation);
                    meshData.Vertices[vertexIndex].Position = new Vector3(topLeftX + x + xLocation, noiseMap[x, y], topLeftZ - y - zLocation);
                    meshData.Vertices[vertexIndex].TexCoord = new Vector2(x / (float)xSize, y / (float)zSize);


                    if (noiseMap[x, y] >= 1)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[7];

                    }
                    else if (noiseMap[x, y] >= 0.75)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[6];
                    }
                    else if (noiseMap[x, y] >=.375)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[5];
                    }
                    else if (noiseMap[x, y] >= .125)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[4];
                    }
                    else if (noiseMap[x, y] >= 0.0625)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[3];
                    }
                    else if (noiseMap[x, y] >= .0)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[2];
                    }
                    else if (noiseMap[x, y] >= -0.25)
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[1];
                    }
                    else 
                    {
                        meshData.Vertices[vertexIndex].Color = colorMap[0];
                    }
                    if (x < xSize-1 && y < zSize-1)
                    {
                        addTriangle(vertexIndex, vertexIndex + xSizeAdjusted + 1, vertexIndex + xSizeAdjusted);
                        addTriangle(vertexIndex + xSizeAdjusted + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            meshData.Indicies = new List<int>(singleMeshIndicies);


            return meshData;
        }

    }
}
