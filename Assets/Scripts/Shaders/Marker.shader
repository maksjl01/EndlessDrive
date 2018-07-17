Shader "Custom/Marker"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RingNum("Number Of Rings", int) = 2
		_RingThickness("Thickness of Rings", float) = 0.1
		_RingDistance("Distance between Rings", float) = 0.1
		_RingCol("Ring Color", Color) = (0,0,0,0)
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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _UVX;
			float _UVY;

			int _RingNum;
			float _RingThickness;
			float _RingDistance;
			float4 _RingCol;

			float cur = 1;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float4 Col;



				float2 c = float2(_UVX, _UVY);
				float d = distance(i.uv.xy, c);

				Col = float4(0.8, 0.8, 0.8, 1);

				for (int i = 0; i < _RingNum; i++)
				{
					if (d > i * _RingThickness + (step(1, i) * _RingDistance) && d < _RingThickness * (i+1)) {
						Col = _RingCol;
					}
				}
				return Col;
			}
			ENDCG
		}
	}
}
