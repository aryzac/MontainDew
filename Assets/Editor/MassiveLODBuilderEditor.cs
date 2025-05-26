// ========== MassiveLODBuilderEditor.cs ==========
// Ubicación: Assets/Editor/MassiveLODBuilderEditor.cs

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class MassiveLODBuilderEditor
{
    const string MENU = "Assets/⚙ Generar LODs en Prefab";

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
        Debug.Log("[LOD▶Prefab] Invocado");
        foreach (var sel in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(sel);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab"))
            {
                Debug.LogWarning($"[LOD▶Prefab] Omitido no-prefab: {path}");
                continue;
            }

            Debug.Log($"[LOD▶Prefab] Cargando prefab: {path}");
            var root = PrefabUtility.LoadPrefabContents(path);
            var builder = root.GetComponent<MassiveLODBuilder>() ?? root.AddComponent<MassiveLODBuilder>();
            builder.GenerarLODs();  // ahora accesible públicamente

            Debug.Log($"[LOD▶Prefab] Guardando prefab asset: {path}");
            PrefabUtility.SaveAsPrefabAsset(root, path);

            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log($"[LOD▶Prefab] Finalizado prefab: {path}");
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[LOD▶Prefab] Todos completados.");
    }
}
#endif
