#version 440 core


in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;


out vec4 FragColor;

void main()
{
    FragColor = vec4(vs_out.Normal, 1.0);
    
}