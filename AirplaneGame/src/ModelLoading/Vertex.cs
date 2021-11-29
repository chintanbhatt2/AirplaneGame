using System.Collections.Generic;
using OpenTK.Mathematics;
namespace AirplaneGame
{
    public class Vertex
    {
        public Vector3 Position = new Vector3(0);
        public Vector3 Normal = new Vector3(0);
        public Vector2 TexCoord = new Vector2(0);
        public Vector3 Tangent = new Vector3(0);
        public Vector3 Bitangent = new Vector3(0);
        public Vector4 Color = new Vector4(0, 0, 0, 1);

        public Vertex()
        {
            Position = new Vector3(0);
            Normal = new Vector3(0);
            Color = new Vector4(0, 0, 0, 1);
        }
    }

}
