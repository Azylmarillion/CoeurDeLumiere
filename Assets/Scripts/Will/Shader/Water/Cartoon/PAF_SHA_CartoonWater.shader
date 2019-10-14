Shader "Custom/PAF_SHA_CartoonWater"
{
    Properties
    {	_SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceNoise("Surface Noise", 2D) = "white" {}

        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _FoamColor("Foam Color", Color) = (1,1,1,1)

        _AntiAliasingValue("Antialising Intensity",Range(1,20)) = 1

        _DepthMaxDistance("Depth Maximum Distance", Float) = 1        
        _FoamMaxDistance("Foam Max Distance", Float) = .4
        _FoamMinDistance("Foam Min Distance", Float) = .04
        
        _SurfaceDistortionAmount("Surface Distortion Amount" , Range(0,1)) = .3
        
        _SurfaceNoiseCutoff("Surface Noise Cutoff",Range(0,1)) = .7
        _SurfaceNoiseScroll("Surface Noise Scroll Amourt", Vector) = (.03, .03, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

        Pass
        {
			CGPROGRAM
            #define SMOOTHSTEP_AA .001
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
	            float alpha = top.a + bottom.a * (1 - top.a);

	            return float4(color, alpha);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 screenPosition : TEXCOORD2;
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distordUV : TEXCOORD1;
                float3 viewNormal : NORMAL;
            };

            float4 _DepthGradientShallow,_DepthGradientDeep, _FoamColor, _SurfaceNoise_ST, _SurfaceDistortion_ST;
            float _DepthMaxDistance,_FoamMaxDistance, _FoamMinDistance , _SurfaceNoiseCutoff, _SurfaceDistortionAmount, _AntiAliasingValue;
            float2 _SurfaceNoiseScroll;
            sampler2D _CameraDepthTexture, _SurfaceNoise, _SurfaceDistortion, _CameraNormalsTexture;

            v2f vert (appdata v)
            {                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.distordUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
                o.viewNormal = COMPUTE_VIEW_NORMAL;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float existingDepth01 = tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);

                float depthDifference = existingDepthLinear - i.screenPosition.w;
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);

                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

                float existinNormal = tex2Dproj(_CameraNormalsTexture,UNITY_PROJ_COORD(i.screenPosition));
                float3 normalDot = saturate(dot(existinNormal,i.viewNormal));

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
                float foamDepthDifference01 = saturate(depthDifference / foamDistance);

                float2 distortSample = (tex2D(_SurfaceDistortion, i.distordUV).xy * 2 - 1) * _SurfaceDistortionAmount;
                float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);

                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;                
                //float surfaceNoise = surfaceNoiseSample > surfaceNoiseCutoff ? 1 : 0;
                
                float aAIntensity = _AntiAliasingValue * SMOOTHSTEP_AA;

                float surfaceNoise = smoothstep(surfaceNoiseCutoff - aAIntensity, surfaceNoiseCutoff + aAIntensity, surfaceNoiseSample);
                float4 surfaceNoiseColor = _FoamColor;
                surfaceNoiseColor.a *= surfaceNoise;

				return alphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDCG
        }
    }
}