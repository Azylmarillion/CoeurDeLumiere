Shader "Custom/Lava"
{
    Properties
    {
        //Textures
        _MainTex ("Base Tex", 2D) = "" {}
        _NoiseTex ("Flow Noise Tex", 2D) = "" {}
        _FlowTex ("Flow Tex", 2D) = "" {}        
        //Maps
        _FlowMap ("FlowMap", 2D) = ""{}
        //Colors
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _FlowColor ("Flow Color", Color) = (1,1,1,1)
        //
        _DistorScale ("Distortion Scale", float) = 0
        _Emission ("Flow Emission", Range(0,2)) = 0
        _RevealSize ("Flow Reveal Size", Range(-1,1)) = 1
        _RevealPow ("Reveal Pow", Range(1,128)) = 1
        _Strength ("Noise strenght", Range(0,1)) = 0
    }
    SubShader
    {
        Tags
        {
             "RenderType"="Opaque"
             "FlowTag" = "Flow"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0

        #include "Flow.cginc"
        
        sampler2D _MainTex, _NoiseTex, _FlowTex, _FlowMap;
        fixed4 _BaseColor, _DistorScale, _FlowColor;
        float4 _FlowMapOffset;
        half _Emission, _PhaseLength, _RevealPow, _RevealSize, _Strength;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_FlowTex;
            float2 uv_FlowMap;
            float2 uv_Noise;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}