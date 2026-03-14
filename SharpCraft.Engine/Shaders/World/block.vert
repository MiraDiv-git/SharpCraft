#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aUV;
layout (location = 2) in mat4 aModel;

uniform mat4 uView;
uniform mat4 uProjection;

out vec2 vUV;

void main()
{
    vUV = aUV;
    gl_Position = uProjection * uView * aModel * vec4(aPos, 1.0);
}