using UnityEngine;
using System.Collections.Generic;

public class MassiveLODBuilder : MonoBehaviour
{
    [ContextMenu("⚙ Generar LODs con clones completos")]
    void GenerarLODs()
    {
        Transform padre = this.transform;

        List<Renderer> lod0Renderers = new List<Renderer>();
        List<Renderer> lod1Renderers = new List<Renderer>();
        List<Renderer> lod2Renderers = new List<Renderer>();

        Debug.Log("🔍 Buscando MeshRenderers...");
        MeshRenderer[] renderers = padre.GetComponentsInChildren<MeshRenderer>(includeInactive: false);
        Debug.Log($"📦 Se encontraron {renderers.Length} MeshRenderers");

        int procesados = 0;

        foreach (var renderer in renderers)
        {
            GameObject originalGO = renderer.gameObject;
            MeshFilter originalMF = originalGO.GetComponent<MeshFilter>();
            if (originalMF == null || originalMF.sharedMesh == null)
            {
                Debug.LogWarning($"⚠️ El objeto '{originalGO.name}' no tiene MeshFilter o Mesh asignado.");
                continue;
            }

            Debug.Log($"➡ Procesando '{originalGO.name}'");

            originalGO.SetActive(false);

            // Crear 3 clones y copiar mesh + material manualmente
            GameObject lod0 = CrearCloneConMesh(originalGO, "_LOD0", padre);
            GameObject lod1 = CrearCloneConMesh(originalGO, "_LOD1", padre);
            GameObject lod2 = CrearCloneConMesh(originalGO, "_LOD2", padre);

            lod0Renderers.Add(lod0.GetComponent<Renderer>());
            lod1Renderers.Add(lod1.GetComponent<Renderer>());
            lod2Renderers.Add(lod2.GetComponent<Renderer>());

            procesados++;
        }

        Debug.Log($"🧮 Total de objetos procesados correctamente: {procesados}");

        // Crear o usar LODGroup
        LODGroup lodGroup = padre.GetComponent<LODGroup>();
        if (!lodGroup)
        {
            lodGroup = padre.gameObject.AddComponent<LODGroup>();
            Debug.Log("📎 Se agregó LODGroup al objeto padre.");
        }

        // Configurar LODs
        LOD[] lods = new LOD[3];
        lods[0] = new LOD(0.6f, lod0Renderers.ToArray());
        lods[1] = new LOD(0.3f, lod1Renderers.ToArray());
        lods[2] = new LOD(0.1f, lod2Renderers.ToArray());

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        Debug.Log($"✅ LODGroup configurado: {lod0Renderers.Count} LOD0 | {lod1Renderers.Count} LOD1 | {lod2Renderers.Count} LOD2");
    }

    GameObject CrearCloneConMesh(GameObject original, string sufijo, Transform padre)
    {
        GameObject clone = new GameObject(original.name + sufijo);
        clone.transform.SetParent(padre);
        clone.transform.position = original.transform.position;
        clone.transform.rotation = original.transform.rotation;
        clone.transform.localScale = original.transform.localScale;

        MeshFilter originalMF = original.GetComponent<MeshFilter>();
        MeshRenderer originalMR = original.GetComponent<MeshRenderer>();

        MeshFilter newMF = clone.AddComponent<MeshFilter>();
        MeshRenderer newMR = clone.AddComponent<MeshRenderer>();

        newMF.sharedMesh = originalMF.sharedMesh;
        newMR.sharedMaterials = originalMR.sharedMaterials;

        // Agregar OptimizeMesh si no está
        if (!clone.GetComponent<OptimizeMesh>())
            clone.AddComponent<OptimizeMesh>();

        Debug.Log($"🛠️ Clonado: {clone.name} con mesh '{originalMF.sharedMesh.name}' y {originalMR.sharedMaterials.Length} materiales");

        return clone;
    }
}
