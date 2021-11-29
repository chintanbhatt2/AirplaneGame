using System;
using OpenTK.Mathematics;

namespace AirplaneGame
{   
    public class Material
    {

        public string Name;

        public Vector4 Diffuse;
        public Vector4 Specular;
        public Vector4 Ambient;
        public Vector4 Emissive;
        public Vector4 Transparent;
        public float Shininess;
        public float ShininessStrength;

        [Flags]
        public enum UniformFlags
        {
            None = 0,
            Ambient = 1,
            Diffuse = 2,
            Specular = 4,
            Emissive = 8,
            Transparent = 16,
            Shininess = 32,
            ShininessStrength = 64,
        }


        public Material(Assimp.Material mat)
        {
            Ambient = ASSIMPHelper.convertAssimpToOpenGLVec4(mat.ColorAmbient);
            Specular = ASSIMPHelper.convertAssimpToOpenGLVec4(mat.ColorSpecular);
            Diffuse = ASSIMPHelper.convertAssimpToOpenGLVec4(mat.ColorDiffuse);
            Emissive = ASSIMPHelper.convertAssimpToOpenGLVec4(mat.ColorEmissive);
            Transparent = ASSIMPHelper.convertAssimpToOpenGLVec4(mat.ColorTransparent);
            Shininess = mat.Shininess;
            ShininessStrength = mat.ShininessStrength;
            Name = mat.Name;
        }

        public Material()
        {
            Diffuse = new Vector4(0.687f, 0.791f, 0.799f, 1.0f);
            Specular = new Vector4(0.687f, 0.791f, 0.799f, 1.0f);
            Ambient = new Vector4(0.687f, 0.791f, 0.799f, 1.0f);
            Emissive = new Vector4(0.687f, 0.791f, 0.799f, 1.0f);
            Transparent = new Vector4(0.687f, 0.791f, 0.799f, 1.0f);
            Shininess = 0.2f;
            ShininessStrength = 0.2f;
        }
    }
}
