#version 330 core
out vec4 FragColor;
uniform vec4 uColor;

in vec2 TexCoord;

uniform sampler2D uTexture;
uniform int uUseTexture;

void main() {
    vec4 tex = (uUseTexture == 1) ? texture(uTexture, TexCoord) : vec4(1.0);
    FragColor = tex * uColor;
}