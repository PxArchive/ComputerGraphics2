#ifndef CUSTOM_SHADER_GRAPH_FUNCTIONS_INCLUDED
#define CUSTOM_SHADER_GRAPH_FUNCTIONS_INCLUDED

void tint_float(float4 Col, float4 tintCol, out float4 outColor)
{
    outColor = Col * tintCol;
}

#endif // CUSTOM_SHADER_GRAPH_FUNCTIONS_INCLUDED