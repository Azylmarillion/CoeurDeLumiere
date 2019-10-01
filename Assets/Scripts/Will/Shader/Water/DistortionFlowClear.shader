﻿Shader "Custom/DistortionFlowClear"
{
    Properties
    {
        [NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}
        [NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" {}
        _MainTex ("Albedo (RGB)", 2D) = "white" {} 
        _Color ("Color", Color) = (1,1,1,1)
        _FlowStrength ("Flow Strength", Float) = 1
        _FlowOffset ("Flow Offset", Float) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _HeightScale ("Height Scale, Constant", Float) = .25
        _HeightScaleModulated ("Height Scale, Modulated", Float) = .75
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _RefractionStrength ("Reraction Strength", Range(0, 1)) = .25
        _Speed("Speed",Float) = 1
        _Tiling ("Tiling",Float) = 1
        _UJump ("U jump per phase", Range(-0.25,0.25)) = 0.25
        _VJump ("V jump per phase", Range(-0.25,0.25)) = 0.25
        _WaterFogColor ("Water Fog Color", Color) = (0,0,0,0)
        _WaterFogDensity ("Water Fog Density", Range(0,2)) = .1       
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" "Queue"="Transparent" 
        }
        LOD 200

        GrabPass
        {
            "_WaterBackground"
        }

        CGPROGRAM
        #pragma surface surf Standard alpha finalcolor:ResetAlpha
        #pragma target 3.0

        #include "FlowClear.cginc"                
        #include "LookingThroughWater.cginc"

        sampler2D _DerivHeightMap, _FlowMap, _MainTex;
        float _FlowStrength, _FlowOffset, _HeightScale, _HeightScaleModulated, _Speed, _Tiling, _UJump, _VJump;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void ResetAlpha (Input IN,SurfaceOutputStandard o , inout fixed4 color)
        {
            color.a =1;
        }

        float3 UnpackDerivativeHeight (float4 textureData)
        {
            float3 dh = textureData.agb;
            dh.xy = dh.xy * 2 -1;
            return dh;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 flow = tex2D(_FlowMap, IN.uv_MainTex).rgb;
            flow.xy = flow.xy * 2 - 1;
            flow *= _FlowStrength;
            float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
            float time = _Time.y * _Speed + noise;
            
            float2 jump = float2(_UJump,_VJump);

            float3 uvwA = FlowUVW(IN.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling,time, false);
            float3 uvwB = FlowUVW(IN.uv_MainTex, flow.xy,jump, _FlowOffset, _Tiling,time, true);

            float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;
            float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap,uvwA.xy)) * (uvwA.z * finalHeightScale);
            float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap,uvwB.xy)) * (uvwB.z * finalHeightScale);
            o.Normal = normalize(float3(-(dhA.xy + dhB.xy), 1));

            fixed4 textA = tex2D(_MainTex, uvwA.xy)* uvwA.z;
            fixed4 textB = tex2D(_MainTex, uvwB.xy)* uvwB.z;

            fixed4 c = (textA + textB) * _Color;
            o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
			o.Emission = ColorBelowWater(IN.screenPos, o.Normal) * (1 - c.a);
        }
        ENDCG
    }
}
