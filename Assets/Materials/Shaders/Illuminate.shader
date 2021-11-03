// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Illuminate"
{
    Properties
    {
            _MainTex("Texture", 2D) = "white" {}
            _Diffuse("Diffuse",Color) = (1,1,1,1)
            _RimColor("RimColor",Color) = (1,1,1,1)
            _RimPower("RimPower",Range(0,1)) = 0

    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag 
                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Diffuse;
                float4 _RimColor;
                float _RimPower;

                struct v2f
                {
                    float4 vertex:SV_POSITION;
                    float2 uv:TEXCOORD0;
                    float3 worldLightNormal:TEXCOORD1;
                    float3 worldNormal:TEXCOORD2;
                    float3 worldViewNormal:TEXCOORD3;
                };

                v2f vert(appdata_base v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                    float3 worldPos = mul(unity_ObjectToWorld,v.vertex);
                    o.worldLightNormal = UnityWorldSpaceLightDir(worldPos);
                    o.worldViewNormal = UnityWorldSpaceViewDir(worldPos);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    return o;
                };

                fixed4 frag(v2f i) :SV_TARGET
                {
                    float3 worldNormalDir = normalize(i.worldNormal);
                    float3 WorldLightDir = normalize(i.worldLightNormal);
                    float3 WorldViewDir = normalize(i.worldViewNormal);
                    float3 texColor = tex2D(_MainTex,i.uv).rgb;
                    float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * texColor.rgb;
                    float3 diffuse = _LightColor0.rgb * _Diffuse.rgb * texColor.rgb * (dot(worldNormalDir,WorldLightDir) * 0.5 + 0.5);
                    float rim = 1 - saturate(dot(worldNormalDir,WorldViewDir));
                    float3 rimColor = _RimColor * pow(rim,1 / _RimPower);
                    float3 color = diffuse + ambient + rimColor;

                    return fixed4(color,1);
                };
                ENDCG
            }
        }
            Fallback "Diffuse"
}
