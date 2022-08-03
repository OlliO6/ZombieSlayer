tool
extends RichTextEffect
class_name TintTextEffect

# [tint r=1 g=1 b=1 a=1][/tint]
var bbcode := "tint"

func _process_custom_fx(char_fx):
	
	var r:float = char_fx.env.get("r", 1.0)
	var g:float = char_fx.env.get("g", 1.0)
	var b:float = char_fx.env.get("b", 1.0)
	var a:float = char_fx.env.get("a", 1.0)
	
	char_fx.color = Color(r, g, b, a)
	
	return true
