#version 330 core
out vec4 FragColor;

in vec2 TexCoord;
in vec4 vColor;

uniform sampler2D uTexture;
uniform int uUseTexture;

void main() {
    vec4 tex = (uUseTexture == 1) ? texture(uTexture, TexCoord) : vec4(1.0);
    FragColor = tex * vColor;
}