Shader "Custom/SimplePBR"
{
    // This Shader does not fully implement all reflection-related ambient lights.
    // Works best if Environment Lighting is set to "Color"
    //Occulsion and height/displacement map not implented
    Properties
    {
        _AlbedoMap ("Albedo", 2D) = "white" {}
        _MetallicMap ("Metallic", 2D) = "white" {}
        _RoughnessMap ("Roughness", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "bump" {}
        _Smoothness ("smoothness",Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
        }

        Pass
        {
            Tags
            {
                "LightMode"="UniversalForward"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Shader Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Smoothness;
            CBUFFER_END

            TEXTURE2D(_AlbedoMap);
            SAMPLER(sampler_AlbedoMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);
            SAMPLER(sampler_RoughnessMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            struct VertexInput
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangentOS : TANGENT;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            // Vertex Shader
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS, v.tangentOS);
                o.tangentWS = float4(normalInputs.tangentWS, v.tangentOS.w);

                o.uv = v.uv;
                return o;
            }

            // Fragment Shader
            float4 frag(VertexOutput i) : SV_Target
            {
                // Sample textures
                float3 albedo = SAMPLE_TEXTURE2D(_AlbedoMap, sampler_AlbedoMap, i.uv).rgb;
                float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, i.uv).r;
                //If using a fancy combined metallic / roughness map
                float smoothness = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, i.uv).a * _Smoothness;
                //If using a roughness map!
                float smoothnessFromRoughNessMap = (1.0 - SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, i.uv).r)
                    * _Smoothness;
                float3 tangentNormal = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                float3 bitangentWS = cross(i.normalWS, i.tangentWS.xyz) * i.tangentWS.w;
                float3x3 tangentToWorld = float3x3(i.tangentWS.xyz, bitangentWS, i.normalWS);
                float3 normalWS = normalize(mul(tangentNormal, tangentToWorld));


                //  SurfaceData
                SurfaceData surface = (SurfaceData)0;
                surface.albedo = albedo;
                surface.metallic = metallic;
                surface.smoothness = smoothnessFromRoughNessMap;
                surface.normalTS = tangentNormal;
                surface.emission = 0;
                surface.occlusion = 1;
                surface.alpha = 1; // alpha could be read from for example albedo if need be.
                surface.specular = 1; // specular color is simply white...

                //  InputData
                InputData input = (InputData)0;
                input.positionWS = i.positionWS;
                input.normalWS = normalWS;
                input.viewDirectionWS = normalize(_WorldSpaceCameraPos - i.positionWS);
                input.shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                input.tangentToWorld = tangentToWorld;

                //This does not properly take into account reflections and other advanced light features. 
                float3 ambientLight = SampleSH(input.normalWS) * surface.albedo;

                return UniversalFragmentPBR(input, surface) + float4(ambientLight, 0);
            }
            ENDHLSL
        }
    }
}