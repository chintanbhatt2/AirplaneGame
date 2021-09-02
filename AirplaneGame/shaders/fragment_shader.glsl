#version 330


in vec3 color_in;

out vec4 color_out;

void main() {
	color_out = vec4(color_in[0], color_in[1], color_in[2], 1.0f);
}