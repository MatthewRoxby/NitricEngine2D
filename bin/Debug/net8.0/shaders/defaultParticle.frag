#version 440 core

in vec2 pass_uv;

out vec4 fragColour;

uniform sampler2D albedo;
uniform int textureEnabled;
uniform vec4 modulate;

void main(){
	vec4 t = vec4(1.0,1.0,1.0,1.0);
	if(textureEnabled == 1){
		t = texture(albedo, pass_uv);
	}
	
	

	fragColour = vec4(modulate * t);
}