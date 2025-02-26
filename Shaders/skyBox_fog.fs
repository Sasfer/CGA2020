#version 330 core

in vec3 our_uv;
out vec4 color;

uniform samplerCube skybox;
uniform float lowerLimit = 0.0;
uniform float upperLimit = 0.02;
uniform vec3 fogColor;

void main()
{
   //color = texture(skybox, our_uv);
   vec4 finalColor = texture(skybox,our_uv);
   // Factor de ponderacion dependiendo de la altura
   float factor = (our_uv.y - lowerLimit) / (upperLimit - lowerLimit);
   factor = clamp(factor, 0.0, 1.0);
   color = mix(vec4(fogColor, 1.0), finalColor, factor);
}
