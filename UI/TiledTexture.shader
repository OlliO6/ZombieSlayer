shader_type canvas_item;

uniform vec2 frames = vec2(1, 1);
uniform vec2 currentFrame = vec2(0, 0);

void fragment(){
	vec2 tileSize = vec2(1, 1) / frames;
	
	if (UV.x < 0.0 || UV.x > tileSize.x || UV.y < 0.0 || UV.y > tileSize.y)
		return;
	
	vec2 calculatedUV = UV + ((tileSize) * vec2(currentFrame));
	
	
	COLOR = texture(TEXTURE, calculatedUV);
}