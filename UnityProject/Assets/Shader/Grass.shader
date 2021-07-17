Shader "Custom/Grass"
{
	Properties{
		_Color("Albedo", Color) = (1,1,1,1)
	}
	SubShader{
	Tags { "Queue" = "Transparent" }
	LOD 200

	CGPROGRAM
	#pragma surface surf Standard alpha:fade
	#pragma target 3.0

	struct Input {
		float3 worldNormal;
			float3 viewDir;
	};
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = _Color.rgb;
		float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
			o.Alpha = alpha * 1.5f;
	}
	ENDCG
	}
		FallBack "Diffuse"
}