using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;



namespace AirplaneGame
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "LearnOpenTK - Coordinates Systems",
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
