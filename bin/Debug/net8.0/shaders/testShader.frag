#version 440 core

in vec2 pass_uv;

out vec4 fragColour;

uniform float time;

void main(){
	fragColour = vec4(pass_uv, sin(time) * 0.5 + 0.5, 1.0);
}