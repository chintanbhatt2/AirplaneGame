using Assimp;
using OpenTK.Mathematics;

namespace AirplaneGame
{
    class Light
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
            ColorAmbient = new Vector3(light.ColorAmbient.R/100, light.ColorAmbient.G / 100, light.ColorAmbient.B / 100);
            ColorDiffuse = new Vector3(light.ColorDiffuse.R / 100, light.ColorDiffuse.G / 100, light.ColorDiffuse.B / 100);
            ColorSpecular = new Vector3(light.ColorSpecular.R / 100, light.ColorSpecular.G / 100, light.ColorSpecular.B / 100);
            Direction = new Vector3(light.Direction.X, light.Direction.Y, light.Direction.Z);
            Name = light.Name;
            Position = new Vector3(light.Position.X, light.Position.Y, light.Position.Z);
            sphere.SetPosition(Position);

        }


        public Light(string path)
        {
            var context = new Assimp.AssimpContext();
            Scene aiScene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

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
            sphere.SetPosition(Position);

            //findLightNode(aiScene);
        }

        //void findLightNode(Assimp.Scene s, Assimp.Node node)
        //{
        //    if (node.Name == this.Name)
        //    {
        //        Transform = node.Transform;
        //    }
        //}
        public void SetLightUniforms(Shader shader)
        {
            sphere.SetPosition(Position);
            shader.SetVector3("light.Ambient", new Vector3(0.5f));
            shader.SetVector3("light.Diffuse", ColorDiffuse);
            shader.SetVector3("light.Specular", ColorSpecular);
            //shader.SetVector3("light.Diffuse", new Vector3(1.0f));
            //shader.SetVector3("light.Specular", new Vector3(1.0f));

            shader.SetVector3("light.Position", new Vector3(30f, 30f, 0f));
            shader.SetVector3("light.Direction", Direction);

            shader.SetFloat("light.CutOff", (float)AngleInnerCone);
            shader.SetFloat("light.OuterCutOff", (float)AngleOuterCone);

            shader.SetFloat("light.Constant", (float)AttenuationConstant);
            shader.SetFloat("light.Linear", (float)AttenuationLinear);
            shader.SetFloat("light.Quadratic", (float)AttentionQuadratic);

            
        }
        public void DrawLight(Shader shader)
        {
            sphere.Draw(shader);
        }

        public void SetColor(float r, float g, float b)
        {
            ColorDiffuse = new Vector3(r, g, b);
            ColorSpecular = new Vector3(r, g, b);
            ColorAmbient = new Vector3(r, g, b);
        }
    }
}
