Shader "Custom/PixelBubbleShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveFrequency ("Wave Frequency", Float) = 2.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
        _PixelSize ("Pixel Size", Float) = 1.0   // ���ش�С�ɽű���̬����
        _TextureResolution ("Texture Resolution", Vector) = (32, 32, 0, 0) // ͼƬ�ֱ���
        _BubbleSize ("Bubble Size", Float) = 1.0
        _BreakThreshold ("Break Threshold", Float) = 2.0
        _BreakRange ("Break Range", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _PixelSize;
            float4 _TextureResolution; // x: ���, y: �߶�
            float _BubbleSize;
            float _BreakThreshold;
            float _BreakRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // ���㲨��Ч��
                float2 waveOffset = float2(
                    sin(_Time.y * _WaveSpeed + v.vertex.x * _WaveFrequency),
                    cos(_Time.y * _WaveSpeed + v.vertex.y * _WaveFrequency)
                ) * _WaveAmplitude;

                v.vertex.xy += waveOffset;

                // ���ݶ���λ�ú� UV
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ��ȡ��Ļ�ֱ��ʵ����� UV
                float2 screenUV = i.uv * _ScreenParams.xy;

                // ��̬���� Pixel Size����Ļ���ش�С / ����ֱ���
                float2 pixelSize = _ScreenParams.xy / _TextureResolution.xy;
                screenUV = floor(screenUV / pixelSize) * pixelSize;

                // ת�ع�һ�� UV
                float2 pixelAlignedUV = screenUV / _ScreenParams.xy;

                // ���������
                fixed4 color = tex2D(_MainTex, pixelAlignedUV);

                // ��������͸�����߼�
                float breakFactor = saturate((_BubbleSize - _BreakThreshold) / _BreakRange);
                float opacity = 1.0 - breakFactor;

                // ������ɫ��Ӧ��͸����
                return fixed4(color.rgb, color.a * opacity);
            }
            ENDHLSL
        }
    }

    FallBack "Transparent"
}
