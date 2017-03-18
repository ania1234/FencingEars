﻿Shader "Unlit/NewTextShader"
{
	Properties {
		[PerRendererData] _MainTex ("Font Texture", 2D) = "white" {}
		_NewTex("Actual Font Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0.0, 1.0))=0.5
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		
		_ColorMask ("Color Mask", Float) = 15

		[MaterialToggle] _IsAnimating("Is animating", Float) = 0 

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
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
				float4 worldPosition : TEXCOORD1;
				float3 worldSpaceViewDir : NORMAL;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;


			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord;
				OUT.worldSpaceViewDir =  UnityObjectToViewPos(OUT.worldPosition);
				OUT.color = IN.color;// * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _NewTex;
			float _Cutoff;
			float _IsAnimating;
			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * _Color;
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				if(_IsAnimating>0){
					color.a*=(1-IN.color.r);
					color.a*=(step(_Cutoff, tex2D(_NewTex, IN.texcoord).r)*IN.color.g+IN.color.b);
				}
				else{
					color.a*=(step(_Cutoff, tex2D(_NewTex, IN.texcoord).r));
				}
				return color;
			}
		ENDCG
		}
	}
}
