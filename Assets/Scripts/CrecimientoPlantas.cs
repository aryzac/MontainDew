using UnityEngine;

public class CrecimientoPlantas : MonoBehaviour
{
    public enum EstadoTile
    {
        Virgen,     // Nunca arado ni sembrado
        Sembrado,   // Semilla plantada, aún no arado
        Arado,      // Tierra arada y lista para crecer
        Creciendo,  // Planta creciendo (varias aguas)
        Maduro      // Planta lista para cosechar
    }

    [Header("Configuración de tiempos (segundos)")]
    public float tiempoParaMadurar = 30f;

    [Header("Cantidad de cosecha/ganancia")]
    public int dineroPorCosecha = 5;

    [HideInInspector]
    public EstadoTile estadoActual = EstadoTile.Virgen;

    private float tiempoInicio;
    private int cuentaRiegos;

    /// <summary>
    /// Arar el cantero (solo funciona si está Sembrado).
    /// </summary>
    public bool Arar()
    {
        if (estadoActual != EstadoTile.Sembrado) return false;
        estadoActual = EstadoTile.Arado;
        Debug.Log("[CrecimientoPlanta] Tierra arada.");
        return true;
    }

    /// <summary>
    /// Plantar semilla (solo si está Virgen).
    /// </summary>
    public bool Plantar()
    {
        if (estadoActual != EstadoTile.Virgen) return false;
        estadoActual = EstadoTile.Sembrado;
        Debug.Log("[CrecimientoPlanta] Semilla plantada.");
        return true;
    }

    /// <summary>
    /// Regar la planta: si está Arado o Creciendo, inicia o acelera el crecimiento.
    /// </summary>
    public bool Regar()
    {
        if (estadoActual != EstadoTile.Arado && estadoActual != EstadoTile.Creciendo)
            return false;

        cuentaRiegos++;
        if (estadoActual == EstadoTile.Arado)
        {
            estadoActual = EstadoTile.Creciendo;
            tiempoInicio = Time.time;
            Debug.Log("[CrecimientoPlanta] Comienza ciclo de crecimiento.");
        }
        else
        {
            Debug.Log("[CrecimientoPlanta] Riego extra recibido.");
        }
        return true;
    }

    void Update()
    {
        if (estadoActual == EstadoTile.Creciendo)
        {
            float t = Time.time - tiempoInicio;
            // Acelerar según riegos: más riegos = menos tiempo
            float factor = 1f / (1 + cuentaRiegos);
            if (t * factor >= tiempoParaMadurar)
            {
                estadoActual = EstadoTile.Maduro;
                Debug.Log("[CrecimientoPlanta] Planta madura.");
            }
        }
    }

    /// <summary>
    /// Cosechar la planta y generar dinero (solo si está Maduro).
    /// </summary>
    public bool Cosechar()
    {
        if (estadoActual != EstadoTile.Maduro) return false;
        estadoActual = EstadoTile.Virgen;
        cuentaRiegos = 0;
        // TODO: Notificar al sistema de inventario/monedas
        Debug.Log($"[CrecimientoPlanta] Planta cosechada. +{dineroPorCosecha} de dinero.");
        return true;
    }
}
