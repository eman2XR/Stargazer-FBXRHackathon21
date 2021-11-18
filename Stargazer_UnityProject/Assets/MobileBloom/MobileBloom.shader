Shader "RufatShaderlab/Bloom"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_BloomTex);
	half4 _MainTex_TexelSize;
	half4 _MainTex_ST;
	half4 _BloomTex_ST;
	half _BlurAmount;
	half4 _BloomColor;
	half4 _BloomData;

	static const half4 curve[2] = {
		half4(0.5,0.5,0.5,0),
		half4(0.0625, 0.0625,0.0625,0)
	};

	struct appdata {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2fb {
		half4 pos : POSITION;
		half4 uv : TEXCOORD0;
		half2 uv1 : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2f {
		half4 pos : POSITION;
#if UNITY_UV_STARTS_AT_TOP
		half4 uv : TEXCOORD0;
#else 
		half2 uv : TEXCOORD0;
#endif	
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2fb vertBlur(appdata i)
	{
		v2fb o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2fb, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv1 = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
		half2 offset = _MainTex_TexelSize * _BlurAmount * (1.0h / _MainTex_ST.xy);
		o.uv = half4(UnityStereoScreenSpaceUVAdjust(i.uv - offset, _MainTex_ST), UnityStereoScreenSpaceUVAdjust(i.uv + offset, _MainTex_ST));
		return o;
	}

	v2f vert(appdata i)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv.xy = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
#if UNITY_UV_STARTS_AT_TOP
		o.uv.zw = o.uv.xy;
		UNITY_BRANCH
		if (_MainTex_TexelSize.y < 0.0)
			o.uv.w = 1.0 - o.uv.w;
#endif
		return o;
	}

	half4 fragBloom(v2fb i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1);
		half br = max(c.r, max(c.g, c.b));
		half soft = clamp(br - _BloomData.y, 0.0h, _BloomData.z);
		half a = max(soft * soft * _BloomData.w, br - _BloomData.x) / max(br, 0.00001h);
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy);
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xw);
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zy);
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw);
		return c * a * 0.2h;
	}

	half4 fragBlur(v2fb i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv1) * curve[0];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xy) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.xw) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zy) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, half2(i.uv1.x, i.uv.y)) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, half2(i.uv.x, i.uv1.y)) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, half2(i.uv1.x, i.uv.w)) * curve[1];
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, half2(i.uv.z, i.uv1.y)) * curve[1];
		return c;
	}

	half4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
#if UNITY_UV_STARTS_AT_TOP
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv.zw);
#else
		half4 c = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
#endif
		c += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_BloomTex, i.uv.xy) * _BloomColor;
		return c;
	}
	ENDCG

	Subshader
	{
		Pass //0
		{
		  ZTest Always Cull Off ZWrite Off 
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBloom
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}

		Pass //1
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBlur
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}

		Pass //2
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
	}
	Fallback off
}