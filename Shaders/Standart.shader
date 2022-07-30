shader_type canvas_item;

// Start Option Methods
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}
// End Option Methods

void fragment()
{
	COLOR = GetColor(UV, TEXTURE, TEXTURE_PIXEL_SIZE);
}