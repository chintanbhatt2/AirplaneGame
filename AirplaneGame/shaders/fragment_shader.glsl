#version 330

out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

in VS_OUT {
    vec4 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;

void main()
{
	FragColor = vs_out.VertexColor;
}