#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;
uniform vec2 uOffset;
uniform vec2 uScale;
uniform vec2 uScreenSize;
uniform vec2 uUVOffset;
uniform vec2 uUVScale;

void main() {
    TexCoord = uUVOffset + aTexCoord * uUVScale;
    vec2 pixel = aPos * uScale + uOffset;
    vec2 ndc = (pixel / uScreenSize) * 2.0 - 1.0;
    ndc.y = -ndc.y;
    gl_Position = vec4(ndc, 0.0, 1.0);
}