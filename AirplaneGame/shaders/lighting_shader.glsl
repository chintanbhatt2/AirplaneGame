#version 440 core 


out vec4 fragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 lightPos;
in VS_OUT {
    vec4 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;

void main()
{
    float Ambient = 0.1;
    vec3 norm = normalize(vs_out.Normal);
    vec3 lightDirection = normalize(lightPos - vec3(vs_out.FragPos));
    float Diffuse = max(dot(norm, lightDirection), 0.0);
    vec3 diffuseVec = Diffuse * lightColor;
    vec4 result = (Ambient + Diffuse) * vs_out.VertexColor;
    fragColor = result;


}
