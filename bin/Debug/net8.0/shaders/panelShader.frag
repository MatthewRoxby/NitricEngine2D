#version 440 core

in vec2 pass_uv;

out vec4 FragColour;

uniform float corner_radius;
uniform float border_size;
uniform vec4 panel_colour;
uniform vec4 border_colour;

uniform vec2 asp;

//returns 0 for nothing, 1 for border and 2 for panel
int border(vec2 uv, vec2 bs, vec2 cr){
	if(uv.x < bs.x || uv.x > 1.0 - bs.x || uv.y < bs.y || uv.y > 1.0 - bs.y){
		return 1;
	}
	else{
		return 2;
	}
}

//0 for top-left, 1 for top-right, 2 for bottom-left, 3 for bottom-right
int corner(float min_r, float max_r, vec2 cPos, vec2 uv, int corner){
	float d = distance(cPos, uv);
    
    
    
	if(corner == 0){
        if (uv.x > cPos.x || uv.y > cPos.y){
            return 2;
        }
    }
	else if(corner == 1){
        if(uv.x < cPos.x || uv.y > cPos.y){
        	return 2;  
        } 
    }
	else if(corner == 2){
        if (uv.x > cPos.x || uv.y < cPos.y){
            return 2;
        }
    }
	else if(corner == 3){
        if (uv.x < cPos.x || uv.y < cPos.y){
            return 2;
        }
    }
	
	if(d > min_r && d < max_r){
        return 1;
    }
	if(d < min_r){
        return 2;
    }
	if(d > max_r){
        return 0;
    }
}

void main(){
	vec2 a;
	if(asp.x > asp.y){
		a.y = 1.0;
		a.x = asp.x / asp.y;
	}
	else{
		a.x = 1.0;
		a.y = asp.y / asp.x;
	}
	
	vec2 bs = vec2(border_size, border_size) / a;
	vec2 cr = vec2(corner_radius, corner_radius) / a;

	int corna;
	if(pass_uv.x <= 0.5 && pass_uv.y <= 0.5){
		corna = corner(corner_radius - border_size, corner_radius, vec2(corner_radius, corner_radius), pass_uv * a, 0);
	}
	else if(pass_uv.x >= 0.5 && pass_uv.y <= 0.5){
		corna = corner(corner_radius - border_size, corner_radius, vec2(1.0 * a.x - corner_radius, corner_radius), pass_uv * a, 1);
	}
	else if(pass_uv.x <= 0.5 && pass_uv.y >= 0.5){
		corna = corner(corner_radius - border_size, corner_radius, vec2(corner_radius, 1.0 * a.y - corner_radius), pass_uv * a, 2);
	}
	else if(pass_uv.x >= 0.5 && pass_uv.y >= 0.5){
		corna = corner(corner_radius - border_size, corner_radius, vec2(1.0 * a.x - corner_radius, 1.0 * a.y - corner_radius), pass_uv * a, 3);
	}
	
	int borda = border(pass_uv, bs, cr);
	
	if(corna == 1 || borda == 1){
		gl_FragColor = border_colour;
	}
	else{
		gl_FragColor = panel_colour;
	}
	
	if(corna == 0 || borda == 0){
		gl_FragColor.a = 0.0;
	}
}