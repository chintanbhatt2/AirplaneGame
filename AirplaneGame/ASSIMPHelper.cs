using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using Assimp;

namespace AirplaneGame
{
    public static class ASSIMPHelper
    {
        public static Matrix4 convertASSIMPtoOpenGLMat(Matrix4x4 assimp)
        {
            //  It's ugly but what can you do ¯\_(ツ)_/¯
            return new Matrix4(assimp.A1, assimp.A2, assimp.A3, assimp.A4,
                                assimp.B1, assimp.B2, assimp.B3, assimp.B4,
                                assimp.C1, assimp.C2, assimp.C3, assimp.C4,
                                assimp.D1, assimp.D2, assimp.D3, assimp.D4);
        }
    }
}
