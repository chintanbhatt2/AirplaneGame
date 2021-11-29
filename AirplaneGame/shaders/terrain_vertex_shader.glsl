#version 430 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec4 aVertexColor;

out VS_OUT {
    vec3 Position;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;

struct Light{
    vec4 Position;
    vec3 Intensity;
};

struct Material{
    vec3 Kd;
    vec3 Ka;
    vec3 Ks;
    float Shininess;
};

const float density = 0.007;
const float gradient = 1.5;


void main(void)
{
        float heightMultiplier = 30;
        vec4 newPos = vec4(aPosition, 1.0);

        if (newPos[1] < 0)
        {
            newPos[1] = newPos[1] * (0.1 * heightMultiplier);
        }
        else 
        {
            newPos[1] = pow(newPos[1], 2) * heightMultiplier;
        }

        vs_out.TexCoords = aTexCoord;
        vs_out.VertexColor = aVertexColor;
        vs_out.Normal =  mat3(transpose(inverse(model))) * aNormal ;

        newPos = model * newPos ;

        vs_out.Position = vec3(newPos);
        gl_Position = newPos * view * projection;


}