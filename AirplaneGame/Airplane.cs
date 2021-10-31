namespace AirplaneGame
{
    public class Airplane : Model
    {

        private const float Gravity = 9.81f;

        Controls controlLock;
        Armature armature = new Armature();
        public Airplane(string path) : base(path)
        {
        }


    }
}
