﻿#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float shininess;
};

struct Light {
    vec3 direction;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;

uniform Material material;
uniform vec3 lightPos;
uniform vec3 viewPos;
uniform Light light;

void main()
{

    vec3 norm = normalize(Normal);
    vec3 lightDirection = normalize(-light.direction);

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDirection, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);

    vec3 ambient = light.ambient * material.ambient;


    float diff = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = light.diffuse * (diff * material.diffuse);

    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}