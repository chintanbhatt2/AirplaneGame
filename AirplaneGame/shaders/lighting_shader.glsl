#version 440 core 


out vec4 fragColor;

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
	FragColor = vec4(lightColor * vs_out.VertexColor, 1.0);

}
