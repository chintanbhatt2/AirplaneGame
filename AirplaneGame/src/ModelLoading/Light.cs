using System;
using Assimp;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    public class Light
    {
        double AngleInnerCone, AngleOuterCone, AttenuationConstant, AttenuationLinear, AttentionQuadratic;
        Vector3 ColorAmbient, ColorDiffuse, ColorSpecular;
        Vector3 Direction, Position;
        Matrix4 Transform;
        string Name;
        LightSourceType Type;
        PrimativeObjects.Cone sphere = new PrimativeObjects.Cone(@"..\..\..\..\Blender Objects\Cone.dae");

        

        public Light(Assimp.Light light)
        {
            Type = light.LightType;
            AngleInnerCone = light.AngleInnerCone;
            AngleOuterCone = light.AngleOuterCone;
            AttenuationConstant = light.AttenuationConstant;
            AttenuationLinear = light.AttenuationLinear;
            AttentionQuadratic = light.AttenuationQuadratic;
            ColorAmbient = new Vector3(light.ColorAmbient.R / 100, light.ColorAmbient.G / 100, light.ColorAmbient.B / 100);
            ColorDiffuse = new Vector3(light.ColorDiffuse.R / 100, light.ColorDiffuse.G / 100, light.ColorDiffuse.B / 100);
            ColorSpecular = new Vector3(light.ColorSpecular.R / 100, light.ColorSpecular.G / 100, light.ColorSpecular.B / 100);
            Direction = new Vector3(light.Direction.X, light.Direction.Y, light.Direction.Z);
            Name = light.Name;
            Position = new Vector3(light.Position.X, light.Position.Y, light.Position.Z);

        }


        public Light(string path)
        {
            var context = new Assimp.AssimpContext();
            Assimp.Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            Assimp.Light light = aiScene.Lights[0];

            Type = light.LightType;
            AngleInnerCone = light.AngleInnerCone;
            AngleOuterCone = light.AngleOuterCone;
            AttenuationConstant = light.AttenuationConstant;
            AttenuationLinear = light.AttenuationLinear;
            AttentionQuadratic = light.AttenuationQuadratic;
            ColorAmbient = new Vector3(light.ColorAmbient.R, light.ColorAmbient.G, light.ColorAmbient.B);
            ColorDiffuse = new Vector3(light.ColorDiffuse.R, light.ColorDiffuse.G, light.ColorDiffuse.B);
            ColorSpecular = new Vector3(light.ColorSpecular.R, light.ColorSpecular.G, light.ColorSpecular.B);
            Direction = new Vector3(light.Direction.X, light.Direction.Y, light.Direction.Z);
            //Position = new Vector3(light.Position.X, light.Position.Y, light.Position.Z);
            Position = new Vector3(30, 15, 5);
            Name = light.Name;

            findLightNode(aiScene.RootNode);
            Position = new Vector3(Transform.Column3);
            Transform.ExtractRotation().ToEulerAngles(out Direction);
            sphere.SetPosition(Position);
        }

        public void movePosition(Vector3 v, Shader shader)
        {
            Position += v;
            shader.SetVector3("light.Position", Position);
        }

        void findLightNode(Assimp.Node node)
        {
            if (node.Name == this.Name)
            {
                Transform = ASSIMPHelper.convertASSIMPtoOpenGLMat(node.Transform);
                return;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                findLightNode(node.Children[i]);
            }

        }

        [Flags]
        public enum UniformFlags
        {
            None = 0,
            Ambient = 1,
            Diffuse = 2,
            Specular = 4,
            Position = 8,
            Direction = 16,
            CutOff = 32,
            OuterCutOff = 64,
            Constant = 128,
            Linear = 256,
            Quadratic = 512,
            Intensity = 1024,
        }
        public void SetLightUniforms(Shader shader, UniformFlags f)
        {
            sphere.SetPosition(Position);
            if (f.HasFlag(UniformFlags.Ambient)) shader.SetVector3("light.Ambient", new Vector3(0.5f));
            if (f.HasFlag(UniformFlags.Diffuse)) shader.SetVector3("light.Diffuse", ColorDiffuse);
            if (f.HasFlag(UniformFlags.Specular)) shader.SetVector3("light.Specular", ColorSpecular);
            //shader.SetVector3("light.Diffuse", new Vector3(1.0f));
            //shader.SetVector3("light.Specular", new Vector3(1.0f));

            if (f.HasFlag(UniformFlags.Position)) shader.SetVector4("light.Position", new Vector4(Position, 1f));
            if (f.HasFlag(UniformFlags.Direction)) shader.SetVector4("light.Position", new Vector4(Direction, 0f));

            if (f.HasFlag(UniformFlags.CutOff)) shader.SetFloat("light.CutOff", (float)AngleInnerCone);
            if (f.HasFlag(UniformFlags.OuterCutOff)) shader.SetFloat("light.OuterCutOff", (float)AngleOuterCone);

            if (f.HasFlag(UniformFlags.Constant)) shader.SetFloat("light.Constant", (float)AttenuationConstant);
            if (f.HasFlag(UniformFlags.Linear)) shader.SetFloat("light.Linear", (float)AttenuationLinear);
            if (f.HasFlag(UniformFlags.Intensity)) shader.SetVector3("light.Intensity", new Vector3(0.9f));
            if (f.HasFlag(UniformFlags.Quadratic)) shader.SetFloat("light.Quadratic", (float)AttentionQuadratic);

            
        }
        public void DrawLight(Shader shader)
        {
            //sphere.Draw(shader);
        }

        public void SetColor(float r, float g, float b)
        {
            ColorDiffuse = new Vector3(r, g, b);
            ColorSpecular = new Vector3(r, g, b);
            ColorAmbient = new Vector3(r, g, b);
        }
    }
}
