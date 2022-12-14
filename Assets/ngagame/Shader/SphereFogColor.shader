Shader "Custom/SphereFogColor"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
		_FogMaxHeight("Fog Max Radius", Float) = 0.0
		_FogMinHeight("Fog Min Radius", Float) = -1.0
	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			ZWrite Off
			CGPROGRAM

			#pragma surface surf Lambert finalcolor:finalcolor vertex:vert

			float4 _Color;
			float4 _FogColor;
			float _FogMaxHeight;
			float _FogMinHeight;

			struct Input
			{
				float4 pos;
				float dis;
			};

			void vert(inout appdata_full v, out Input o)
			{
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				o.dis = length(ObjSpaceViewDir(v.vertex));
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color;
			}

			void finalcolor(Input IN, SurfaceOutput o, inout fixed4 color)
			{
				#ifndef UNITY_PASS_FORWARDADD
				float lerpValue = clamp((IN.dis - _FogMinHeight) / (_FogMaxHeight - _FogMinHeight), 0, 1);
				color.rgb = lerp(color.rgb, _FogColor, lerpValue);
				#endif
			}

			ENDCG
		}
		Fallback "Transparent/VertexLit"
			
}