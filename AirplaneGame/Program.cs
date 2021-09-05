using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;


namespace AirplaneGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const int FPS = 30;

            //Console.WriteLine("Hello World!");
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;
            gws.RenderFrequency = FPS;
            gws.UpdateFrequency = FPS;

            nws.Title = "FlyAround";
            nws.Size = (800, 480);

            //string stlLocation = "C:\\Users\\cb\\source\\repos\\chintanbhatt2\\AirplaneGame\\Chintan_STL\\Airo1 - Elev1-1 HorizontalStab1Move-1.stl";
            //List<float> vertsList = readSTL(stlLocation);
            //uint triangleCount = (uint)vertsList[0];
            //vertsList.RemoveAt(0);
            //float[] verts = vertsList.ToArray();

            //CREATES TRIANGLE 
            float[] verts = new float[] {
                -0.5f, -0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f
            };


            GameWindow window = new GameWindow(gws, nws);

            ShaderProgram shaderProgram = new ShaderProgram() { id = 0 };
            window.Load += () => {
                shaderProgram = LoadShaderProgram("..\\..\\..\\shaders\\vertex_shader.glsl", "..\\..\\..\\shaders\\fragment_shader.glsl");
            };


            window.RenderFrame += (FrameEventArgs args) =>
            {
                GL.UseProgram(shaderProgram.id);

                GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                double timeValue = args.Time;
                double greenTime = (Math.Sin(timeValue) / 2.0f) + 0.5f;

                int vertexColorLocation = GL.GetUniformLocation(shaderProgram.id, "shifting_color");
                GL.UseProgram(shaderProgram.id);
                GL.Uniform4(vertexColorLocation, 0.0f, greenTime, 0.0f, 1.0f);
                //// Creates triangle
                //float[] verts = { -0.5f, -0.5f, 0.0f, 0.5f, -0.5f, 0.0f, 0.0f, 0.5f, 0.0f };
                //float[] verts = vertsList.ToArray();
                float[] color = { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f };

                //int vao = GL.GenVertexArray();
                //int verticies = GL.GenBuffer();
                //GL.BindVertexArray(vao);
                //GL.BindBuffer(BufferTarget.ArrayBuffer, verticies);
                //GL.BufferData(BufferTarget.ArrayBuffer, 36, verts, BufferUsageHint.StaticDraw);
                //GL.EnableVertexAttribArray(0);
                //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                //GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 3);

                //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                //GL.BindVertexArray(0);
                //GL.DeleteVertexArray(vao);
                //GL.DeleteBuffer(verticies);

                //creating buffers
                int vao = GL.GenVertexArray();
                int verticies = GL.GenBuffer();
                int colors = GL.GenBuffer();

                //binding buffers
                GL.BindVertexArray(vao);

                GL.BindBuffer(BufferTarget.ArrayBuffer, verticies);
                GL.BufferStorage(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferStorageFlags.None);
                GL.EnableVertexAttribArray(0);
                //GL.VertexAttribPointer(0, (int)triangleCount*3, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, 0, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, colors);
                GL.BufferStorage(BufferTarget.ArrayBuffer, color.Length * sizeof(float), color, BufferStorageFlags.None);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);


                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.DeleteVertexArray(vao);
                GL.DeleteBuffer(verticies);
                GL.DeleteBuffer(colors);

                window.SwapBuffers();
            };

            window.Run();
        }


        private static Shader LoadShader( string shaderLocation, ShaderType type)
        {
            int shaderID  = GL.CreateShader(type);
            GL.ShaderSource(shaderID, System.IO.File.ReadAllText(shaderLocation));
            GL.CompileShader(shaderID);
            string infoLog = GL.GetShaderInfoLog(shaderID);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception(infoLog);
            }

            return new Shader() { id = shaderID };
        }   

        private static ShaderProgram LoadShaderProgram (string vertexShaderLocation, string fragmentShaderLocation)
        {
            int shaderProgramID = GL.CreateProgram();

            Shader vertexShader = LoadShader(vertexShaderLocation, ShaderType.VertexShader);
            Shader fragmentShader = LoadShader(fragmentShaderLocation, ShaderType.FragmentShader);

            GL.AttachShader(shaderProgramID, vertexShader.id);
            GL.AttachShader(shaderProgramID, fragmentShader.id);
            GL.LinkProgram(shaderProgramID);
            GL.DetachShader(shaderProgramID, vertexShader.id);
            GL.AttachShader(shaderProgramID, fragmentShader.id);
            GL.DeleteShader(vertexShader.id);
            GL.DeleteShader(fragmentShader.id);

            string infoLog = GL.GetProgramInfoLog(shaderProgramID);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception(infoLog);
            }

            return new ShaderProgram() { id = shaderProgramID };
        }

        public struct Shader
        {
            public int id;
        }

        public struct ShaderProgram
        {
            public int id;
        }
        public struct Vertex
        {
            Vector3 Position;
            Vector3 Normal;
            Vector3 TexCoords;
        };

        public static List<float> readSTL(string stlFile)
        {
            List<float> pointList = new List<float> { };
            BinaryReader reader = new BinaryReader(File.Open(stlFile, FileMode.Open));

            reader.ReadBytes(80);
            uint triangleCount = reader.ReadUInt32();
            pointList.Add((float)triangleCount);

            float nextFloat = 0;
            //while((nextFloat = reader.ReadSingle()) > 0)
            for (int j = 0; j < 1; j++)
            {
                reader.ReadBytes(8);
                for (int i = 0; i  < 3*3; i++)
                {
                    pointList.Add(reader.ReadSingle());
                }

                reader.ReadBytes(2);
            }
            reader.Close();
            return pointList;
        }

    }
}
