using SharpNoise;
using OpenTK.Mathematics;


namespace AirplaneGame
{

    public class TerrainChunk
    {

        public static int xSize = 50;
        public static int zSize = 50;
        public static int heightMultiplier = 30;

        private int chunkRadius = 5;
        private SharpNoise.Modules.Perlin perlin;
        private SharpNoise.NoiseMap noiseMap;
        private SharpNoise.Builders.PlaneNoiseMapBuilder noiseMapBuilder;
        private Structures.Vertex[] vertexList;
        private int[] singleMeshIndicies;
        public Structures.Mesh terrainMesh;
        private int xLocation = 0;
        private int zLocation = 0;

        public TerrainChunk(int seed, int oct, double freq, int xL, int zL)
        {
            xLocation = (int)(xL / xSize) * xSize;
            zLocation = (int)(zL / zSize) * zSize;
            int indexSize = ((xSize - 1) * (zSize - 1)) * 6;
            singleMeshIndicies = new int[indexSize];
            vertexList = new Structures.Vertex[xSize * zSize];
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
            setIndicies();
            setVerticies();
            createMesh();
        }

        private void setIndicies()
        {
            int indexCounter = 0;

            indexCounter = 0;
            for (int y = 0; y < xSize; y++)
            {
                for (int x = 0; x < zSize; x++)
                {
                    if (x < zSize-1 && y < xSize-1)
                    {
                        singleMeshIndicies[indexCounter] = x;
                        singleMeshIndicies[indexCounter + 1] = x + 1;
                        singleMeshIndicies[indexCounter + 2] = x + (y * xSize);
                        singleMeshIndicies[indexCounter + 3] = x + 1;
                        singleMeshIndicies[indexCounter + 4] = x + (y * xSize);
                        singleMeshIndicies[indexCounter + 5] = x + (y * xSize) + 1;
                        indexCounter+=6;
                    }
                }
            }
        }

        private void setVerticies()
        {
            for (int i = 0; i < noiseMap.Data.Length; i++)
            {
                vertexList[i] = new Structures.Vertex();

                if (noiseMap.Data[i] <= 0.1)
                {
                    vertexList[i].Color = new Vector4((float)(176 / 255), (float)(224 / 255), (float)(230 / 255), 1f);
                }
                else if (noiseMap.Data[i] <= 0.5)
                {
                    vertexList[i].Color = new Vector4((float)(127 / 255), (float)(255 / 255), (float)(0 / 255), 1f);
                }
                else if (noiseMap.Data[i] <= 0.7)
                {
                    vertexList[i].Color = new Vector4((float)(128 / 255), (float)(128 / 255), (float)(128 / 255), 1f);
                }
                else
                {
                    vertexList[i].Color = new Vector4((float)(1), (float)(1), (float)(1), 1f);
                }


                vertexList[i].Position.Y = noiseMap.Data[i] * heightMultiplier;

            }

            for (int j = 0; j < xSize; j++)
            {
                for (int i = 0; i < zSize; i++)
                {
                    int index = (j * xSize) + i;
                    vertexList[index].Position.X = i + xLocation;
                    vertexList[index].Position.Z = j + zLocation;
                }
            }

        }

        private void createMesh()
        {
            terrainMesh = new Structures.Mesh(vertexList, singleMeshIndicies);
        }

    }
}
