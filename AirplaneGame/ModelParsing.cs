using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class ModelParsing
    {
        public class STL
        {   
            public string name;
            public Structures.Mesh mesh = new Structures.Mesh();
            public STL(string location)
            {
                if (location == null) return;
                BinaryReader binaryReader = new BinaryReader(File.OpenRead(location));
                this.name = new string(binaryReader.ReadChars(80));
                this.name.Trim();
                this.mesh.triangleCount = binaryReader.ReadUInt32();
                for (int i = 0; i < this.mesh.triangleCount; i++)
                {
                    Structures.Triangle tri = new Structures.Triangle();
                    Vector3 floatList;
                    for (int j = 0; j < 3; j++)
                    {
                        floatList[j] = binaryReader.ReadSingle();
                    }
                    tri.NormalVector = new Vector3(floatList[0], floatList[1], floatList[2]);

                    for (int j = 0; j < 3; j++)
                    {
                        for(int k = 0; k < 3; k++)
                        {
                            floatList[k] = binaryReader.ReadSingle();
                        }
                        Structures.Vertex vert = new Structures.Vertex();
                        vert.Position = floatList;
                        tri.AddVertex(vert);
                    }

                    tri.AttributeCount = binaryReader.ReadUInt16();
                    this.mesh.Triangles.Add(tri);
                }
            }

            public float[] getVertexArray()
            {
                float[] arrayList = new float[this.mesh.triangleCount * 9];
                

                for (int i = 0; i < this.mesh.triangleCount; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            arrayList[i*9 + j*3 + k] = this.mesh.Triangles.ElementAt(i).Verticies.ElementAt(j).Coord[k];

                        }

                    }
                }


                return arrayList;
            }
        }
    }
}
