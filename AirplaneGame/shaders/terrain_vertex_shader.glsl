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
} vs_out;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 transform;

void main(void)
{
        vs_out.TexCoords = aTexCoord;
        vs_out.VertexColor = aVertexColor;

        vs_out.FragPos = aPosition;
        vs_out.Normal = aNormal;
        gl_Position = vec4(aPosition, 1.0) * view * projection;
}