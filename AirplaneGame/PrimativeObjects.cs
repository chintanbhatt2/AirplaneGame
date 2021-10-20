using System;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class PrimativeObjects
    {
        public class Cube : Model
        {
            string CubePath = @"..\..\..\..\Blender Objects\Cube.dae";
            public Cube(string path) : base(path)
            {
                loadModel(path);
                this.rotateModel(0, 0, 0);
            }
        }

        public class Cone : Model
        {
            string ConePath = @"..\..\..\..\Blender Objects\Cone.dae";
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
            string SpherePath = @"..\..\..\..\Blender Objects\Sphere.dae";
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
