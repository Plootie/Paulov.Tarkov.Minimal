﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Paulov.Bepinex.Framework;
using Paulov.Bepinex.Framework.Patches;
using Paulov.Tarkov.Minimal.Models;
using UnityEngine.Networking;

namespace Paulov.Tarkov.Minimal.Patches
{
    public sealed class UnityWebRequestTexturePatch : NullPaulovHarmonyPatch
    {
        public override IEnumerable<MethodBase> GetMethodsToPatch()
        {
            const string methodName = nameof(UnityWebRequestTexture.GetTexture);
            
            Type classType = typeof(UnityWebRequestTexture);
            MethodInfo method = classType.GetMethod(methodName, [typeof(string)]);

            if(method is null) throw new MissingMethodException(classType.FullName, methodName);
            
            Plugin.Logger.LogDebug($"{nameof(UnityWebRequestTexturePatch)}.{nameof(GetMethodsToPatch)}:{method.Name}");

            yield return method;
        }
        
        public override HarmonyMethod GetPostfixMethod()
        {
            return new HarmonyMethod(this.GetType().GetMethod(nameof(PostfixOverrideMethod), BindingFlags.Public | BindingFlags.Static));
        }

        public static void PostfixOverrideMethod(UnityWebRequest __result)
        {
            __result.certificateHandler = new FakeCertificateHandler();
            __result.disposeCertificateHandlerOnDispose = true;
            __result.timeout = 15000;
        }
    }
}
