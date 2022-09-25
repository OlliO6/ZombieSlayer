shader_type canvas_item;

uniform bool blinking = true;
uniform float blinkSpeed = 8;
uniform vec4 flashColor : hint_color;
uniform float flashStrenght : hint_range(0,1) = 0.0;

// Start Option Methods
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}
// End Option Methods

void fragment()
{
	if (blinking && sin(TIME * blinkSpeed) > 0.0) {
		discard;
	}
	
	vec4 color = GetColor(UV, TEXTURE, TEXTURE_PIXEL_SIZE);
	color.rgb = mix(color.rgb, flashColor.rgb, flashStrenght);
	COLOR = color;
}