Shader "Custom/UnlitFillAmount"
{
	Properties
	{
		_ColorTop("Color Top", Color) = (1,1,1,1)
		_ColorBottom("Color Bottom", Color) = (1,1,1,1)
		_FillAmount("Fill Amount", Range(-1,1)) = 0.0
	}

		SubShader
	{
		Tags {"Queue" = "Transparent"  "DisableBatching" = "True" }

		Pass
		{
		 Zwrite off
		 Cull Off // we want the front and back faces
		 AlphaToMask off // transparency
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
			float3 viewDir : COLOR;
			float3 normal : COLOR2;
			float fillEdge : TEXCOORD2;
		 };

		 float _FillAmount;
		 float4  _ColorTop;
		 float4  _ColorBottom;

		 v2f vert(appdata v)
		 {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			float3 worldPosAdjusted = mul(unity_ObjectToWorld, v.vertex.xyz);
			o.fillEdge = 1 - worldPosAdjusted.y - 0.5;
			return o;
		 }

		 fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
		 {
			 float top = step(i.fillEdge, _FillAmount);
			 return _ColorTop * top + _ColorBottom * (1-top);
		   }
		   ENDCG
		  }
	}
}