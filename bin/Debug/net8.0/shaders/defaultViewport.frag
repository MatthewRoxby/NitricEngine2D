#version 440 core

in vec2 pass_uv;

out vec4 fragColour;

uniform sampler2D texture0;

void main(){
	vec4 t = texture(texture0, pass_uv);

	fragColour = vec4(t);
}