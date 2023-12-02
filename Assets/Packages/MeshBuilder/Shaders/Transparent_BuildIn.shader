Shader "MeshBuilder/Transparent_Buildin"
{
    Properties
    {
        _Color ("颜色", COLOR) = (1,1,1,1)
        _Cutoff("透明剪切",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" //透明渲染模式
            "ForceNoShadowCasting" = "True" //无投影
            "IgnoreProjector" = "True"//无投影器
        }
        LOD 100

        Pass
        {
            NAME "FORWARD"
            Tags//tagas一定要有
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase_fullshadows//阴影必备
            #pragma target 3.0
            #include "UnityCG.cginc"

            float _Cutoff;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };


            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float opacity = _Color.a;
                clip(opacity - _Cutoff);
                return half4(_Color.rgb, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}