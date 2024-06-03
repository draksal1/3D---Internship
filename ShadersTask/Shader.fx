struct LightData
{
    float4 LightPos;
    float4 LightColor;
};

struct MVP
{
    matrix Model;
    matrix View;
    matrix Projection;
};
 
cbuffer WorldViewProjectionBuffer : register(b0)
{
    MVP mvp;
};

cbuffer LightBuffer : register(b1)
{
    LightData light;
};

cbuffer ViewBuffer : register(b2)
{
    float4 ViewPosition;
};

struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
    float4 normal : NORMAL;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 fragPos : TEXCOORD2;
    float4 viewPos : TEXCOORD0;
    float4 lightPos : TEXCOORD1;
    float4 lightCol : COLOR0;
    float4 col : COLOR1;
    float4 normal : NORMAL;
};

PS_IN VS(VS_IN input)
{
    PS_IN output;

    output.pos = mul(mvp.Projection, mul(mvp.View, mul(mvp.Model, input.pos)));
    
    output.lightCol = light.LightColor;
    
    output.lightPos = light.LightPos;
    
    output.viewPos = ViewPosition;
    
    output.col = input.col;
    
    output.normal = mul(mvp.Model, input.normal);
    
    output.fragPos = mul(mvp.Model , input.pos);

    return output;
}
float4 PS(PS_IN input) : SV_TARGET
{
    
    float3 lightPos = input.lightPos.xyz;
    float3 pos = input.fragPos.xyz;
    float3 viewPos = input.viewPos.xyz;
    float3 LightColor = input.lightCol.xyz;
    
    float3 lightDir = normalize(lightPos - pos);
    float3 normal = normalize(input.normal.xyz);
    

    float3 viewDir = normalize(viewPos - pos);

    float3 reflectDir = reflect(-lightDir, normal);
    
    float ambientStrength = 0.1;
    float3 ambient = ambientStrength * LightColor;

    float diff = max(dot(normal, lightDir), 0.0);
    float3 diffuse = diff * LightColor;

    float specularStrength = 0.5;
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    float3 specular = specularStrength * spec * LightColor;


    float3 result = (ambient + diffuse + specular) * input.col.xyz;

    return float4(result, 1.0);

}

technique10 Render
{
    pass P0
    {
        SetGeometryShader(0);
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS()));
    }
}

