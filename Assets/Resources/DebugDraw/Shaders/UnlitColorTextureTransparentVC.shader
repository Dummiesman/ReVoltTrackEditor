Shader "Unlit/Texture Transparent Colored VC" {
    Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
    }

	SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off ZWrite Off

			Pass {

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = v.uv;
					return o;
				}

				uniform sampler2D _MainTex;
				fixed4 frag(v2f i) : SV_Target
				{
					return i.color * tex2D(_MainTex, float2(i.uv.x, i.uv.y));
				}
				ENDCG
		}
	}
}