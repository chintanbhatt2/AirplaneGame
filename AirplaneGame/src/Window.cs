using System.Collections.Generic;
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

        private Shader ObjectShader, LightingShader, TerrainShader;

        private Camera Cam;

        private Light _lights;

        private bool FirstMove = true;

        private Vector2 LastPos;

        private double _time;

        public List<Model> Models = new List<Model>();

        public Model plane;
        public Shader SkyboxShader;
        public Scene scene;
        public Terrain ter;
        float MoveSpeed = 0.0001f;
        const float rotateSpeed = 0.00001f;


        Skybox skybox;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();


            

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            

            ObjectShader = new Shader(@"..\..\..\..\shaders\vertex_shader.glsl", @"..\..\..\..\shaders\lighting_shader.glsl");
            SkyboxShader = new Shader(@"..\..\..\..\shaders\skybox_vertex.glsl", @"..\..\..\..\shaders\skybox_fragment.glsl");
            TerrainShader = new Shader(@"..\..\..\..\shaders\terrain_vertex_shader.glsl", @"..\..\..\..\shaders\terrain_fragment_shader.glsl");
            _lights = new Light(@"..\..\..\..\Blender Objects\Airplane_Lighting.dae");
            scene = new Scene(@"..\..\..\..\Blender Objects\Airplane_Lighting.dae");

            Cam = new Camera(new Vector3(0.054436013f, 12.051596f, -26.652008f), Size.X / (float)Size.Y);
            Cam.Pitch = -13.799696f;
            Cam.Yaw = -270.1763f;
            skybox = new Skybox(Directory.GetFiles(@"..\..\..\..\resources\skybox\daylight"));
            CursorGrabbed = true;

            SkyboxShader.SetInt("skybox", 0);
            
            ter = new Terrain(Cam.Position, 420, 7, 0.01);
            plane = scene.Models[0];
            plane.position = new Vector3(0f, 10f, 0f);
            plane.rotateModel(0f, 1.5708f, 0f);

        }
            
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            ter.Update(Cam.Position);

            base.OnRenderFrame(e);

            _time += 4.0 * e.Time;
            GL.ClearColor(.5f, .50f, .50f, .50f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            
            //ObjectShader.SetMatrix4("view", Cam.GetViewMatrix());
            ObjectShader.SetMatrix4("view", Matrix4.LookAt(Cam.Position, plane.position, Cam.Up));
            ObjectShader.SetMatrix4("projection", Cam.GetProjectionMatrix());


            _lights.SetLightUniforms(ObjectShader, Light.UniformFlags.Direction | Light.UniformFlags.Intensity);
            plane.Draw(ObjectShader);



            TerrainShader.SetMatrix4("view", Matrix4.LookAt(Cam.Position, plane.position, Cam.Up));
            TerrainShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
            ter.Draw(TerrainShader, Cam);
            _lights.SetLightUniforms(TerrainShader, Light.UniformFlags.Direction | Light.UniformFlags.Intensity);



            _lights.DrawLight(ObjectShader);
            GL.DepthFunc(DepthFunction.Lequal);
            SkyboxShader.Use();
            SkyboxShader.SetMatrix4("projection", Cam.GetProjectionMatrix());
            SkyboxShader.SetMatrix4("view", Matrix4.LookAt(Cam.Position, plane.position, Cam.Up).ClearTranslation());
            skybox.Draw(SkyboxShader);
            GL.DepthFunc(DepthFunction.Less);



            SwapBuffers();
        }

        public void movePlane()
        {
            Matrix4 movement = Matrix4.CreateTranslation(new Vector3(0, 0, MoveSpeed));
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(plane.rotationVector);
            movement = movement * rotationMatrix;
            Vector3 ext = movement.ExtractTranslation();
            plane.moveModel(ext.X, ext.Y, ext.Z);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            plane.rotateMesh(0.0f, 0.0f, 0.01f, "Airo1_-_Propeller-2");

            Matrix4 modelRotation = plane.getModelTransform();
            Quaternion rotationQuat = plane.getRotationVector();
            Vector3 euAngles = rotationQuat.ToEulerAngles();



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
            const float sensitivity = 0.02f;

            if (input.IsKeyDown(Keys.W))
            {
                plane.rotateModel(-rotateSpeed, 0, 0);
            }

            if (input.IsKeyDown(Keys.S))
            {
                plane.rotateModel(rotateSpeed, 0, 0);
            }
            if (input.IsKeyDown(Keys.A))
            {
                plane.rotateModel(0, rotateSpeed, 0);
            }
            if (input.IsKeyDown(Keys.D))
            {
                plane.rotateModel(0, -rotateSpeed, 0);
            }

            if (input.IsKeyDown(Keys.Q))
            {
                plane.rotateModel(0, 0, rotateSpeed);
            }
            if (input.IsKeyDown(Keys.E))
            {
                plane.rotateModel(0, 0, -rotateSpeed);
            }

            if (input.IsKeyDown(Keys.LeftControl))
            {
                MoveSpeed -= 0.00000001f;
                if (MoveSpeed < 0)
                {
                    MoveSpeed = 0.0001f;
                }
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                MoveSpeed += 0.00000001f;
            }

            movePlane();

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

                Matrix4 camRotationMat = Matrix4.CreateRotationY(Cam.Yaw);
                camRotationMat *= Matrix4.CreateRotationX(Cam.Pitch);
                Cam.Position = plane.position + (Matrix4.CreateTranslation(new Vector3(4f, 0, 0)) * camRotationMat).ExtractTranslation();



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
