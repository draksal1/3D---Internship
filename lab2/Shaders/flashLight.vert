#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float shininess;
};

struct Light {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;	
};

out vec4 FragCol;

layout(location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;

uniform Material material;
uniform vec3 viewPos;
uniform Light light;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 matrix;

void main()
{
    gl_Position = matrix * vec4(aPosition, 1.0) * model * view * projection;
    vec3 FragPos = vec3(matrix * model * vec4(aPosition, 1.0));
    vec3 Normal = (matrix * vec4(aNormal, 1.0)).xyz;

    vec3 norm = normalize(Normal);

    vec3 lightDirection = normalize(light.position - FragPos);

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDirection, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);

    vec3 ambient = light.ambient * material.ambient;


    float diff = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = light.diffuse * (diff * material.diffuse);

    float theta     = dot(lightDirection, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);    

    diffuse  *= intensity;
    specular *= intensity;
    vec3 result = ambient + diffuse + specular;
    FragCol = vec4(result, 1.0);
}