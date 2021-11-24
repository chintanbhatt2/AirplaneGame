#version 430 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec4 aVertexColor;

out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
    float DistToCam;
    float Visibility;
} vs_out;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;
uniform mat4 transform;

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
        newPos = model * newPos ;




        vs_out.FragPos = vec3(newPos);
        vs_out.Normal = mat3(transpose(inverse(model))) * aNormal;
        gl_Position = newPos * view * projection;

        vec3 ndc = gl_Position.xyz / gl_Position.w;
        vs_out.DistToCam = ndc.z * 0.5 + 0.5;

        vs_out.Visibility = exp(-pow((vs_out.DistToCam * density), gradient));
}