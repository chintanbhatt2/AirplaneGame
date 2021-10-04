using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace AirplaneGame
{

    public class Window : GameWindow
    {
        float[] _vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};


        private readonly uint[] _indices_old =
        {
            0, 1, 3,
            1, 2, 3
        };

        private Shader _shader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;

        public List<Model> _stls = new List<Model>();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();


            _stls.Add(new Model(@"..\..\..\..\Blender Objects\Airplane.dae"));

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _shader = new Shader(@"..\..\..\..\shaders\vertex_shader.glsl", @"..\..\..\..\shaders\fragment_shader.glsl");



            _camera = new Camera(new Vector3(0.054436013f, 12.051596f, -26.652008f), Size.X / (float)Size.Y);
            _camera.Pitch = -13.799696f;
            _camera.Yaw = -270.1763f;

            CursorGrabbed = true;
        }
            
        private float scaleFactor = 0.1f;
        protected override void OnRenderFrame(FrameEventArgs e)
        {


            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_shader.Use();

            


            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());


            foreach (Model x in _stls)
            {
                x.Draw(_shader);
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //System.Console.WriteLine("Camera Position {0} \t Camera Angle {1}, {2} +++++++++ Rotation Vector {3}", _camera.Position, _camera.Pitch, _camera.Yaw, _stls[0].rotationVector);
            _stls[0].rotateMesh(0.1f, 0, 0, "Airo1_-_Propeller-2");


            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            float cameraSpeed = 15f;
            const float sensitivity = 0.2f;
            if (input.IsKeyDown(Keys.LeftControl))
            {
                if (input.IsKeyDown(Keys.W))
                {
                    _stls[0].rotateModel(-0.001f, 0, 0);
                }

                if (input.IsKeyDown(Keys.S))
                {
                    _stls[0].rotateModel(0.001f, 0, 0);
                }
                if (input.IsKeyDown(Keys.A))
                {
                    _stls[0].rotateModel(0, 0.001f, 0);
                }
                if (input.IsKeyDown(Keys.D))
                {
                    _stls[0].rotateModel(0, -0.001f, 0);
                }

                if (input.IsKeyDown(Keys.Q))
                {
                    _stls[0].rotateModel(0, 0, 0.001f);
                }
                if (input.IsKeyDown(Keys.E))
                {
                    _stls[0].rotateModel(0, 0, -0.001f);
                }
            }
            else
            {
                if (input.IsKeyDown(Keys.W))
                {
                    _camera.Position += _camera.Front * cameraSpeed * (float)e.Time;
                }

                if (input.IsKeyDown(Keys.S))
                {
                    _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.A))
                {
                    _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.D))
                {
                    _camera.Position += _camera.Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.Space))

                {
                    _camera.Position += _camera.Up * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time;
                }
            }
 

            if (input.IsKeyPressed(Keys.F1))
            {
                scaleFactor -= 0.1f;
            }
            if (input.IsKeyPressed(Keys.F2))
            {
                scaleFactor += 0.1f;
            }

            
            var mouse = MouseState;

            if (_firstMove) 
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }


        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
