using System;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class PrimativeObjects
    {
        public class Cube : Model
        {
            public Cube(string path) : base(path)
            {
                loadModel(path);
            }

        }

        public class Cone : Model
        {
            public Cone(string path) : base(path)
            {
                loadModel(@"..\..\..\..\Blender Objects\Cone.dae");
            }
            public void SetPosition(Vector3 vec)
            {
                meshes[0].localMatrix = meshes[0].localMatrix.ClearTranslation();
                meshes[0].localMatrix = meshes[0].localMatrix * Matrix4.CreateTranslation(vec);
            }
        }
        public class Sphere : Model
        {
             public Sphere(string path) : base(path)
            {
                loadModel(@"..\..\..\..\Blender Objects\Sphere.dae");
            }

            public void SetPosition(Vector3 vec)
            {
                meshes[0].localMatrix = meshes[0].localMatrix.ClearTranslation();
                meshes[0].localMatrix = meshes[0].localMatrix * Matrix4.CreateTranslation(vec);
            }
        }
    }
}
