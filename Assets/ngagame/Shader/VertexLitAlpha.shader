Shader "Custom/VertexLitAlpha"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			CGPROGRAM

			#pragma surface surf Lambert alpha vertex:vert

			float4 _Color;

			struct Input
			{
				float4 pos;
			};

			void vert(inout appdata_full v, out Input o)
			{
				o.pos = mul(unity_ObjectToWorld, v.vertex);
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color;
				o.Alpha = _Color.a;
			}

			ENDCG
		}
		Fallback "Transparent/VertexLit"
			
}