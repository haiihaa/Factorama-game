// References:
// https://alastaira.wordpress.com/2014/12/30/adding-shadows-to-a-unity-vertexfragment-shader-in-7-easy-steps/
// https://www.youtube.com/watch?v=4XfXOEDzBx4&t=2113s
// https://lindenreidblog.com/2017/12/16/dissolve-shader-in-unity/


Shader "Custom/vert"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_DissolveSpeed("Dissolve Speed", float) = 1
		_TimeOfDissolve("Time of Dissolve", float) = 0
		_ColorDissolveSpeed("Color Dissolve Speed", float) = 2
		_DissolveColor("Dissolve Color", Color) = (1, 1, 1, 1)
	}

		SubShader
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha
				Cull Off

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase // Shadows

				#include "UnityCG.cginc"
				#include "Lighting.cginc" // Lighting
				#include "AutoLight.cginc" // Shadows

				// Properties
				float4 _Color;
				float4 _DissolveColor;
				sampler2D _NoiseTex;
				float _DissolveSpeed;
				float _ColorDissolveSpeed;
				float _TimeOfDissolve;
				sampler2D _MainTex;
                float4 _MainTex_ST;

				struct vertexInput
				{
					float4 vertex : POSITION;
					float4 texCoord : TEXCOORD0;
					float4 normal : NORMAL;
					
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 texCoord : TEXCOORD0;
					float3 worldLightNormal:TEXCOORD1;
					float3 worldNormal:TEXCOORD2;
					float3 worldViewNormal:TEXCOORD3;
					LIGHTING_COORDS(4,5)
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					// convert to world space
					output.pos = UnityObjectToClipPos(input.vertex);

					// texture coordinates 
					output.texCoord = input.texCoord;

					// For Lighting
					float3 worldPos = mul(unity_ObjectToWorld,input.vertex);
					output.worldLightNormal = UnityWorldSpaceLightDir(worldPos);
					output.worldViewNormal = UnityWorldSpaceViewDir(worldPos);
					output.worldNormal = UnityObjectToWorldNormal(input.normal);

					// For Shadows
					TRANSFER_VERTEX_TO_FRAGMENT(output);

					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					// Lighting
                    float3 worldNormalDir = normalize(input.worldNormal);
                    float3 WorldLightDir = normalize(input.worldLightNormal);
                    float3 WorldViewDir = normalize(input.worldViewNormal);
                    float3 texColor = tex2D(_MainTex,input.pos).rgb;
                    float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * texColor.rgb;
                    float3 diffuse = _LightColor0.rgb * _Color.rgb * texColor.rgb * (dot(worldNormalDir,WorldLightDir) * 0.5 + 0.5);
                    float rim = 1 - saturate(dot(worldNormalDir,WorldViewDir));

					// Shadows
					float shadows = LIGHT_ATTENUATION(input);

					// Color
                    float4 color = fixed4((diffuse + ambient) * shadows, 1);

					// Dissolve Effect
					// sample noise texture
					float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));

					// determine deletion thresehold
					float threshold = (_Time.y - _TimeOfDissolve) * _DissolveSpeed;

					// Dissolve Color
					float dissolveThreshold = (_Time.y - _TimeOfDissolve) * _ColorDissolveSpeed;
					float useDissolve = noiseSample - dissolveThreshold < 0;
					color = (1-useDissolve)*color + useDissolve*_DissolveColor;

					// 'clip'
					clip(noiseSample - threshold);
					
					return color;
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}

