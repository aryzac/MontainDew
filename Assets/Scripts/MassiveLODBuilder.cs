// ========== MassiveLODBuilder.cs ==========
// Ubicación: Assets/Scripts/MassiveLODBuilder.cs
// Script modificado: no desactiva root, agrega chequeo de LODGroup

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class MassiveLODBuilder : MonoBehaviour
{
    [Header("Calidad por LOD (0 = peor, 1 = original)")]
    [Range(0f, 1f)] public float calidadLOD0 = 1.0f;
    [Range(0f, 1f)] public float calidadLOD1 = 0.5f;
    [Range(0f, 1f)] public float calidadLOD2 = 0.2f;

    private const string carpetaDestino = "Assets/MeshesOptimized/";

    [ContextMenu("⚙ Generar LODs (Instancia en Escena)")]
    public void GenerarLODs()
    {
        // Asegura carpeta destino
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(carpetaDestino.TrimEnd('/')))
        {
            Directory.CreateDirectory(carpetaDestino);
            AssetDatabase.Refresh();
            Debug.Log($"[LOD] Carpeta creada: {carpetaDestino}");
        }
#endif

        Transform padre = transform;

        // Listas de renderers por nivel
        var lod0List = new List<Renderer>();
        var lod1List = new List<Renderer>();
        var lod2List = new List<Renderer>();

        // Chequeo o creación de LODGroup en el root
        LODGroup lg = GetComponent<LODGroup>();
        if (lg == null)
        {
            Debug.Log("[LOD] No se encontró LODGroup en root, agregando uno nuevo.");
            lg = gameObject.AddComponent<LODGroup>();
        }

        // Recorrer cada pieza original
        var renderers = padre.GetComponentsInChildren<MeshRenderer>(includeInactive: false);
        foreach (var r in renderers)
        {
            var go = r.gameObject;
            var mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogWarning($"[LOD] Pieza sin MeshFilter o mesh: {go.name}");
                continue;
            }

            // LOD0: reutiliza original si calidad=1, sino clona y desactiva original
            if (calidadLOD0 >= 1f)
            {
                lod0List.Add(go.GetComponent<Renderer>());
                // dejar original activo para LOD0
            }
            else
            {
                go.SetActive(false);
                var c0 = CloneAndOptimize(go, "_LOD0", padre, calidadLOD0);
                lod0List.Add(c0.GetComponent<Renderer>());
            }

            // LOD1: sólo si calidad<1
            if (calidadLOD1 < 1f)
            {
                var c1 = CloneAndOptimize(go, "_LOD1", padre, calidadLOD1);
                lod1List.Add(c1.GetComponent<Renderer>());
            }

            // LOD2: sólo si calidad<1
            if (calidadLOD2 < 1f)
            {
                var c2 = CloneAndOptimize(go, "_LOD2", padre, calidadLOD2);
                lod2List.Add(c2.GetComponent<Renderer>());
            }
        }

        // Configurar niveles en LODGroup
        var lods = new List<LOD>();
        if (lod0List.Count > 0) lods.Add(new LOD(0.6f, lod0List.ToArray()));
        if (lod1List.Count > 0) lods.Add(new LOD(0.3f, lod1List.ToArray()));
        if (lod2List.Count > 0) lods.Add(new LOD(0.1f, lod2List.ToArray()));

        lg.SetLODs(lods.ToArray());
        lg.RecalculateBounds();

        Debug.Log($"✅ LODGroup configurado con {lods.Count} niveles: " +
                  $"LOD0={lod0List.Count}, LOD1={lod1List.Count}, LOD2={lod2List.Count}");
    }

    // Clona solo el MeshFilter/MeshRenderer y aplica OptimizeMesh si corresponde
    GameObject CloneAndOptimize(GameObject original, string sufijo, Transform padre, float calidad)
    {
        var clone = new GameObject(original.name + sufijo);
        clone.transform.SetParent(padre);
        clone.transform.localPosition = original.transform.localPosition;
        clone.transform.localRotation = original.transform.localRotation;
        clone.transform.localScale = original.transform.localScale;

        var origMF = original.GetComponent<MeshFilter>();
        var origMR = original.GetComponent<MeshRenderer>();
        var mf = clone.AddComponent<MeshFilter>();
        var mr = clone.AddComponent<MeshRenderer>();

        mf.sharedMesh = origMF.sharedMesh;
        mr.sharedMaterials = origMR.sharedMaterials;

#if UNITY_EDITOR
        // Optimizar malla si la calidad es menor a 1
        if (calidad < 1f)
        {
            var optimizer = clone.AddComponent<OptimizeMesh>();
            var t = optimizer.GetType();
            t.GetField("_quality", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, calidad);
            t.GetField("_renderer", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, mf);
            t.GetField("_mesh", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(optimizer, mf.sharedMesh);
            t.GetMethod("DecimateMesh", BindingFlags.Instance | BindingFlags.Public)?.Invoke(optimizer, null);

            // Guardar asset optimizado
            var meshCopy = Object.Instantiate(mf.sharedMesh);
            var path = Path.Combine(carpetaDestino, "optimized__" + clone.name + ".asset").Replace("\\", "/");
            AssetDatabase.CreateAsset(meshCopy, path);
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            Debug.Log($"[LOD] Mesh optimizado guardado: {path}");
        }
#endif

        return clone;
    }
}
