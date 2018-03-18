// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/RepeatingTerrainShader"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_WorldSize ("World Size", Range(0,513)) = 128
		_Debug ("Debug", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float3 normal : NORMAL;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			half _WorldSize;
			int _Debug;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				o.normal = v.normal;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			[maxvertexcount(3)]
            void geom(triangle v2f input[3], inout TriangleStream<v2f> OutputStream)
            {
                v2f v = (v2f)0;
                float offsetX;
                float offsetZ = 0.0f;

                // X-Axis
                if (_WorldSpaceCameraPos.x - input[0].vertex.x > _WorldSize / 2.0f ||
                    _WorldSpaceCameraPos.x - input[1].vertex.x > _WorldSize / 2.0f ||
                    _WorldSpaceCameraPos.x - input[2].vertex.x > _WorldSize / 2.0f)
                {
	                offsetX = _WorldSize;
	            } else if (_WorldSpaceCameraPos.x - input[0].vertex.x < -_WorldSize / 2.0f ||
	                       _WorldSpaceCameraPos.x - input[1].vertex.x < -_WorldSize / 2.0f ||
	                       _WorldSpaceCameraPos.x - input[2].vertex.x < -_WorldSize / 2.0f)
	            {
		            offsetX= -_WorldSize;
		        } else {
		            offsetX = 0.0f;
		        }

		        // Z-Axis
                if (_WorldSpaceCameraPos.z - input[0].vertex.z > _WorldSize / 2.0f ||
                    _WorldSpaceCameraPos.z - input[1].vertex.z > _WorldSize / 2.0f ||
                    _WorldSpaceCameraPos.z - input[2].vertex.z > _WorldSize / 2.0f)
                {
	                offsetZ = _WorldSize;
	            } else if (_WorldSpaceCameraPos.z - input[0].vertex.z < -_WorldSize / 2.0f ||
	                       _WorldSpaceCameraPos.z - input[1].vertex.z < -_WorldSize / 2.0f ||
	                       _WorldSpaceCameraPos.z - input[2].vertex.z < -_WorldSize / 2.0f)
	            {
		            offsetZ= -_WorldSize;
		        } else {
		            offsetZ = 0.0f;
		        }

	            for (int i = 0; i < 3; i++)
                {
                    v.normal = input[i].normal;
                    v.vertex = mul(UNITY_MATRIX_VP, float4(input[i].vertex.x + offsetX, input[i].vertex.y, input[i].vertex.z + offsetZ, input[i].vertex.w));
                    if (_Debug == 1) {
	                    if (offsetX == 0.0f && offsetZ == 0.0f) {
	                      v.color = input[i].color;
	                    } else if (offsetX != 0.0f && offsetZ == 0.0f) {
	                      v.color = float4(255, 255, 0, 255);
	                    } else if (offsetX == 0.0f && offsetZ != 0.0f) {
	                      v.color = float4(0, 255, 255, 255);
	                    } else {
	                      v.color = float4(255, 0, 255, 255);
	                    }
	                } else {
                      v.color = input[i].color;
                    }

                    OutputStream.Append(v);
                }

            }
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
