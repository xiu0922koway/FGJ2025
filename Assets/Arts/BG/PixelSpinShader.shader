Shader "Custom/PixelSpinWithHDRGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 主纹理
        _PixelFilter ("Pixel Filter", Float) = 745.0
        _SpinRotation ("Spin Rotation", Float) = -2.0
        _SpinSpeed ("Spin Speed", Float) = 7.0
        _SpinAmount ("Spin Amount", Float) = 0.25
        _SpinEase ("Spin Ease", Float) = 1.0
        _Contrast ("Contrast", Float) = 3.5
        _Lighting ("Lighting", Float) = 0.4
        _GlowColor ("Glow Color", Color) = (1, 1, 0, 1) // 边缘发光颜色
        _GlowIntensity ("Glow Intensity", Float) = 1.0 // 边缘发光强度
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1) // 背景颜色
        _EmissionColor ("Emission Color", Color) = (1, 0.5, 0.0, 1) // 自发光颜色 (HDR)
        _EmissionIntensity ("Emission Intensity", Float) = 1.0 // 自发光强度 (HDR)
        _Color1 ("Color 1", Color) = (0.871, 0.267, 0.231, 1.0)
        _Color2 ("Color 2", Color) = (0.0, 0.42, 0.706, 1.0)
        _Color3 ("Color 3", Color) = (0.086, 0.137, 0.145, 1.0)
        _IsRotate ("Is Rotate", Int) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _PixelFilter;
            float _SpinRotation;
            float _SpinSpeed;
            float _SpinAmount;
            float _SpinEase;
            float _Contrast;
            float _Lighting;
            float4 _GlowColor;
            float _GlowIntensity;
            float4 _BackgroundColor;  // 背景颜色 (支持 HDR)
            float4 _EmissionColor;    // 自发光颜色 (支持 HDR)
            float _EmissionIntensity; // 自发光强度
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            int _IsRotate;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 屏幕分辨率和 UV
                float2 screenSize = _ScreenParams.xy; // 获取屏幕宽度和高度
                float2 screenCoords = i.uv * screenSize;

                // 像素大小
                float pixelSize = length(screenSize) / _PixelFilter;
                float2 uv = (floor(screenCoords * (1.0 / pixelSize)) * pixelSize - 0.5 * screenSize) / length(screenSize);

                // UV 偏移
                float uvLen = length(uv);
                float speed = (_SpinRotation * _SpinEase * 0.2);

                if (_IsRotate == 1) // 是否旋转
                {
                    speed = _Time.y * speed;
                }
                speed += 302.2;

                // 新像素角度
                float newPixelAngle = atan2(uv.y, uv.x) + speed - _SpinEase * 20.0 * (1.0 * _SpinAmount * uvLen + (1.0 - 1.0 * _SpinAmount));
                float2 mid = (screenSize / length(screenSize)) / 2.0;
                uv = float2(
                    uvLen * cos(newPixelAngle) + mid.x,
                    uvLen * sin(newPixelAngle) + mid.y
                ) - mid;

                // 复杂 UV 变换
                uv *= 30.0;
                speed = _Time.y * _SpinSpeed;
                float2 uv2 = uv.x + uv.y;

                for (int j = 0; j < 5; j++)
                {
                    uv2 += sin(max(uv.x, uv.y)) + uv;
                    uv += 0.5 * float2(cos(5.1123314 + 0.353 * uv2.y + speed * 0.131121), sin(uv2.x - 0.113 * speed));
                    uv -= 1.0 * cos(uv.x + uv.y) - 1.0 * sin(uv.x * 0.711 - uv.y);
                }

                // 对比度和颜色计算
                float contrastMod = (0.25 * _Contrast + 0.5 * _SpinAmount + 1.2);
                float paintRes = min(2.0, max(0.0, length(uv) * (0.035) * contrastMod));
                float c1p = max(0.0, 1.0 - contrastMod * abs(1.0 - paintRes));
                float c2p = max(0.0, 1.0 - contrastMod * abs(paintRes));
                float c3p = 1.0 - min(1.0, c1p + c2p);
                float light = (_Lighting - 0.2) * max(c1p * 5.0 - 4.0, 0.0) + _Lighting * max(c2p * 5.0 - 4.0, 0.0);

                // 原始颜色计算
                fixed4 color = (0.3 / _Contrast) * _Color1
                    + (1.0 - 0.3 / _Contrast) * (_Color1 * c1p + _Color2 * c2p + float4(c3p * _Color3.rgb, c3p * _Color1.a))
                    + light;

                // 自发光颜色
                fixed4 emission = _EmissionColor * _EmissionIntensity;

                // 边缘发光计算
                float alpha = color.a; // 使用 Alpha 通道检测边缘
                float edge = fwidth(alpha); // 计算边缘梯度
                float glow = smoothstep(0.1, 0.2, edge) * _GlowIntensity; // 发光强度控制
                fixed4 glowColor = _GlowColor * glow;

                // 背景颜色和发光效果叠加
                fixed4 finalColor = lerp(_BackgroundColor, color + glowColor + emission, color.a);

                return finalColor;
            }
            ENDCG
        }
    }
}
