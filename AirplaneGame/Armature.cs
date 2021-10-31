using System.Collections.Generic;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public struct Controls
    {
        public Vector3 Rotation;
        public Vector3 RotationLimit;
        public Vector3 Position;
        public Vector3 PositionLimit;
        public Vector3 Scale;
        public Vector3 ScaleLimit;

    }

    public struct MeshControls
    {
        private Structures.Mesh Mesh;
        private Controls Control;

        public MeshControls(Structures.Mesh m)
        {
            Control = new Controls();
            Mesh = m;
        }
        public void SetControl(Controls control)
        {
            Control = control;
        }
    }
    public class Armature
    {
        protected Dictionary<string, MeshControls> controlDict = new Dictionary<string, MeshControls>();
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        private Matrix4 Transform = Matrix4.Zero;
        public Armature()
        {

        }

        public Armature(List<Structures.Mesh> Dependencies, Dictionary<string, Structures.Mesh> meshDict)
        {
            foreach(KeyValuePair<string, Structures.Mesh> entry in meshDict)
            {
                controlDict.Add(entry.Key, new MeshControls(entry.Value));
            }
        }
        public void setControl(string name, Controls c)
        {
            controlDict[name].SetControl(c);
        }



        

    }
}
