shader_type canvas_item;

uniform vec2 frames = vec2(1, 1);
uniform vec2 currentFrame = vec2(0, 0);

// Start Option Methods
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}
// End Option Methods

void fragment()
{
	vec2 tileSize = vec2(1, 1) / frames;
	vec2 calculatedUV = UV + ((tileSize) * vec2(currentFrame));
	
	if (UV.x > tileSize.x || UV.y > tileSize.y)
		discard;
	
	COLOR = GetColor(calculatedUV, TEXTURE, TEXTURE_PIXEL_SIZE);
}