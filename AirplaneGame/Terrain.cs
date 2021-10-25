
namespace AirplaneGame
{
    class Terrain : Model
    {

        public int xSize = 20;
        public int zSize = 20;
        public Terrain(string path) : base(path)
        {

        }

        protected void GenerateNoiseMap(int seed)
        {
            
        }

        public Structures.Mesh GetLocationMesh(float x, float y)
        {
            return null;
        }

    }
}
