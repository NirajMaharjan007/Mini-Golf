shader_type canvas_item;

uniform sampler2D screen_tex : hint_screen_texture, repeat_disable, filter_linear;

uniform float sigma : hint_range(0.1, 15.0) = 4.0;       // blur strength
uniform int radius : hint_range(1, 16) = 8;              // kernel half-size, ~2-3x sigma
uniform float opacity : hint_range(0.0, 1.0) = 1.0;

void fragment() {
    float s2 = 2.0 * sigma * sigma;
    vec3 sum = vec3(0.0);
    float total = 0.0;

    for (int x = -radius; x <= radius; x++) {
        for (int y = -radius; y <= radius; y++) {
            vec2 o = vec2(float(x), float(y));
            float w = exp(-dot(o, o) / s2);
            sum += texture(screen_tex, SCREEN_UV + o * SCREEN_PIXEL_SIZE).rgb * w;
            total += w;
        }
    }

    COLOR = vec4(sum / total, opacity);
}