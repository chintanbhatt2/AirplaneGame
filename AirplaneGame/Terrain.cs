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
                    float xPosition =  ((int)(pos.X / TerrainChunk.xSize) * TerrainChunk.xSize) + ((float)i * TerrainChunk.xSize);
                    float zPosition = ((int)(pos.Z / TerrainChunk.zSize) * TerrainChunk.zSize) + ((float)j * TerrainChunk.zSize);
                    Ter[(j + 1) * 3 + (i + 1)] = new TerrainChunk(seed, oct, freq, xPosition, zPosition, System.Math.Abs(i * 3 + j * 3));
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

        public static int xSize = 241;
        public static int zSize = 241;
        public static int heightMultiplier = 30;

        public int levelOfDetail;

        private SharpNoise.Modules.Perlin perlin;
        private SharpNoise.NoiseMap noiseMap;
        private SharpNoise.Builders.PlaneNoiseMapBuilder noiseMapBuilder;
        private int[] singleMeshIndicies;
        public Structures.Mesh terrainMesh;
        private int xLocation = 0;
        private int zLocation = 0;
        private int indexCounter = 0;
        private int simp = 0;
        private int xSizeAdjusted;
        private int zSizeAdjusted;

        public static Vector4[] colorMap = {
            new Vector4(  0,   0, 128, 255).Normalized(), // deeps
            new Vector4(  0,   0, 255, 255).Normalized(), // shallow
            new Vector4(  0, 128, 255, 255).Normalized(), // shore
            new Vector4(240, 240,  64, 255).Normalized(), // sand
            new Vector4( 32, 160,   0, 255).Normalized(), // grass
            new Vector4(224, 224,   0, 255).Normalized(), // dirt
            new Vector4(128, 128, 128, 255).Normalized(), // rock
            new Vector4(255, 255, 255, 255).Normalized(), // snow
        };

        public TerrainChunk(int seed, int oct, double freq, float xL, float zL, int lod)
        {
            levelOfDetail = lod;
            xLocation = (int)(xL / xSize) * xSize;
            zLocation = (int)(zL / zSize) * zSize;
            simp = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            xSizeAdjusted = (xSize - 1) / simp + 1;
            zSizeAdjusted = (zSize - 1) / simp + 1;

            int indexSize = (xSizeAdjusted) * (zSizeAdjusted) * 6;
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



        public Structures.Mesh generateMeshTerrain()
        {

            float topLeftX = (xSize - 1) / -2f;
            float topLeftZ = (zSize - 1) / 2f;

            Structures.Mesh meshData = new Structures.Mesh(xSizeAdjusted, zSizeAdjusted);
            int vertexIndex = 0;

            

            for (int y = 0; y < zSize ; y+= simp)
            {
                for (int x = 0; x < xSize; x += simp)
                {
                    meshData.vertices[vertexIndex] = new Structures.Vertex();
                    //meshData.vertices[vertexIndex].Position = new Vector3(topLeftX + x + xLocation, noiseMap[x, y] * heightMultiplier, topLeftZ - y - zLocation);
                    meshData.vertices[vertexIndex].Position = new Vector3(topLeftX + x + xLocation, noiseMap[x, y], topLeftZ - y - zLocation);
                    meshData.vertices[vertexIndex].TexCoord = new Vector2(x / (float)xSize, y / (float)zSize);


                    if (noiseMap[x, y] >= 1)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[7];

                    }
                    else if (noiseMap[x, y] >= 0.75)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[6];
                    }
                    else if (noiseMap[x, y] >=.375)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[5];
                    }
                    else if (noiseMap[x, y] >= .125)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[4];
                    }
                    else if (noiseMap[x, y] >= 0.0625)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[3];
                    }
                    else if (noiseMap[x, y] >= .0)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[2];
                    }
                    else if (noiseMap[x, y] >= -0.25)
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[1];
                    }
                    else 
                    {
                        meshData.vertices[vertexIndex].Color = colorMap[0];
                    }
                    if (x < xSize-1 && y < zSize-1)
                    {
                        addTriangle(vertexIndex, vertexIndex + xSizeAdjusted + 1, vertexIndex + xSizeAdjusted);
                        addTriangle(vertexIndex + xSizeAdjusted + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            meshData.indicies = singleMeshIndicies;

            System.Console.WriteLine(vertexIndex);

            return meshData;
        }

    }
}
