#version 330 core

layout(location = 0) in vec4 posAttr;
layout(location = 1) in vec4 colAttr;

out vec4 col;

uniform mat4 matrix;
uniform float morph;

void main() {
    vec3 normalPosAttr = normalize(posAttr.xyz);
    vec3 mixedPosAttr = mix(normalPosAttr, posAttr.xyz, morph);
    col = colAttr;
    gl_Position = matrix * vec4(mixedPosAttr, 1.0);
}
