Shader "Fourier/ShowSingleChanelColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		//ref: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
		//Each name will enable "property name" + underscore + "enum name", uppercased, shader keyword.
		[KeywordEnum(R, G, B)]_DrawChanel("Draw Chanel", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			//#pragma multi_compile_local _DRAWCHANEL_R, _DRAWCHANEL_G, _DRAWCHANEL_B
			// NOTE: uppercased key
			#pragma shader_feature _DRAWCHANEL_R _DRAWCHANEL_G _DRAWCHANEL_B

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 texCol = tex2D(_MainTex, i.uv);
				fixed4 col = fixed4(0.5, 0, 0, 1);

#ifdef _DRAWCHANEL_R
	col = fixed4(texCol.r, texCol.r, texCol.r, 1);
#endif

#ifdef _DRAWCHANEL_G
	col = fixed4(texCol.g, texCol.g, texCol.g, 1);
#endif

#ifdef _DRAWCHANEL_B
	col = fixed4(texCol.b, texCol.b, texCol.b, 1);
#endif

                return col;
            }
            ENDCG
        }
    }
}
