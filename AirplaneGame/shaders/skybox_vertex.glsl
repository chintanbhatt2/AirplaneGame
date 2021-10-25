#version 430 core
//Skybox Vertex Shader
layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;


void main()
{
	TexCoords = aPos;
	vec4 newPos = model * vec4(aPos, 1.0);
	newPos = newPos * view * projection;
	gl_Position = newPos.xyww;
}