shader_type canvas_item;

uniform bool blinking = true;
uniform float blinkSpeed = 8;

// Start Option Methods
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}
// End Option Methods

void fragment()
{
	if (blinking && sin(TIME * blinkSpeed) > 0.0){
		return;
	}
	COLOR = GetColor(UV, TEXTURE, TEXTURE_PIXEL_SIZE);
}