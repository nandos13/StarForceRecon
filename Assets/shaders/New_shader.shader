// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32719,y:32712,varname:node_2865,prsc:2|emission-853-OUT;n:type:ShaderForge.SFN_Tex2d,id:8110,x:32067,y:32991,ptovrint:False,ptlb:texture,ptin:_texture,varname:node_8110,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b961c7af343d34c4a912a4d209589e79,ntxv:0,isnm:False|UVIN-9918-OUT;n:type:ShaderForge.SFN_Slider,id:2600,x:31899,y:32846,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_2600,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.1,max:100;n:type:ShaderForge.SFN_Multiply,id:853,x:32305,y:32972,varname:node_853,prsc:2|A-2600-OUT,B-8110-RGB;n:type:ShaderForge.SFN_TexCoord,id:908,x:31306,y:33195,varname:node_908,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7249,x:31523,y:33195,varname:node_7249,prsc:2,spu:0.2,spv:0.2|UVIN-908-UVOUT;n:type:ShaderForge.SFN_Sin,id:8120,x:31500,y:32860,varname:node_8120,prsc:2|IN-3175-OUT;n:type:ShaderForge.SFN_Time,id:8433,x:31176,y:32912,varname:node_8433,prsc:2;n:type:ShaderForge.SFN_Vector1,id:7206,x:31176,y:32860,varname:node_7206,prsc:2,v1:2.5;n:type:ShaderForge.SFN_Multiply,id:3175,x:31326,y:32860,varname:node_3175,prsc:2|A-7206-OUT,B-8433-T;n:type:ShaderForge.SFN_Multiply,id:9918,x:31853,y:32991,varname:node_9918,prsc:2|A-4469-OUT,B-7249-UVOUT;n:type:ShaderForge.SFN_Vector1,id:8936,x:31168,y:32637,varname:node_8936,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:3713,x:31338,y:32637,varname:node_3713,prsc:2|A-8936-OUT,B-286-T;n:type:ShaderForge.SFN_Time,id:286,x:31168,y:32690,varname:node_286,prsc:2;n:type:ShaderForge.SFN_Sin,id:312,x:31496,y:32637,varname:node_312,prsc:2|IN-3713-OUT;n:type:ShaderForge.SFN_Multiply,id:4469,x:31690,y:32841,varname:node_4469,prsc:2|A-312-OUT,B-8120-OUT;proporder:8110-2600;pass:END;sub:END;*/

Shader "hologram_test" {
    Properties {
        _texture ("texture", 2D) = "white" {}
        _brightness ("brightness", Range(0, 100)) = 1.1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float4 node_286 = _Time;
                float4 node_8433 = _Time;
                float4 node_8226 = _Time;
                float2 node_7249 = (i.uv0+node_8226.g*float2(0.2,0.2));
                float2 node_9918 = ((sin((1.0*node_286.g))*sin((2.5*node_8433.g)))*node_7249);
                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(node_9918, _texture));
                float3 emissive = (_brightness*_texture_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_286 = _Time;
                float4 node_8433 = _Time;
                float4 node_9727 = _Time;
                float2 node_7249 = (i.uv0+node_9727.g*float2(0.2,0.2));
                float2 node_9918 = ((sin((1.0*node_286.g))*sin((2.5*node_8433.g)))*node_7249);
                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(node_9918, _texture));
                o.Emission = (_brightness*_texture_var.rgb);
                
                float3 diffColor = float3(0,0,0);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0, specColor, specularMonochrome );
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
