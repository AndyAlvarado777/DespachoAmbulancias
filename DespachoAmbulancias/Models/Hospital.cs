namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa una institución hospitalaria de destino para el traslado del paciente.
/// <para>
/// <b>Justificación en el Grafo Vial:</b>
/// Cada hospital está asociado a un nodo receptor \( v \in V \). Al despachar una emergencia,
/// se calcula la ruta más corta desde el incidente hacia los hospitales usando el Grafo Original \( G \),
/// seleccionando el centro más cercano que tenga <see cref="CamasDisponibles"/> &gt; 0.
/// </para>
/// </summary>
public class Hospital
{
    private int _camasDisponibles;

    /// <summary>
    /// Identificador del hospital (ej. "HOSP-ROSALES", "H-01").
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la institución médica.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Número de camas o espacios de atención de urgencias libres.
    /// </summary>
    public int CamasDisponibles
    {
        get => _camasDisponibles;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "El número de camas disponibles no puede ser negativo.");
            _camasDisponibles = value;
        }
    }

    /// <summary>
    /// Vértice vial del mapa donde se ubica el acceso de emergencias del hospital.
    /// </summary>
    public string UbicacionNodoId { get; set; } = string.Empty;

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Hospital() { }

    /// <summary>
    /// Registra un nuevo centro hospitalario.
    /// </summary>
    public Hospital(string id, string nombre, int camasDisponibles, string ubicacionNodoId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El Id del hospital no puede estar vacío.", nameof(id));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del hospital no puede estar vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(ubicacionNodoId))
            throw new ArgumentException("El nodo de ubicación del hospital no puede estar vacío.", nameof(ubicacionNodoId));

        if (camasDisponibles < 0)
            throw new ArgumentOutOfRangeException(nameof(camasDisponibles), "Las camas no pueden ser un valor negativo.");

        Id = id.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        _camasDisponibles = camasDisponibles;
        UbicacionNodoId = ubicacionNodoId.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Comprueba si el hospital tiene capacidad para recibir a un nuevo paciente.
    /// </summary>
    public bool TieneCamasDisponibles() => CamasDisponibles > 0;

    /// <summary>
    /// Descuenta una cama al ingresar un paciente derivado de un despacho.
    /// </summary>
    /// <returns><c>true</c> si se pudo reservar; de lo contrario <c>false</c>.</returns>
    public bool OcuparCama()
    {
        if (TieneCamasDisponibles())
        {
            CamasDisponibles--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Incrementa la capacidad cuando un paciente es dado de alta o transferido.
    /// </summary>
    public void LiberarCama()
    {
        CamasDisponibles++;
    }

    public override string ToString()
    {
        return $"[{Id}] {Nombre} | Nodo: {UbicacionNodoId} | Camas Libres: {CamasDisponibles}";
    }
}
