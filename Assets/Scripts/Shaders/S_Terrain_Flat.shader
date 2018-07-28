// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/S_Flat"
{
	SubShader
	{

		Pass
		{	
			Tags{  "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma target 4.0

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

		

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			const static int maxColorCount = 8;
			const static int epsilon = 1E-4;	

			int baseColorCount;
			float3 baseColors[maxColorCount];
			float baseStartHeights[maxColorCount];
			float baseBlends[maxColorCount];

			float yOffset;
			float minHeight;
			float maxHeight;

			float randomness;
			sampler2D tex;

			float shininess;
			float3 color;
			float3 SpecularColor;
			int highlightType;

			float SpecularMultiplier;
			float ShadowDarkness;
			float3 ShadowColor;
			float ShadowColorMultiplier;

			float attenuation;

			float inverseLerp(float a, float b, float value) {
				return saturate((value - a) / (b - a));
			}

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			struct v {
				float4 vertex: TEXCOORD0;
			};

			struct v2g
			{
				float2 uv : TEXCOORD0;
				float3 vertex : TEXCOORD2;
				float4 pos : SV_POSITION;
				float4 col : COLOR0;
			};

			struct g2f {
				float4 pos : SV_POSITION;
				float4 col : COLOR0;
				float3 light : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float3 diffuseColor : TEXCOORD3;
				float3 specularColor : TEXCOORD4;
			};
			
			v2g vert (appdata_base v)
			{
				v2g o;
				o.uv = v.texcoord;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertex = v.vertex;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

				float heightPercent = inverseLerp(minHeight + yOffset, maxHeight + yOffset, worldPos.y);
				float3 col;
				for (int i = 0; i < baseColorCount; i++)
				{
					//float drawStrength = saturate(sign(heightPercent - baseStartHeights[i]));
					float drawStrengthWithBlend = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);
					col = col * (1 - drawStrengthWithBlend) + drawStrengthWithBlend * baseColors[i], 1;
				}
				o.col = float4(col, 1);

				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> OutputStream) {
				g2f o;

				//vertive position and center pos
				float3 v0 = input[0].vertex;
				float3 v1 = input[1].vertex;
				float3 v2 = input[2].vertex;
				float3 centerPos = (v0 + v1 + v2) / 3;

				//normal and view direction calculation
				float3 normal = cross(v1 - v0, v2 - v0);
				float3 normalDirection = normalize(mul(normal, (float3x3)unity_WorldToObject));
				float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(centerPos, 0.0)).xyz);

				//light dirs lighting 
				float3 lightdir = normalize(_WorldSpaceLightPos0.xyz);
				float light = max(0.0, dot(normalDirection, lightdir));

				//Shadow calc
				light = inverseLerp(ShadowDarkness, 1, light);
				float3 colLight = lerp(ShadowColor, float3(1, 1, 1), light);
				colLight *= pow(colLight, ShadowColorMultiplier);

				//diffuse lighting
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * color.rgb;
				float3 diffuseReflection = attenuation * _LightColor0.rgb * color.rgb * light;

				float hglType = clamp(highlightType, -1, 1);

				float3 specularReflection;
				if (light < 0) {
					specularReflection = float3(0, 0, 0);
				}
				else {
					float3 spec = pow(max(0.0, dot(reflect(-lightdir, normalDirection), viewDirection * hglType)), shininess);
					specularReflection = attenuation * SpecularColor * spec;
				}

				float random = rand(input[2].col) * randomness - 1E-10;
				//o.uv = (input[0].uv, input[1].uv, input[2].uv) /3;

				o.light = colLight;
				for (int i = 0; i < 3; i++)
				{
					o.diffuseColor = diffuseReflection + ambient;
					o.specularColor = specularReflection * SpecularMultiplier;
					o.col = saturate(input[2].col + (input[i].col * random));
					o.pos = input[i].pos;
					o.uv = input[i].uv;
					OutputStream.Append(o);
				}
			}
			
			fixed4 frag (g2f input) : SV_Target
			{
				float4 textureSample = tex2D(tex, input.uv);
				return float4(input.diffuseColor/* + input.specularColor*/, 1) * input.col *  float4(input.light, 1);
			}
			ENDCG
		}

		Pass
		{
			Tags{"LightMode" = "ForwardBase" "Queue"="Transparent" "RenderQueue"="Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fwdbase	

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			float4 recievedShadowColor;

			struct v2f {
				float4 pos : SV_POSITION;
				float4 vertex : TEXCOORD3;
				LIGHTING_COORDS(0,1)
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.vertex = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed atten = LIGHT_ATTENUATION(i);
				if (atten > 0.1)
					discard;
				return recievedShadowColor * UNITY_LIGHTMODEL_AMBIENT * _LightColor0 * float4(1,1,1,0.75);
			}
			ENDCG
		}
	}
}
