Shader "Unlit/Stretcher"
{
	Properties
	{
		// Color property for material inspector, default to white
		_MainTex("Texture", 2D) = "white" {}
		_TLTR("TopLeft - TopRight", vector) = (0,0,0,0)
		_BLBR("BottomLeft - BottomRight", vector) = (0,0,0,0)
	}
	
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _TLTR;
			float4 _BLBR;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float2 TL = float2(_TLTR.x, _TLTR.y);
				float2 TR = float2(_TLTR.z, _TLTR.w);
				float2 BL = float2(_BLBR.x, _BLBR.y);
				float2 BR = float2(_BLBR.z, _BLBR.w);

				// We get the the bottom x and top x position on the stretched rectangle.
				float2 bottomX = BL + ((BR - BL) * i.uv.x);
				float2 topX = TL + ((TR - TL) * i.uv.x);

				// We get the y position in between the bottom x and top x position, and we use it to get the corresponding uv coordinate.
				float2 uv = bottomX + ((topX - bottomX) * i.uv.y);
				return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
