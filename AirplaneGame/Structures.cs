using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class Structures
    {
        public class Vertex
        {
            public Vector3 Coord;
            public Vector3 texCoord;
            public Vertex(float x, float y, float z)
            {
                this.Coord = new Vector3(x, y, z);
            }
            public Vertex(float x, float y, float z, float u, float v, float w)
            {
                this.Coord = new Vector3(x, y, z);
                this.texCoord = new Vector3(u, v, w);
            }

        }
        public class Triangle
        {
            public List<Vertex> Verticies = new List<Vertex>();
            public Vector3 NormalVector;

            public UInt16 AttributeCount;

            public void AddVertex(Vertex vert)
            {
                this.Verticies.Add(vert);
            }
            public Triangle(Vertex x, Vertex y, Vertex z)
            {
                this.Verticies.Append(x);
                this.Verticies.Append(y);
                this.Verticies.Append(z);
            }
            public Triangle()
            {

            }
        }

        public class Mesh
        {
            public List<Triangle> Triangles = new List<Triangle>();
            public uint triangleCount;
        }
    }
}
