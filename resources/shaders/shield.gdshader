shader_type canvas_item;

uniform float time : hint_range(0, 1) = 0.0;

void fragment() {
    vec4 texture_color = texture(TEXTURE, UV);

    // Definir dirección y velocidad de flujo
    vec2 flow_direction = normalize(vec2(1.0, 1.0));
    float speed = 3.0;

    // Desplazar UV basado en el tiempo y la dirección
    vec2 displaced_UV = UV + flow_direction * time * speed;

    // Calcular las ondulaciones
    float wave = sin(displaced_UV.y * 10.0 + displaced_UV.x * 3.0) * 0.5 + 0.5;

    vec4 base_blue = vec4(0.4, 0.6, 0.8, 0.3);
    vec4 light_blue = mix(base_blue, vec4(0.6, 0.8, 1.0, 0.7), wave);

    COLOR.rgb = light_blue.rgb;
    COLOR.a = light_blue.a * texture_color.a;
}
