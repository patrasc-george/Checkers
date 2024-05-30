Shader "Custom/BlinnPhong" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecIntensity ("Specular Intensity", Range(0, 1)) = 0.5
        _Glossiness ("Glossiness", Range(0, 1)) = 1
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf BlinnPhong

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;
        float4 _SpecularColor;
        float _SpecIntensity;
        float _Glossiness;

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            
            // Calculate the specular color
            float3 specColor = _SpecularColor.rgb * _SpecIntensity;

            // Calculate the specular intensity
            float specIntensity = max(0, dot(normalize(UnityWorldSpaceViewDir(IN.worldPos)), o.Normal));
            specIntensity = pow(specIntensity, _Glossiness);

            // Blend the specular color with the pixel color
            float3 finalColor = texColor.rgb + specColor * specIntensity;

            o.Albedo = finalColor;
            o.Specular = 0; // Set to 0 to avoid affecting pixel color
            o.Gloss = _Glossiness;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
