Shader "Custom/AltitudeFogColor"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
		_FogMaxHeight("Fog Max Height", Float) = 0.0
		_FogMinHeight("Fog Min Height", Float) = -1.0
	}

		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			Cull Back
			ZWrite On

			CGPROGRAM

			#pragma surface surf Lambert finalcolor:finalcolor vertex:vert

			float4 _Color;
			float4 _FogColor;
			float _FogMaxHeight;
			float _FogMinHeight;

			struct Input
			{
				float4 pos;
			};

			void vert(inout appdata_full v, out Input o)
			{
				float4 hpos = UnityObjectToClipPos(v.vertex);
				o.pos = mul(unity_ObjectToWorld, v.vertex);
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color;
			}

			void finalcolor(Input IN, SurfaceOutput o, inout fixed4 color)
			{
				#ifndef UNITY_PASS_FORWARDADD
				float lerpValue = clamp((IN.pos.y - _FogMinHeight) / (_FogMaxHeight - _FogMinHeight), 0, 1);
				color.rgb = lerp(_FogColor.rgb, color.rgb, lerpValue);
				#endif
			}

			ENDCG
		}

			FallBack "Diffuse"
}