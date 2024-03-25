#version 440 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUV;

out vec2 pass_uv;

uniform mat4 transformation;
uniform mat4 projection;
uniform mat4 view;

void main(){
	gl_Position = projection * view * transformation * vec4(aPos, 1.0);
	pass_uv = aUV;
}