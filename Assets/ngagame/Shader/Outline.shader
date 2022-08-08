Shader "Custom/Outline"
{
	Properties
	{
		_Colour("Colour", Color) = (1,1,1,1)
		_FillAmount("Fill Amount", Range(0,1)) = 0.0
	}

		SubShader
	{
		Tags {"Queue" = "Transparent"  "DisableBatching" = "True" }

		Pass
		{
		 Zwrite off
		 Cull Back
		Offset 0, 50
		 AlphaToMask off
		 Blend SrcAlpha OneMinusSrcAlpha
		 CGPROGRAM
		 #pragma vertex vert
		 #pragma fragment frag
		 #include "UnityCG.cginc"

		 struct appdata
		 {
		   float4 vertex : POSITION;
		   float3 normal : NORMAL;
		 };

		 struct v2f
		 {
			float4 vertex : SV_POSITION;
		 };

		 float _FillAmount;
		 float4  _Colour;

		 v2f vert(appdata v)
		 {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _FillAmount);
			return o;
		 }

		 fixed4 frag(v2f i) : SV_Target
		 {
			 return _Colour;
		   }
		   ENDCG
		  }
	}
}