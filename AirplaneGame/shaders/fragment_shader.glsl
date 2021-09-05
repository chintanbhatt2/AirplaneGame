#version 330

in vec4 color_in;

out vec4 color_out;



void main() {
//	color_out = vec4(color_in, 0.0f);
	//color_out = vec4(0.0f, 1.0f, 1.0f, 1.0f);
	color_out = color_in;
}