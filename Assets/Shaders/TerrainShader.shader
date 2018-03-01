Shader "Custom/TerrainShader" {
	Properties {
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_WorldSize ("World Size", Range(0,513)) = 5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			//float2 uv_MainTex;
			float4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		half _WorldSize;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

	   // Vertex modifier function
       void vert (inout appdata_full v) {
           half3 object = mul(unity_ObjectToWorld, v.vertex);
           if (_WorldSpaceCameraPos.x - object.x > _WorldSize / 2.0f) {
             v.vertex.x += _WorldSize;
             v.color = fixed4(0, 255, 255, 255);
           } else if (_WorldSpaceCameraPos.x - object.x < -_WorldSize / 2.0f) {
             v.vertex.x -= _WorldSize;
             v.color = fixed4(0, 255, 255, 255);
           }

//           if (_WorldSpaceCameraPos.z - object.z > 0.6f * _WorldSize) {
//             v.vertex.z += _WorldSize;
//           } else if (_WorldSpaceCameraPos.z - object.z < 0.8f * -_WorldSize) {
//             v.vertex.z -= _WorldSize;
//           }
       }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from vertex
			fixed4 c = IN.color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
