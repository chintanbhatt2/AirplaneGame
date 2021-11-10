using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaneGame
{
    public class Armature
    {
        public Dictionary<string, Structures.Mesh> MeshDictionary = new Dictionary<string, Structures.Mesh>();
        
        public Armature(List<Structures.Mesh> Dependencies, Dictionary<string, Structures.Mesh> meshDict)
        {
            MeshDictionary = meshDict;
        }
    }
}
