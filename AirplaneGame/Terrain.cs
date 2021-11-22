using SharpNoise;
using OpenTK.Mathematics;


namespace AirplaneGame
{
    public class Terrain
    {
        public TerrainChunk[] Ter = new TerrainChunk[9];
        public int seed, oct; 
        public double freq;
        public Terrain(Vector3 pos, int s, int o, double f)
        {
            seed = s;
            oct = o;
            freq = f;

            for(int j = -1; j < 2; j++)
            {
                for(int i = -1; i < 2; i++)
                {
                    float xPosition = pos.X + ((float)i * TerrainChunk.xSize);
                    float zPosition = pos.Z + ((float)j * TerrainChunk.zSize);
                    Ter[(j + 1) * 3 + (i + 1)] = new TerrainChunk(seed, oct, freq, xPosition, zPosition);
                }
            }
        }

        public void Draw(Shader shader)
        {
            for(int i = 0; i < 9; i++)
            {
                Ter[i].terrainMesh.Draw(shader);
            }
        }
    }
    public class TerrainChunk
    {

        public static int xSize = 50;
        public static int zSize = 50;
        public static int heightMultiplier = 30;

        private SharpNoise.Modules.Perlin perlin;
        private SharpNoise.NoiseMap noiseMap;
        private SharpNoise.Builders.PlaneNoiseMapBuilder noiseMapBuilder;
        private int[] singleMeshIndicies;
        public Structures.Mesh terrainMesh;
        private int xLocation = 0;
        private int zLocation = 0;
        private int indexCounter = 0;

        public TerrainChunk(int seed, int oct, double freq, float xL, float zL)
        {
            xLocation = (int)(xL / xSize) * xSize;
            zLocation = (int)(zL / zSize) * zSize;
            int indexSize = ((xSize - 1) * (zSize - 1)) * 6;
            singleMeshIndicies = new int[indexSize];
            perlin = new SharpNoise.Modules.Perlin
            {
                Seed = seed,
                Frequency = freq,
                OctaveCount = oct,
            };
            noiseMap = new NoiseMap();
            noiseMapBuilder = new SharpNoise.Builders.PlaneNoiseMapBuilder
            {
                DestNoiseMap = noiseMap,
                SourceModule = perlin,
                EnableSeamless = true,
            };

            noiseMapBuilder.SetDestSize(xSize, zSize);
            noiseMapBuilder.SetBounds(-4, 4, -4, 4);
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



        public Structures.Mesh generateMeshTerrain()
        {
            float topLeftX = (xSize - 1) / -2f;
            float topLeftZ = (zSize - 1) / 2f;

            Structures.Mesh meshData = new Structures.Mesh(xSize, zSize);
            int vertexIndex = 0;

            for (int y = 0; y < zSize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    meshData.vertices[vertexIndex] = new Structures.Vertex();
                    meshData.vertices[vertexIndex].Position = new Vector3(topLeftX + x + xLocation, noiseMap[x,y] * heightMultiplier, topLeftZ - y - zLocation);
                    meshData.vertices[vertexIndex].TexCoord = new Vector2(x / (float)xSize, y / (float)zSize);
                    if (noiseMap[x, y] < 0.1)
                    {
                        meshData.vertices[vertexIndex].Color = new Vector4(0f, 0.61568f, 0.76862f, 1.0f);

                    }
                    else if (noiseMap[x, y] < 0.2)
                    {
                        meshData.vertices[vertexIndex].Color = new Vector4(0f, 0.61568f, 0.76862f, 1.0f);
                    }
                    else if (noiseMap[x, y] < 0.7)
                    {
                        meshData.vertices[vertexIndex].Color = new Vector4(0.48627f, 0.98823f, 0, 1.0f);
                    }
                    else
                    {
                        meshData.vertices[vertexIndex].Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    if (x < xSize -1 && y < zSize-1)
                    {
                        addTriangle(vertexIndex, vertexIndex + xSize + 1, vertexIndex + xSize);
                        addTriangle(vertexIndex + xSize + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            meshData.indicies = singleMeshIndicies;

            return meshData;
        }

    }
}
