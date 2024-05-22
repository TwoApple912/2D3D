﻿
Shader "Hidden/BlendModes/UIDefaultFont/Overlay" {
    Properties {
        [PerRendererData] _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _BLENDMODES_OverlayTexture("Overlay Texture", 2D) = "white" {}
        _BLENDMODES_OverlayColor("Overlay Color", Color) = (1,1,1,1)
    }

    FallBack "Hidden/BlendModes/UIDefault/Overlay"
}