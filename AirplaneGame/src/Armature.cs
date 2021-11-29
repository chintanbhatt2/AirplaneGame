using System.Collections.Generic;

namespace AirplaneGame
{
    public class Armature
    {
        public Dictionary<string, Mesh> MeshDictionary = new Dictionary<string, Mesh>();
        
        public Armature(List<Mesh> Dependencies, Dictionary<string, Mesh> meshDict)
        {
            MeshDictionary = meshDict;
        }
    }
}
