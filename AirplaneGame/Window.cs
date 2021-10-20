using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

        private Shader ObjectShader, LightingShader;

        private Camera Cam;

        private Light _lights;

        private bool FirstMove = true;

        private Vector2 LastPos;

        private double _time;

        public List<Model> _stls = new List<Model>();

        public Airplane plane;
        public Shader SkyboxShader;


        Skybox skybox;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();


            //_stls.Add(new Model(@"..\..\..\..\Blender Objects\Airplane_Lighting.dae"));

            plane = new Airplane(@"..\..\..\..\Blender Objects\Airplane_Lighting.dae");

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            //ObjectShader = new Shader(@"..\..\..\..\shaders\vertex_shader.glsl", @"..\..\..\..\shaders\fragment_shader.glsl");
            ObjectShader = new Shader(@"..\..\..\..\shaders\vertex_shader.glsl", @"..\..\..\..\shaders\lighting_shader.glsl");
            LightingShader = new Shader(@"..\..\..\..\shaders\vertex_shader.glsl", @"..\..\..\..\shaders\lighting_shader.glsl");
            SkyboxShader = new Shader(@"..\..\..\..\shaders\skybox_vertex.glsl", @"..\..\..\..\shaders\skybox_fragment.glsl");
            _lights = new Light(@"..\..\..\..\Blender Objects\Airplane_Lighting.dae");
       

            Cam = new Camera(new Vector3(0.054436013f, 12.051596f, -26.652008f), Size.X / (float)Size.Y);
            Cam.Pitch = -13.799696f;
            Cam.Yaw = -270.1763f;
            plane.lockMeshRotation(true, true, false, "Airo1_-_Propeller-2");
            plane.lockMeshRotation(false, true, true, "Airo1_-_Elev1-2_HorizontalStab1stat-1");
            skybox = new Skybox(Directory.GetFiles(@"..\..\..\..\resources\skybox\daylight"));
            CursorGrabbed = true;
        }
            
        private float scaleFactor = 0.1f;
        protected override void OnRenderFrame(FrameEventArgs e)
        {


            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;
            GL.ClearColor(.5f, .50f, .50f, .50f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);



            ObjectShader.SetMatrix4("view", Cam.GetViewMatrix());
            ObjectShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
            _lights.SetLightUniforms(ObjectShader);

            ObjectShader.Use();

            GL.Disable(EnableCap.DepthTest);
            ObjectShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
            SkyboxShader.SetMatrix4("view", Cam.GetViewMatrix().ClearTranslation());
            skybox.Draw(SkyboxShader);
            GL.Enable(EnableCap.DepthTest);

            _lights.DrawLight(ObjectShader);
            //plane.Draw(ObjectShader);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //System.Console.WriteLine("Camera Position {0} \t Camera Angle {1}, {2} +++++++++ Rotation Vector {3}", _camera.Position, _camera.Pitch, _camera.Yaw, plane.rotationVector);
            plane.rotateMesh(0.0f, 0.0f, 0.001f, "Airo1_-_Propeller-2");

            Matrix4 modelRotation = plane.getModelTransform();
            Quaternion rotationQuat = plane.getRotationVector();
            Vector3 euAngles = rotationQuat.ToEulerAngles();
            if (euAngles.X > MathHelper.DegreesToRadians(45))
            {
                plane.setMeshAngle(euAngles, "Airo1_-_Elev1-2_HorizontalStab1stat-1");

            }


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
                    plane.rotateModel(-0.001f, 0, 0);
                }

                if (input.IsKeyDown(Keys.S))
                {
                    plane.rotateModel(0.001f, 0, 0);
                }
                if (input.IsKeyDown(Keys.A))
                {
                    plane.rotateModel(0, 0.001f, 0);
                }
                if (input.IsKeyDown(Keys.D))
                {
                    plane.rotateModel(0, -0.001f, 0);
                }

                if (input.IsKeyDown(Keys.Q))
                {
                    plane.rotateModel(0, 0, 0.001f);
                }
                if (input.IsKeyDown(Keys.E))
                {
                    plane.rotateModel(0, 0, -0.001f);
                }
            }
            else
            {
                if (input.IsKeyDown(Keys.W))
                {
                    Cam.Position += Cam.Front * cameraSpeed * (float)e.Time;
                }

                if (input.IsKeyDown(Keys.S))
                {
                    Cam.Position -= Cam.Front * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.A))
                {
                    Cam.Position -= Cam.Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.D))
                {
                    Cam.Position += Cam.Right * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.Space))

                {
                    Cam.Position += Cam.Up * cameraSpeed * (float)e.Time;
                }
                if (input.IsKeyDown(Keys.LeftShift))
                {
                    Cam.Position -= Cam.Up * cameraSpeed * (float)e.Time;
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

            if (FirstMove) 
            {
                LastPos = new Vector2(mouse.X, mouse.Y);
                FirstMove = false;
            }
            else
            {
                var deltaX = mouse.X - LastPos.X;
                var deltaY = mouse.Y - LastPos.Y;
                LastPos = new Vector2(mouse.X, mouse.Y);

                Cam.Yaw += deltaX * sensitivity;
                Cam.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }


        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Cam.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            Cam.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
