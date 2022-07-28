shader_type canvas_item;

uniform vec4 flashColor : hint_color;
uniform float flashStrenght : hint_range(0,1) = 0.0;

void fragment(){
	vec4 fullColor = texture(TEXTURE, UV);
	
	vec3 color = fullColor.rgb;
	float alpha = fullColor.a;
	
	color = mix(fullColor.rgb, flashColor.rgb, flashStrenght);
	
	COLOR.rgb = color;
	COLOR.a = alpha;
}