#version 440 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aUV;

uniform mat4 transformation;
uniform mat4 projection;
uniform mat4 view;

uniform vec2 aspect;

out vec2 pass_uv;

void main(){
	gl_Position = vec4(aPos * vec3(aspect, 1.0), 1.0);
	pass_uv = vec2(aUV.x, 1.0 - aUV.y);
}