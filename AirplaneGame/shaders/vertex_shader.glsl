#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

out vec2 texCoord;


uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;
uniform mat4 transform;

void main(void)
{
        texCoord = aTexCoord;
        
        vec4 newPos = model * vec4(aPosition, 1.0) ;
        gl_Position = newPos * view * projection;
}