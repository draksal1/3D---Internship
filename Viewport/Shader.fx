cbuffer ConstantBuffer : register(b0)
{
    matrix WorldViewProjection; // Матрица преобразования мира-вида-проекции
};

struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR;
};

PS_IN VS(VS_IN input)
{
    PS_IN output;

    output.pos = mul(input.pos, WorldViewProjection);

    output.col = input.col;

    return output;
}

float4 PS(PS_IN input) : SV_TARGET
{
    return input.col;
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
