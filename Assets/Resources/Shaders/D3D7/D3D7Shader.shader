/*
    Shader made to loosely mimic D3D7s materials
    Removed transparent pass because it's not needed in this program
*/
Shader "Custom/D3D7" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
        _AmbientColor("Ambient Color", Color) = (1,1,1,1)
        _SpecColor("Specular Color", Color) = (1,1,1,1)
        _EmissiveColor("Emissive Color", Color) = (0,0,0,0)
        _Shininess("Shininess", Range(0.03, 1)) = 1
        _AlphaClipThreshold("AlphaClipThreshold", Range(0.03, 1)) = 0.9
		_MainTex("Main Texture (RGB)", 2D) = "white" {}
	}

    SubShader
    {
        LOD 100

        Pass
        {
            //OPAQUE PASS
            Tags { "RenderType" = "Opaque" }
            Lighting On
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "MMLightModel.cginc"
            #include "MMCommon.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 lighting: TEXCOORD1;
                float3 normal: NORMAL;
                LIGHTING_COORDS(2, 3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AmbientColor;
            float _AlphaClipThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.lighting = MMLight8(_AmbientColor, COMPUTE_VIEW_NORMAL);
                TRANSFER_VERTEX_TO_FRAGMENT(o);

                return o;
            }

            float4 _Color;
            float4 _EmissiveColor;

            fixed4 frag(v2f i) : SV_Target
            {
                float attenuation = LIGHT_ATTENUATION(i);
                attenuation = max(attenuation, 0.5);

                float4 baseColor = tex2D(_MainTex, i.uv);
                float4 emisColor = _EmissiveColor * baseColor;

                float lR = i.lighting.r * (1.0 - _EmissiveColor.r);
                float lG = i.lighting.g * (1.0 - _EmissiveColor.g);
                float lB = i.lighting.b * (1.0 - _EmissiveColor.b);

                float3 lit = (baseColor * float3(lR, lG, lB) * _Color) + emisColor;
                fixed4 col = fixed4(lit, baseColor.a * _Color.a);

                MMPassOneClip(_AlphaClipThreshold, col);

                return col;
            }
            ENDCG
        }
	}
	
    Fallback "VertexLit"
}