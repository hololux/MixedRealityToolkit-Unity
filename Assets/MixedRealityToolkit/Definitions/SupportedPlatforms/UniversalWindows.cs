﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class UniversalWindows : IPlatformSupport
{
    public bool IsEditorOrRuntimePlatform()
    {
#if UNITY_EDITOR
        var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        return activeBuildTarget == BuildTarget.XboxOne || activeBuildTarget == BuildTarget.WSAPlayer;
#else
        return Application.platform == RuntimePlatform.XboxOne || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64;
#endif
    }
    public bool IsCurrentRuntimePlatformSameAs(RuntimePlatform runtimePlatform)
    {
        return (runtimePlatform == RuntimePlatform.XboxOne || runtimePlatform == RuntimePlatform.WSAPlayerARM || runtimePlatform == RuntimePlatform.WSAPlayerX86 || runtimePlatform == RuntimePlatform.WSAPlayerX64) && IsEditorOrRuntimePlatform();
    }
}