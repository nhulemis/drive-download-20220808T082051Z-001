Shader "Custom/Kicker" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Speed("Speed", Range(0,10)) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 150

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		float _Speed;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex - float2(0, _Time.y * _Speed));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

		Fallback "Mobile/VertexLit"
}