using UnityEngine;
using UnityEditor;
using System.Reflection;

[ExecuteInEditMode]
public class LODOptimizer : MonoBehaviour
{
    [Header("Calidad por LOD (0 = peor, 1 = original)")]
    [Range(0f, 1f)] public float calidadLOD0 = 1.0f;
    [Range(0f, 1f)] public float calidadLOD1 = 0.5f;
    [Range(0f, 1f)] public float calidadLOD2 = 0.2f;

    [ContextMenu("🔧 Optimizar LODs")]
    public void OptimizarLODs()
    {
        int optimizados = 0;

        foreach (Transform hijo in transform)
        {
            if (!hijo.name.EndsWith("_LOD0") &&
                !hijo.name.EndsWith("_LOD1") &&
                !hijo.name.EndsWith("_LOD2"))
                continue;

            Component optimizer = hijo.GetComponent("OptimizeMesh");
            if (optimizer == null)
            {
                Debug.LogWarning($"⚠️ {hijo.name} no tiene OptimizeMesh.");
                continue;
            }

            // Asignar calidad por sufijo
            float calidad = hijo.name.EndsWith("_LOD0") ? calidadLOD0 :
                            hijo.name.EndsWith("_LOD1") ? calidadLOD1 : calidadLOD2;

            // Setear _quality via Reflection
            FieldInfo qualityField = optimizer.GetType().GetField("_quality", BindingFlags.NonPublic | BindingFlags.Instance);
            qualityField?.SetValue(optimizer, calidad);

            // Forzar asignación de _renderer y _mesh si estamos en Editor
            MeshFilter mf = hijo.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                FieldInfo rendererField = optimizer.GetType().GetField("_renderer", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo meshField = optimizer.GetType().GetField("_mesh", BindingFlags.NonPublic | BindingFlags.Instance);

                rendererField?.SetValue(optimizer, mf);
                meshField?.SetValue(optimizer, mf.sharedMesh);
            }
            else
            {
                Debug.LogWarning($"⚠️ {hijo.name} no tiene MeshFilter o no tiene mesh asignado.");
                continue;
            }

            // Llamar a DecimateMesh() via Reflection
            MethodInfo decimateMethod = optimizer.GetType().GetMethod("DecimateMesh", BindingFlags.Public | BindingFlags.Instance);
            if (decimateMethod != null)
            {
                decimateMethod.Invoke(optimizer, null);
                Debug.Log($"✅ {hijo.name} optimizado con calidad {calidad:0.00}");
                optimizados++;
            }
            else
            {
                Debug.LogError($"❌ No se encontró el método DecimateMesh en {hijo.name}");
            }
        }

        Debug.Log($"🎯 Optimización finalizada. Total objetos procesados: {optimizados}");
    }
}
