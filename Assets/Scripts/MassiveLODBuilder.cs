// ========== MassiveLODBuilder.cs ==========
// Ubicación: Assets/Scripts/MassiveLODBuilder.cs

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class MassiveLODBuilder : MonoBehaviour
{
    [Header("Calidad por LOD (0 = peor, 1 = original)")]
    [Range(0f, 1f)] public float calidadLOD0 = 1.0f;
    [Range(0f, 1f)] public float calidadLOD1 = 0.5f;
    [Range(0f, 1f)] public float calidadLOD2 = 0.2f;

    private const string carpetaDestino = "Assets/MeshesOptimized/";

    // Cambiado a public para que el editor script pueda invocarlo
    [ContextMenu("⚙ Generar LODs (Instancia en Escena)")]
    public void GenerarLODs()
    {
#if UNITY_EDITOR
        // Crear carpeta destino si no existe
        if (!AssetDatabase.IsValidFolder(carpetaDestino.TrimEnd('/')))
        {
            Directory.CreateDirectory(carpetaDestino);
            AssetDatabase.Refresh();
        }
#endif

        Transform padre = this.transform;
        List<Renderer> lod0 = new List<Renderer>();
        List<Renderer> lod1 = new List<Renderer>();
        List<Renderer> lod2 = new List<Renderer>();

        MeshRenderer[] renderers = padre.GetComponentsInChildren<MeshRenderer>(false);
        foreach (var r in renderers)
        {
            GameObject go = r.gameObject;
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            // LOD0
            GameObject c0 = CloneAndOptimize(go, "_LOD0", padre, calidadLOD0);
            lod0.Add(c0.GetComponent<Renderer>());
            // LOD1
            GameObject c1 = CloneAndOptimize(go, "_LOD1", padre, calidadLOD1);
            lod1.Add(c1.GetComponent<Renderer>());
            // LOD2
            GameObject c2 = CloneAndOptimize(go, "_LOD2", padre, calidadLOD2);
            lod2.Add(c2.GetComponent<Renderer>());

            go.SetActive(false);
        }

        // Configurar LODGroup
        LODGroup lg = padre.GetComponent<LODGroup>() ?? padre.gameObject.AddComponent<LODGroup>();
        LOD[] lods = new LOD[3];
        lods[0] = new LOD(0.6f, lod0.ToArray());
        lods[1] = new LOD(0.3f, lod1.ToArray());
        lods[2] = new LOD(0.1f, lod2.ToArray());
        lg.SetLODs(lods);
        lg.RecalculateBounds();
    }

    GameObject CloneAndOptimize(GameObject original, string sufijo, Transform padre, float calidad)
    {
        GameObject clone = new GameObject(original.name + sufijo);
        clone.transform.SetParent(padre);
        clone.transform.localPosition = original.transform.localPosition;
        clone.transform.localRotation = original.transform.localRotation;
        clone.transform.localScale = original.transform.localScale;

        MeshFilter mf = clone.AddComponent<MeshFilter>();
        MeshRenderer mr = clone.AddComponent<MeshRenderer>();
        MeshFilter omf = original.GetComponent<MeshFilter>();
        MeshRenderer omr = original.GetComponent<MeshRenderer>();
        mf.sharedMesh = omf.sharedMesh;
        mr.sharedMaterials = omr.sharedMaterials;

#if UNITY_EDITOR
        // Optimizar si calidad <1
        if (calidad < 1f)
        {
            var optimizer = clone.AddComponent<OptimizeMesh>();
            var t = optimizer.GetType();
            t.GetField("_quality", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(optimizer, calidad);
            t.GetField("_renderer", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(optimizer, mf);
            t.GetField("_mesh", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(optimizer, mf.sharedMesh);
            t.GetMethod("DecimateMesh", BindingFlags.Instance | BindingFlags.Public).Invoke(optimizer, null);

            // Guardar asset
            Mesh meshCopy = Object.Instantiate(mf.sharedMesh);
            string path = Path.Combine(carpetaDestino, "optimized__" + clone.name + ".asset").Replace("\\", "/");
            AssetDatabase.CreateAsset(meshCopy, path);
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        }
#endif
        return clone;
    }
}