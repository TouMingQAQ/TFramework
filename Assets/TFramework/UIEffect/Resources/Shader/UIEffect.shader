Shader "TFramework/UIEffect"
{
    Properties
    {
        //Begin Properties
        _MainTex("Texture", 2D) = "white" {}
        
        
        _MoveSpeed("Move Speed", Float) = 0.5
        _MoveDirection("Move Direction", Vector) = (1, 0, 0, 0)

        _ShinyTex("Shiny Texture", 2D) = "white" {}
        _ShinyRotate("Shiny Rotate", Float) = 0
        _ShinySpeed("Shiny Speed", Float) = 0.5
        _ShinyDirection("Shiny Direction", Vector) = (1, 0, 0, 0)
        _ShinyDurationTime("Duration Time", float) = 0
        [HDR]_ShinyColor("Shiny Color", Color) = (1, 1, 1, 1)


        _ClipArea("Clip Area", Vector) = (0,0 , 0, 0)


        [Toggle(UVFLOW_ON)] _IsUVFlow("UV Flow", Float) = 0
        [Toggle(SHINY_ON)] _IsShiny("Shiny", Float) = 0
        [Toggle(CLIPAREA_ON)] _IsClipArea("Clip Area", Float) = 0
    }

    SubShader
    {

        Tags
        {
            "RenderType"="Opaque" "RenderQueue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma shader_feature UVFLOW_ON
            #pragma shader_feature SHINY_ON
            #pragma shader_feature CLIPAREA_ON

            #if LIGHT_ON

            #endif

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            #if UVFLOW_ON
            float _MoveSpeed;
            float4 _MoveDirection;
            #endif

            #if SHINY_ON
            sampler2D _ShinyTex;
            float4 _ShinyTex_ST;
            float _ShinyRotate;
            float _ShinySpeed;
            fixed4 _ShinyColor;
            float4 _ShinyDirection;
            float _ShinyDurationTime;
            #endif

            #if CLIPAREA_ON
            float4 _ClipArea;
            #endif


            //将二维顶点point2，沿着圆心center，顺时针旋转radian弧度
            float2 RotatePoint2(float2 point2, float2 center, half radian)
            {
                float2 dir = float2(0, 0) - center;
                point2 += dir;

                float2 newP;
                float c = cos(radian);
                float s = sin(radian);
                newP.x = point2.x * c - point2.y * s;
                newP.y = point2.x * s + point2.y * c;

                newP += (dir * -1);
                return newP;
            }

            v2f vert(appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                #if UVFLOW_ON
                float2 move = normalize(_MoveDirection.xy) * _Time.y * _MoveSpeed;
                o.uv.xy += move;
                #endif


                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                #if SHINY_ON
                half2 uv = i.uv;
                uv = RotatePoint2(uv, float2(0.5, 0.5), _ShinyRotate);
                float t = _Time.y;
                uv.xy *= _ShinyTex_ST.xy;
                uv.xy += t * _ShinySpeed * _ShinyDirection.xy;
                fixed shinyalpha = tex2D(_ShinyTex, uv).a;

                //拉长空白时间
                _ShinyDurationTime = max(1, _ShinyDurationTime);
                float n = abs(uv.x) % _ShinyDurationTime;
                shinyalpha = step(n, 1) * shinyalpha;

                col.rgb = lerp(col.rgb, _ShinyColor.rgb, shinyalpha * _ShinyColor.a);
                #endif

                #if CLIPAREA_ON
                if (i.uv.x > _ClipArea.x && i.uv.x < _ClipArea.z && i.uv.y > _ClipArea.y && i.uv.y < _ClipArea.w)
                {
                    clip(-1); // Discard the pixel
                }
                #endif


                return col;
            }
            ENDHLSL
        }
    }
    CustomEditor "TFramework.UIEffect.UIEffectEditor"
}