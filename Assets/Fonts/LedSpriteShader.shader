Shader "Custom/LedSpriteShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_CubeWidth("LED number horizontal", Float) = 10
		_CubeHeight("LED number vertical", Float) = 10
		_CubeColor ("LED Color", Color) = (1,1,1,1)
		_Margin("Margin", Range(0, 0.25))=0.2
		_ColorCutoff("Color cutoff", Range(0, 1))=0.4
		_InvisibleCubeColor("Invisible LED light", Range(0, 1))=0.1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _CubeWidth;
			float _CubeHeight;
			float _ColorCutoff;
			fixed4 _CubeColor;
			float _InvisibleCubeColor;
			float _Margin;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, float2(ceil(IN.texcoord.x*_CubeWidth)/_CubeWidth, ceil(IN.texcoord.y*_CubeHeight)/_CubeHeight));
				half4 colorMask = step(0.025, abs((IN.texcoord.y*_CubeHeight+0.5)%1-0.5)*abs((IN.texcoord.x*_CubeWidth+0.5)%1-0.5)-_Margin);
				colorMask.a = 1;
				color.rgb = step(_ColorCutoff, color.r)+_InvisibleCubeColor;
				color.rgb = colorMask*color*_CubeColor;
				return color;
			}
		ENDCG
		}
	}
}