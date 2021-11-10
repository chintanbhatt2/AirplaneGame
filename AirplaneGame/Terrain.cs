using SharpNoise;
using OpenTK.Mathematics;


namespace AirplaneGame
{

    public class TerrainChunk : Model
    {
        public Vector3[] vertexList;
        public int[] indexList;
        public Structures.Mesh mesh;
        public TerrainChunk(int xSize, int zSize, int xLocation, int zLocation, SharpNoise.NoiseMap noiseMap)
        {
            vertexList = new Vector3[xSize * zSize];
            for(int i = zLocation; i < zSize; i++)
            {

            }
            mesh = new Structures.Mesh();
        }
    }
    public class Terrain
    {

        public static int xSize = 20;
        public static int zSize = 20;

        private int chunkRadius = 5;
        private SharpNoise.Modules.Perlin perlin;
        private SharpNoise.NoiseMap noiseMap;
        private SharpNoise.Builders.PlaneNoiseMapBuilder noiseMapBuilder;
        private int[] singleMeshIndicies = new int[(xSize - 1 * zSize - 1) * 6];

        public Terrain(int seed, int oct, double freq  )
        {
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
        }

        public Structures.Mesh GetLocationMesh(float x, float y)
        {
            x = ((int)x);
            y = ((int)y);

            Structures.Mesh m = new Structures.Mesh();

            return m;
        }

        private void setIndicies()
        {

        }

    }
}
