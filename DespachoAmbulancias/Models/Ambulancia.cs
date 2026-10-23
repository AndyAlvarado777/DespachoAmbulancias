namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa una unidad móvil de respuesta prehospitalaria.
/// <para>
/// <b>Justificación en el Grafo Vial:</b>
/// Cada ambulancia está estacionada en un vértice \( u \in V \) del grafo (base o nodo vial).
/// Cuando se despacha, el algoritmo de Dijkstra se ejecuta en reversa sobre el Grafo Transpuesto (\( G^T \))
/// para encontrar cuál unidad disponible minimiza el tiempo de arribo al incidente.
/// </para>
/// </summary>
public class Ambulancia
{
    /// <summary>
    /// Código identificador de la unidad (ej. "AMB-01", "U-02").
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Número de placa vehicular oficial.
    /// </summary>
    public string Placa { get; set; } = string.Empty;

    /// <summary>
    /// Estado operativo actual de la ambulancia.
    /// </summary>
    public EstadoAmbulancia Estado { get; set; } = EstadoAmbulancia.Disponible;

    /// <summary>
    /// Vértice del mapa vial donde se encuentra la ambulancia físicamente en este instante.
    /// </summary>
    public string UbicacionActualNodoId { get; set; } = string.Empty;

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Ambulancia() { }

    /// <summary>
    /// Registra una nueva unidad de ambulancia.
    /// </summary>
    public Ambulancia(string id, string placa, string ubicacionActualNodoId, EstadoAmbulancia estado = EstadoAmbulancia.Disponible)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El Id de la ambulancia no puede estar vacío.", nameof(id));

        if (string.IsNullOrWhiteSpace(placa))
            throw new ArgumentException("La placa de la ambulancia no puede estar vacía.", nameof(placa));

        if (string.IsNullOrWhiteSpace(ubicacionActualNodoId))
            throw new ArgumentException("La ubicación inicial de la ambulancia no puede estar vacía.", nameof(ubicacionActualNodoId));

        Id = id.Trim().ToUpperInvariant();
        Placa = placa.Trim().ToUpperInvariant();
        UbicacionActualNodoId = ubicacionActualNodoId.Trim().ToUpperInvariant();
        Estado = estado;
    }

    /// <summary>
    /// Indica si la unidad está lista para ser asignada a una emergencia.
    /// </summary>
    public bool HayDisponible() => Estado == EstadoAmbulancia.Disponible;

    /// <summary>
    /// Reserva la unidad para servicio activo si se encuentra disponible.
    /// </summary>
    /// <returns><c>true</c> si pudo tomarse en servicio; de lo contrario <c>false</c>.</returns>
    public bool TomarDisponible()
    {
        if (HayDisponible())
        {
            Estado = EstadoAmbulancia.EnRuta;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Finaliza la atención o traslado y devuelve la ambulancia al estado disponible.
    /// </summary>
    /// <param name="nuevoNodoId">Nodo del mapa donde queda estacionada tras la liberación (ej. hospital o base).</param>
    public void Liberar(string nuevoNodoId)
    {
        if (string.IsNullOrWhiteSpace(nuevoNodoId))
            throw new ArgumentException("El nodo final de destino no puede estar vacío.", nameof(nuevoNodoId));

        UbicacionActualNodoId = nuevoNodoId.Trim().ToUpperInvariant();
        Estado = EstadoAmbulancia.Disponible;
    }

    public override string ToString()
    {
        return $"[{Id}] Placa: {Placa} | Ubicación: {UbicacionActualNodoId} | Estado: {Estado}";
    }
}
