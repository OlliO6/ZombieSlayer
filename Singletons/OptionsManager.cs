using Godot;
using System;

public class OptionsManager : Node
{
    public static OptionsManager instance;

    public static Shader[] spriteShaders = new Shader[]
    {
        GD.Load<Shader>("res://Shaders/Standart.shader"),
        GD.Load<Shader>("res://Shaders/Flash.shader"),
        GD.Load<Shader>("res://Shaders/Player.shader"),
        GD.Load<Shader>("res://Shaders/CuttetTexture.shader"),
    };

    public override void _Ready()
    {
        instance = this;
        UpdateOptions();
        UpdateShaders();
    }

    public static float sfxVolume = 0.7f;
    public static float musicVolume = 0.7f;
    public static bool fullscreen = true;
    public static bool useUpscaling = true;

    public static void UpdateOptions() => instance._UpdateOptions();
    private void _UpdateOptions()
    {
        OS.WindowFullscreen = fullscreen;

        if (sfxVolume is 0)
        {
            AudioServer.SetBusMute(1, true);
        }
        else
        {
            AudioServer.SetBusMute(1, false);
            AudioServer.SetBusVolumeDb(1, GD.Linear2Db(sfxVolume)); // Weird calculation because decibles are weird
        }

        if (musicVolume is 0)
        {
            AudioServer.SetBusMute(2, true);
        }
        else
        {
            AudioServer.SetBusMute(2, false);
            AudioServer.SetBusVolumeDb(2, GD.Linear2Db(musicVolume));
        }
    }

    public static void UpdateShaders() => instance._UpdateShaders();

    private void _UpdateShaders()
    {
        #region Shader code 

        const string StartOptions = "// Start Option Methods";
        const string EndOptions = "// End Option Methods";

        const string UpscaleCode = @"const int ML = 0;
const float THRESHOLD = 0.05;
const float AA_SCALE = 10.0;

vec4 texelGet ( sampler2D tg_tex, ivec2 tg_coord, int tg_lod ) {
	vec2 tg_texel = 1.0 / vec2(textureSize(tg_tex, 0));
	vec2 tg_getpos = (vec2(tg_coord) * tg_texel) + (tg_texel * 0.5);
	return texture(tg_tex, tg_getpos, float(tg_lod));
}

vec4 diag(vec4 sum, vec2 uv, vec2 p1, vec2 p2, sampler2D iChannel0, float LINE_THICKNESS) {
	vec4 v1 = texelGet(iChannel0,ivec2(uv+vec2(p1.x,p1.y)),ML),
		v2 = texelGet(iChannel0,ivec2(uv+vec2(p2.x,p2.y)),ML);
	if (length(v1-v2) < THRESHOLD) {
		vec2 dir = p2-p1,
			lp = uv-(floor(uv+p1)+.5);
		dir = normalize(vec2(dir.y,-dir.x));
		float l = clamp((LINE_THICKNESS-dot(lp,dir))*AA_SCALE,0.,1.);
		sum = mix(sum,v1,l);
	}
	return sum;
}
        
vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
	//line thickness
	float LINE_THICKNESS = 0.4;
	vec2 ip = uv;
	ip = uv * (1.0 / pixel_size);

	//start with nearest pixel as 'background'
	vec4 s = texelGet(text, ivec2(ip), ML);

	//draw anti aliased diagonal lines of surrounding pixels as 'foreground'
	s = diag(s,ip,vec2(-1,0),vec2(0,1), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(0,1),vec2(1,0), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(1,0),vec2(0,-1), text, LINE_THICKNESS);
	s = diag(s,ip,vec2(0,-1),vec2(-1,0), text, LINE_THICKNESS);
	
	return s;
}";
        const string NormalCode = @"vec4 GetColor(vec2 uv, sampler2D text , vec2 pixel_size)
{
    return texture(text, uv);
}";

        #endregion Shader code

        string optionsCode = useUpscaling ? UpscaleCode : NormalCode;

        foreach (Shader shader in spriteShaders)
        {
            string code = shader.Code;

            int startPos = code.IndexOf(StartOptions) + StartOptions.Length;
            int endPos = code.IndexOf(EndOptions);

            code = $"{code.Remove(startPos)}\n{optionsCode}\n{code.Remove(0, endPos)}";

            shader.Code = code;
        }
    }
}
