Shader "Unlit/Mask" {
    SubShader{
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry-100" }
        LOD 100
        Pass {
            ColorMask 0
        }
    }
}