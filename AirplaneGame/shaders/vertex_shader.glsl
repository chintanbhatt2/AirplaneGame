#version 330

layout(location=0) in vec3 vPosition;
layout(location=1) in vec3 vColors;
uniform vec4 shifting_color;
out vec4 color_in;


void main() {
	gl_Position = vec4(vPosition, 1.0);
	color_in = shifting_color;
//	color_in = vec3(vColors);
}