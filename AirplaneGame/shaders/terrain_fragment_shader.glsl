#version 440

out vec4 fragColor;


struct Light {

	vec3 Position;
	vec3 Direction;
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
uniform vec3 ViewPosition;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    flat vec4 VertexColor;
	float DistToCam;
	float Visibility;
} vs_out;



void main()
{
	
	vec3 AmbientVec = light.Ambient * vec3(vs_out.VertexColor);
	fragColor = vec4(AmbientVec, 1);
	fragColor = mix(vec4(255, 255, 255, 255), fragColor, vs_out.Visibility);
}