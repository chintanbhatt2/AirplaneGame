#version 430 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec4 aVertexColor;

out VS_OUT {
    vec3 Position;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;



uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;
uniform mat4 transform;


struct Light{
    vec4 Position;
    vec3 Intensity;
};

struct Material{
    vec3 Kd;
    vec3 Ka;
    vec3 Ks;
    float Shininess;
};


uniform Light light;
uniform Material mat;



out Light lightTest;
out Material matTest;

void main(void)
{
        vs_out.TexCoords = aTexCoord;
        vs_out.VertexColor = aVertexColor;
        vec4 newPos = model * vec4(aPosition, 1.0);
        
        
        vs_out.Position = vec3(newPos);
        vs_out.Normal = mat3(transpose(inverse(model))) * aNormal;



        lightTest = light;
        matTest = mat;

        gl_Position =  vec4(aPosition, 1.0) * model * view * projection;
}