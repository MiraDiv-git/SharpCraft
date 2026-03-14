#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aOther;
layout (location = 2) in float aSide;

uniform mat4 uView;
uniform mat4 uProjection;
uniform vec2 uScreenSize;
uniform float uThickness;

void main() {
    vec4 p0 = uProjection * uView * vec4(aPos, 1.0);
    vec4 p1 = uProjection * uView * vec4(aOther, 1.0);

    vec2 screen0 = p0.xy / p0.w * vec2(uScreenSize.x / uScreenSize.y, 1.0);
    vec2 screen1 = p1.xy / p1.w * vec2(uScreenSize.x / uScreenSize.y, 1.0);

    vec2 dir = normalize(screen1 - screen0);
    vec2 normal = vec2(-dir.y, dir.x);
    normal.x /= (uScreenSize.x / uScreenSize.y);

    p0.xy += normal * aSide * uThickness * p0.w;
    gl_Position = p0;
}