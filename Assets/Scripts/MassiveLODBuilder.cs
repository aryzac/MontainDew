// ✅ PRIMERA PARTE: FUNCIONA SOBRE INSTANCIAS EN ESCENA
// Ejecutar solo en objetos que están en la jerarquía, no en el panel Project

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
    void GenerarLODs()
    {
        if (!AssetDatabase.IsValidFolder(carpetaDestino.TrimEnd('/')))
        {
            Directory.CreateDirectory(carpetaDestino);
            AssetDatabase.Refresh();
        }

        Transform padre = this.transform;

        List<Renderer> lod0Renderers = new List<Renderer>();
        List<Renderer> lod1Renderers = new List<Renderer>();
        List<Renderer> lod2Renderers = new List<Renderer>();

        MeshRenderer[] renderers = padre.GetComponentsInChildren<MeshRenderer>(includeInactive: false);
        Debug.Log($"📦 Se encontraron {renderers.Length} objetos con MeshRenderer");

        foreach (var renderer in renderers)
        {
            GameObject originalGO = renderer.gameObject;
            MeshFilter originalMF = originalGO.GetComponent<MeshFilter>();
            if (originalMF == null || originalMF.sharedMesh == null)
            {
                Debug.LogWarning($"⚠️ '{originalGO.name}' no tiene MeshFilter o mesh asignado.");
                continue;
            }

            GameObject lod0 = null;

            if (calidadLOD0 < 1f)
            {
                lod0 = ClonarYOptimizar(originalGO, "_LOD0", originalGO.transform.parent, calidadLOD0);
                lod0Renderers.AddRange(lod0.GetComponentsInChildren<Renderer>());
            }
            else
            {
                lod0 = Instantiate(originalGO, originalGO.transform.parent);
                lod0.name = originalGO.name + "_LOD0";
                lod0.transform.localPosition = originalGO.transform.localPosition;
                lod0.transform.localRotation = originalGO.transform.localRotation;
                lod0.transform.localScale = originalGO.transform.localScale;
                lod0Renderers.AddRange(lod0.GetComponentsInChildren<Renderer>());
            }

            if (calidadLOD1 < 1f)
            {
                GameObject lod1 = ClonarYOptimizar(originalGO, "_LOD1", originalGO.transform.parent, calidadLOD1);
                lod1Renderers.AddRange(lod1.GetComponentsInChildren<Renderer>());
            }

            if (calidadLOD2 < 1f)
            {
                GameObject lod2 = ClonarYOptimizar(originalGO, "_LOD2", originalGO.transform.parent, calidadLOD2);
                lod2Renderers.AddRange(lod2.GetComponentsInChildren<Renderer>());
            }

            // Desactivar original
            originalGO.SetActive(false);
        }

        LODGroup lodGroup = padre.GetComponent<LODGroup>();
        if (!lodGroup)
            lodGroup = padre.gameObject.AddComponent<LODGroup>();

        List<LOD> lodsList = new List<LOD>();
        if (lod0Renderers.Count > 0) lodsList.Add(new LOD(0.6f, lod0Renderers.ToArray()));
        if (lod1Renderers.Count > 0) lodsList.Add(new LOD(0.3f, lod1Renderers.ToArray()));
        if (lod2Renderers.Count > 0) lodsList.Add(new LOD(0.1f, lod2Renderers.ToArray()));

        lodGroup.SetLODs(lodsList.ToArray());
        lodGroup.RecalculateBounds();

        Debug.Log("✅ LODGroup generado correctamente y jerarquía preservada.");
    }

    GameObject ClonarYOptimizar(GameObject original, string sufijo, Transform destino, float calidad)
    {
        GameObject clone = Instantiate(original);
        clone.name = original.name + sufijo;
        clone.transform.SetParent(destino);
        clone.transform.localPosition = original.transform.localPosition;
        clone.transform.localRotation = original.transform.localRotation;
        clone.transform.localScale = original.transform.localScale;

        MeshFilter mf = clone.GetComponent<MeshFilter>();
        MeshRenderer mr = clone.GetComponent<MeshRenderer>();

        if (!mf || !mr || mf.sharedMesh == null) return clone;

        var optimizer = clone.AddComponent<OptimizeMesh>();
        SetPrivateField(optimizer, "_quality", calidad);
        SetPrivateField(optimizer, "_renderer", mf);
        SetPrivateField(optimizer, "_mesh", mf.sharedMesh);

        var decimate = optimizer.GetType().GetMethod("DecimateMesh", BindingFlags.Instance | BindingFlags.Public);
        decimate?.Invoke(optimizer, null);

        string nombreMesh = $"Optimized__{clone.name.ToLower()}.asset";
        string path = Path.Combine(carpetaDestino, nombreMesh).Replace("\\", "/");

        Mesh meshParaGuardar = Object.Instantiate(mf.sharedMesh);
        AssetDatabase.CreateAsset(meshParaGuardar, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Mesh meshCargado = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        mf.sharedMesh = meshCargado;

        Debug.Log($"💾 Mesh guardado: {path}");

        return clone;
    }

    void SetPrivateField(Component target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
            field.SetValue(target, value);
    }
}
