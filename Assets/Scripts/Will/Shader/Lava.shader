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
        _BaseColor ("Cold Lava Color", Color) = (1,1,1,1)
        _FlowColor ("Lava Color", Color) = (1,1,1,1)
        //
        _DistorScale ("Distortion Scale", float) = 0
        _Emission ("Lava Emission", Range(0,2)) = 0
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
        
        sampler2D _MainTex, _NoiseTex, _FlowTex, _FlowMap;
        fixed4 _BaseColor, _DistorScale, _FlowColor;
        float4 _FlowMapOffset;
        half _Emission, _PhaseLength, _RevealPow, _RevealSize, _Strength;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_FlowTex;
            float2 uv_FlowMap;
            float2 uv_NoiseTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 flowMap = tex2D (_FlowMap, IN.uv_FlowMap);
            flowMap.r = flowMap.r * 2.0f - 1.011765;
            flowMap.g = flowMap.g * 2.0f - 1.003922;

            half gradient = _RevealSize;
            gradient += flowMap.a;
            gradient = clamp(gradient,0,1);
            gradient = pow(gradient, _RevealPow);

            flowMap.b *= gradient;


            float phase1 = _FlowMapOffset.x;
            float phase2 = _FlowMapOffset.y;

            float noise = tex2D(_NoiseTex, IN.uv_NoiseTex).r * _Strength;

            half4 t1 = tex2D (_FlowTex,IN.uv_FlowTex + flowMap.rg *(phase1+noise));
            half4 t2 = tex2D (_FlowTex, IN.uv_FlowTex + flowMap.rg * (phase2+noise));

            half blend = abs(_PhaseLength - _FlowMapOffset.z) / _PhaseLength;
            blend = max(0,blend);
            half4 final  = lerp(t1,t2,blend);

            half flowMapColor = flowMap.b * _FlowColor.a;

            float2 distUV = (saturate(final)* 2 - 1) * _DistorScale * flowMap.b;
            half4 mainColor = tex2D (_MainTex, IN.uv_MainTex + distUV);

            mainColor.rgb *= _BaseColor * (1 - flowMapColor);
            final.rgb *= _FlowColor.rgb * flowMapColor;

            o.Albedo = mainColor.rgb + final.rgb;
            o.Emission = o.Albedo.rgb * _Emission * flowMap.b;
            o.Alpha = mainColor.a * _BaseColor.a * flowMap.b;
        }
        ENDCG
    }
    FallBack "Diffuse"
}