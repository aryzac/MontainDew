// ========== MassiveLODBuilder.cs ==========
// Ubicación: Assets/Scripts/MassiveLODBuilder.cs
// Este script se adjunta al GameObject raíz de tu prefab o escena.

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class MassiveLODBuilder : MonoBehaviour
{
    [Header("Calidad por LOD (0 = peor, 1 = original)")]
    [Range(0f, 1f)] public float calidadLOD0 = 1f;
    [Range(0f, 1f)] public float calidadLOD1 = 0.5f;
    [Range(0f, 1f)] public float calidadLOD2 = 0.2f;

    private const string carpetaDestino = "Assets/MeshesOptimized/";

    // Genera LODs en la escena (instancia)
    [ContextMenu("🔧 Generar LODs (Instancia)")]
    public void GenerarLODs()
    {
        Debug.Log("[LOD] Iniciando GenerarLODs() en instancia.");
        // Asegura carpeta destino
        if (!System.IO.Directory.Exists(carpetaDestino))
        {
            System.IO.Directory.CreateDirectory(carpetaDestino);
            Debug.Log($"[LOD] Carpeta creada: {carpetaDestino}");
        }

        // Lista de originales
        var originales = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>(false));
        Debug.Log($"[LOD] Encontrados {originales.Count} MeshRenderers originales.");

        // Limpiar clones previos
        foreach (Transform hijo in transform)
        {
            if (hijo.name.EndsWith("_LOD0") || hijo.name.EndsWith("_LOD1") || hijo.name.EndsWith("_LOD2"))
            {
                Debug.Log($"[LOD] Eliminando clon previo: {hijo.name}");
                DestroyImmediate(hijo.gameObject);
            }
        }

        // Listas por nivel
        var lod0 = new List<Renderer>();
        var lod1 = new List<Renderer>();
        var lod2 = new List<Renderer>();

        // Crear y optimizar clones
        foreach (var mr in originales)
        {
            var go = mr.gameObject;
            var mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.Log($"[LOD] Saltando {go.name}, sin MeshFilter o mesh.");
                continue;
            }

            // LOD0
            GameObject c0 = calidadLOD0 < 1f ? CloneAndOptimize(go, "_LOD0", calidadLOD0) : CloneSimple(go, "_LOD0");
            lod0.Add(c0.GetComponent<Renderer>());
            Debug.Log($"[LOD] Clon LOD0 creado: {c0.name}");

            // LOD1
            if (calidadLOD1 < 1f)
            {
                var c1 = CloneAndOptimize(go, "_LOD1", calidadLOD1);
                lod1.Add(c1.GetComponent<Renderer>());
                Debug.Log($"[LOD] Clon LOD1 creado: {c1.name}");
            }
            // LOD2
            if (calidadLOD2 < 1f)
            {
                var c2 = CloneAndOptimize(go, "_LOD2", calidadLOD2);
                lod2.Add(c2.GetComponent<Renderer>());
                Debug.Log($"[LOD] Clon LOD2 creado: {c2.name}");
            }

            // Desactivar original
            go.SetActive(false);
        }

        // Configurar o crear LODGroup
        LODGroup lg = GetComponent<LODGroup>();
        if (lg == null)
        {
            Debug.Log("[LOD] No se encontró LODGroup, agregando uno nuevo.");
            lg = gameObject.AddComponent<LODGroup>();
        }
        else Debug.Log("[LOD] LODGroup existente encontrado.");

        var lods = new List<LOD>();
        if (lod0.Count > 0) lods.Add(new LOD(0.6f, lod0.ToArray()));
        if (lod1.Count > 0) lods.Add(new LOD(0.3f, lod1.ToArray()));
        if (lod2.Count > 0) lods.Add(new LOD(0.1f, lod2.ToArray()));

        lg.SetLODs(lods.ToArray());
        lg.RecalculateBounds();
        Debug.Log($"[LOD] LODGroup configurado con {lods.Count} niveles.");
    }

    // Clon simple sin optimizar
    GameObject CloneSimple(GameObject original, string sufijo)
    {
        var clone = Instantiate(original, transform);
        clone.name = original.name + sufijo;
        clone.transform.localPosition = original.transform.localPosition;
        clone.transform.localRotation = original.transform.localRotation;
        clone.transform.localScale = original.transform.localScale;
        return clone;
    }

    // Clon y optimización via reflection a OptimizeMesh
    GameObject CloneAndOptimize(GameObject original, string sufijo, float calidad)
    {
        var clone = CloneSimple(original, sufijo);
        var mf = clone.GetComponent<MeshFilter>();
        var optimizer = clone.AddComponent<OptimizeMesh>();
        var t = optimizer.GetType();
        t.GetField("_quality", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, calidad);
        t.GetField("_renderer", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, mf);
        t.GetField("_mesh", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, mf.sharedMesh);
        t.GetMethod("DecimateMesh", BindingFlags.Instance | BindingFlags.Public)?.Invoke(optimizer, null);

        // Guardar mesh optimizado
        var meshCopy = Instantiate(mf.sharedMesh);
        var path = carpetaDestino + "optimized__" + clone.name + ".asset";
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(meshCopy, path);
        mf.sharedMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(path);
        Debug.Log($"[LOD] Mesh optimizado guardado en: {path}");
#endif

        return clone;
    }
}