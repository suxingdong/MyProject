//$-----------------------------------------------------------------------------
//@ Lighted Shader		- The fastest textured shader of this group.
//@
//# LIGHT PROBES        - YES
//# SHADOWS             - YES
//# BEAST LIGHTMAPPING  - NO
//# IGNORE PROJECTOR    - NO
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoScale          - Scales Shader/Mesh 
//#  
//&-----------------------------------------------------------------------------
//sunny add additive effect(Particle/Additive)
Shader "echoLogin/Light/10-Fastest-Additive2"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "white" {} 
    	_MainTex1 ("Texture", 2D)				= "white" {} 
    	_TintColor ("TintColor", Color) = (1.0, 1.0, 1.0, 1.0)
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
   	}
   	
	//=========================================================================
	SubShader 
	{
		
		Pass 
		{    
			Name "BASE"
			Tags { "Queue" = "Geometry" "IgnoreProjector"="False" "RenderType"="echoLight" "LightMode" = "ForwardBase" }
 
      		Cull Back
     		
     		CGPROGRAM

			#define DIRECTIONAL
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma exclude_renderers flash
			#pragma multi_compile SHADOWS_SCREEN SHADOWS_OFF
#if defined (ECHO_ADDBEAST_CODE)
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
#endif

			#include "echologin_shaderoptions.cginc"
								
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
#ifndef SHADOWS_OFF			  	
			#include "AutoLight.cginc"
#endif
			#include "../Include/EchoLogin.cginc"
			#include "../Include/EchoLogin-Light.cginc"

			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;

						
			// Vertex\Frag Shader     =====================
			#define ECHODEF_USEFRAG 	
			#define ECHODEF_DEFSTRUCTS
			#include "echologin_LitVertexShader.cginc"

			ENDCG
		}	

		Pass {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One One

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		#include "UnityCG.cginc"
		
		
		sampler2D _MainTex1;
		fixed4 _TintColor;
		half4 _MainTex1_ST;
						
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 vertexColor : COLOR;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex1);
			o.vertexColor = v.color * _TintColor;
					
			return o; 
		}
		
		fixed4 frag( v2f i ) : COLOR {
			return  tex2D (_MainTex1, i.uv.xy) * i.vertexColor;
			
		}
		
		ENDCG
		 
		}

	}
	
	Fallback "echoLogin/Light/Solid/Color"
}
 
