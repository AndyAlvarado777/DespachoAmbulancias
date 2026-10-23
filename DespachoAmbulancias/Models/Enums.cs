namespace DespachoAmbulancias.Models;

/// <summary>
/// Clasificación clínica de triaje para priorización de emergencias (Sistema START/Manchester adaptado).
/// <para>
/// <b>Justificación de Estructura de Datos:</b>
/// En una cola de prioridad basada en Min-Heap, los valores numéricos menores representan
/// mayor urgencia (1 = Rojo tiene mayor prioridad que 2 = Amarillo y 3 = Verde).
/// </para>
/// </summary>
public enum NivelTriaje
{
    /// <summary>Prioridad 1: Riesgo vital inminente (paro cardiorrespiratorio, hemorragia masiva, trauma severo).</summary>
    Rojo = 1,

    /// <summary>Prioridad 2: Urgencia médica no inmediatamente mortal pero de pronta atención (fracturas mayores, dolor torácico estable).</summary>
    Amarillo = 2,

    /// <summary>Prioridad 3: Condición leve o no urgente (heridas menores, contusiones leves).</summary>
    Verde = 3
}

/// <summary>
/// Estado operativo del recurso móvil (ambulancia).
/// Representa el ciclo de vida de la unidad dentro del despacho.
/// </summary>
public enum EstadoAmbulancia
{
    /// <summary>La unidad está en base o nodo asignado y puede recibir una nueva emergencia.</summary>
    Disponible,

    /// <summary>La unidad se encuentra desplazándose hacia la escena o hacia el hospital de referencia.</summary>
    EnRuta,

    /// <summary>La unidad está comprometida en la escena brindando primeros auxilios o transfiriendo paciente.</summary>
    Ocupada
}

/// <summary>
/// Define la naturaleza o función del vértice dentro de la red vial urbana (Grafo G).
/// </summary>
public enum TipoNodo
{
    /// <summary>Intersección estándar o cruce de avenidas/calles.</summary>
    Interseccion,

    /// <summary>Centro asistencial u hospital con capacidad para recibir pacientes.</summary>
    Hospital,

    /// <summary>Base operativa o estación de salida de ambulancias.</summary>
    BaseAmbulancia,

    /// <summary>Punto de referencia o zona de interés urbano (residencial, comercial).</summary>
    PuntoInteres
}
