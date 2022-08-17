shader_type canvas_item;

uniform float size = 1.0;
uniform float smoothness : hint_range(0.0, 1.0) = 1.0;
uniform vec4 color : hint_color = vec4(0, 0, 0, 0.5);

void fragment()
{
	float a = smoothstep(size - smoothness * 0.5, size + smoothness * 0.5, length(UV - vec2(0.5))) * color.a;
	
	COLOR.rgb = color.rgb;
	COLOR.a = a;
}