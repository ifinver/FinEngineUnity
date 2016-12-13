﻿Shader "my/yuv" {
	Properties {
		_yTexture("yTexture", 2D) = "white"{}
		_uvTexture("uvTexture", 2D) = "white"{}
	}
	SubShader {
		pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0
			#include "unitycg.cginc"

			uniform sampler2D _yTexture;
			uniform sampler2D _uvTexture;


			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXTCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f ret;
				ret.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				ret.uv = v.texcoord.xy;

				return ret;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				float4 col;
				float r, g, b, y, u, v;

				// get yData
				float4 yColor = tex2D(_yTexture, IN.uv);
				y = yColor.a;

				// get uvData
				float4 uvColor = tex2D(_uvTexture, IN.uv);
				u = uvColor.g - 0.5;
				v = uvColor.r - 0.5;

				r = y + 1.13983 * v;
				g = y - 0.39465 * u - 0.58060 * v;
				b = y + 2.03211 * u;

				//return uvColor;
				return fixed4(r, g, b, 1);
			}

			ENDCG
		}
	}
}
