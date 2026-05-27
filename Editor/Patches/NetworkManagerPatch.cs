#if ENABLE_BEPINEX
using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Nomnom.BepInEx.Editor.Patches {
    [InjectPatch]
    internal static class NetworkManagerPatch {
        
        public static bool Prepare() {
            return AccessTools.TypeByName("Unity.Netcode.NetworkManager") != null;
        }

        public static MethodBase TargetMethod() {
            var networkManagerType = AccessTools.TypeByName("Unity.Netcode.NetworkManager");
            if (networkManagerType == null) return null;

            var rpcFuncTableField = AccessTools.Field(networkManagerType, "__rpc_func_table");
            if (rpcFuncTableField == null) return null;

            return rpcFuncTableField.FieldType.GetMethod("Add");
        }

        public static bool Prefix(IDictionary __instance, object[] __args) {
            var key = __args[0];
            var value = __args[1];
            if (__instance.Contains(key)) {
                var existingValue = __instance[key];
                Debug.LogWarning($"RPC function with key \"{key}\" already exists with value \"{existingValue}\"!");
                return false;
            }

            return true;
        }
    }
}
#endif