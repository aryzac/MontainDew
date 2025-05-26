// ========== MassiveLODBuilderEditor.cs ==========
// Ubicación: Assets/Editor/MassiveLODBuilderEditor.cs
// Este script llama a GenerarLODs() directamente sobre el prefab asset

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class MassiveLODBuilderEditor
{
    const string MENU = "Assets/🔧 Generar LODs en Prefab";

    [MenuItem(MENU, true)]
    static bool ValidatePrefabLOD()
    {
        var obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj);
        return !string.IsNullOrEmpty(path) && path.EndsWith(".prefab");
    }

    [MenuItem(MENU)]
    static void GenerateLODForPrefabs()
    {
        foreach (var sel in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(sel);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab")) continue;

            // Carga prefab en memoria
            var root = PrefabUtility.LoadPrefabContents(path);
            var builder = root.GetComponent<MassiveLODBuilder>();
            if (builder == null)
                builder = root.AddComponent<MassiveLODBuilder>();

            // Llama a la misma función
            builder.GenerarLODs();

            // Guarda y descarga
            PrefabUtility.SaveAsPrefabAssetAndConnect(root, path, InteractionMode.UserAction);
            PrefabUtility.UnloadPrefabContents(root);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
