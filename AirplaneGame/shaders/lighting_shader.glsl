#version 440 core 


out vec4 fragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 lightPos;

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;

};

struct Light {
    vec3  Position;
    vec3  Direction;
    float CutOff;
    float OuterCutOff;

    vec3 Ambient;
    vec3 Diffuse;
    vec3 Specular;

    float Constant;
    float Linear;
    float Quadratic;
};

uniform Light light;
uniform Material material;
uniform vec3 ViewPosition;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;

void main()
{
    //ambient
    vec3 AmbientVec = light.Ambient * vec3(vs_out.VertexColor);

    //diffuse
    vec3 norm = normalize(vs_out.Normal);
    vec3 lightDirection = normalize(light.Position - vs_out.FragPos);
    float Diffuse = max(dot(norm, light.Direction), 0.0);
    vec3 DiffuseVec = light.Diffuse * Diffuse * vec3(vs_out.VertexColor);

    //Specular
    vec3 ViewDirection = normalize(ViewPosition - vs_out.FragPos);
    vec3 reflectDir = reflect(-light.Direction, norm);
    float spec = pow(max(dot(ViewDirection, reflectDir), 0.0), 0.5);
    vec3 SpecularVec = light.Specular * spec * vec3(vs_out.VertexColor);

    //Attenuation 
    float distance = length(light.Position - vs_out.FragPos);
    float attenuation = 1.0 / (light.Constant + light.Linear * distance * light.Quadratic * (distance * distance));
    

    float theta = dot(light.Direction, normalize(-light.Direction));
    float epsilon = light.CutOff - light.OuterCutOff;
    float intensity = clamp((theta - light.OuterCutOff) / epsilon, 0.0, 0.1);

    AmbientVec *= attenuation;
    DiffuseVec *= attenuation * intensity;
    SpecularVec *= attenuation * intensity;

//    vec4 result = (Ambient + Diffuse) * vs_out.VertexColor;
    
//    vec3 result = Ambient * diffuseVec * Specular;
    vec3 result = SpecularVec * DiffuseVec * AmbientVec;


	fragColor = vec4(result, 1.0);

}
