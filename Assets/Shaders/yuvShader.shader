Shader "Custom/yuvShader" {
	Properties {
//		_MainTex("MainTexture",2D) = "white"{}
		_yTex("yTexture(Alpha8)",2D) = "white" {}
		_uvTex ("uvTexture (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		#pragma target 3.0

		sampler2D _yTex;
		sampler2D _uvTex;
//		sampler2D _MainT;


		struct Input {
			float2 uv_yTex;
			float2 uv_uvTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float3 uvColor = tex2D (_uvTex, IN.uv_yTex).rgb;
			float y = tex2D(_yTex,IN.uv_yTex).a;
			float u = uvColor.g - 0.5;
			float v = uvColor.r - 0.5;

			float r = y + 1.13983 * v;
			float g = y - 0.39465 * u - 0.58060 * v;
			float b = y + 2.03211 * u;

			o.Albedo = fixed3(r,g,b);
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
