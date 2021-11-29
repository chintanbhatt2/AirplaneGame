#version 440 core 


out vec4 fragColor;

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


in VS_OUT {
    vec3 Position;
    vec3 Normal;
    vec2 TexCoords;
    vec4 VertexColor;
} vs_out;

uniform Light light;
uniform Material mat;

vec3 ads()
{
    vec3 s;
        s = normalize(vec3(light.Position));

    vec3 surfaceColor = vec3(vs_out.VertexColor);
    vec3 newd = vec3(0.2,0.2,0.2);
    vec3 newa = vec3(0.4);
    vec3 n = normalize(vs_out.Normal);
    vec3 v = normalize(vec3(-vs_out.Position));
    vec3 r = reflect(-s, n);
    return  
            ( mat.Ka +
            mat.Kd * max( dot(s, n), 0.0 ) +
            mat.Ks * pow( max( dot(r,v), 0.0 ), mat.Shininess ) ) * light.Intensity ;
}

void main()
{
	fragColor = vec4(mix(ads(), vec3(vs_out.VertexColor), 0.5416894), 1.0);
//    fragColor = vs_out.VertexColor;
}