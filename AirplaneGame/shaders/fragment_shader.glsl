#version 330

out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

void main()
{
//    outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);
	FragColor = vec4(lightColor * objectColor, 1.0);
}