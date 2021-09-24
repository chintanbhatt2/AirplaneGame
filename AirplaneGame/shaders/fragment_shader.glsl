#version 330

out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
	FragColor = vec4(0.5, 0.5, 0.5, 1.0);
}