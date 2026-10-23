namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa un incidente o solicitud de auxilio médico recibido por el 911.
/// <para>
/// <b>Justificación de Estructura de Datos:</b>
/// Las emergencias se gestionan en una cola de prioridad basada en montículo binario (Min-Heap).
/// Implementa <see cref="IComparable{Emergencia}"/> para formalizar el criterio clínico:
/// primero por severidad de Triaje (Rojo &gt; Amarillo &gt; Verde), y en caso de empate,
/// por orden de llegada FIFO (marca de tiempo <see cref="FechaHora"/> más antigua primero).
/// </para>
/// </summary>
public class Emergencia : IComparable<Emergencia>
{
    /// <summary>
    /// Código identificador único del incidente (ej. "EMG-001").
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del paciente o reportante del incidente.
    /// </summary>
    public string Paciente { get; set; } = string.Empty;

    /// <summary>
    /// Clasificación clínica de gravedad (Rojo = 1, Amarillo = 2, Verde = 3).
    /// </summary>
    public NivelTriaje NivelTriaje { get; set; } = NivelTriaje.Verde;

    /// <summary>
    /// Vértice vial más cercano donde se ubica el incidente para el cálculo de rutas en el grafo.
    /// </summary>
    public string UbicacionNodoId { get; set; } = string.Empty;

    /// <summary>
    /// Marca de tiempo exacta en que se registró la llamada en la central de emergencias.
    /// </summary>
    public DateTime FechaHora { get; set; } = DateTime.Now;

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Emergencia() { }

    /// <summary>
    /// Registra una nueva emergencia validando la integridad de sus campos.
    /// </summary>
    public Emergencia(string id, string paciente, NivelTriaje nivelTriaje, string ubicacionNodoId, DateTime? fechaHora = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El Id de la emergencia no puede estar vacío.", nameof(id));

        if (string.IsNullOrWhiteSpace(paciente))
            throw new ArgumentException("El nombre del paciente no puede estar vacío.", nameof(paciente));

        if (string.IsNullOrWhiteSpace(ubicacionNodoId))
            throw new ArgumentException("El nodo de ubicación no puede estar vacío.", nameof(ubicacionNodoId));

        Id = id.Trim().ToUpperInvariant();
        Paciente = paciente.Trim();
        NivelTriaje = nivelTriaje;
        UbicacionNodoId = ubicacionNodoId.Trim().ToUpperInvariant();
        FechaHora = fechaHora ?? DateTime.Now;
    }

    /// <summary>
    /// Compara dos emergencias para determinar el orden de atención en la cola de prioridad.
    /// <para>
    /// Criterio 1: Nivel de Triaje (menor valor numérico = mayor urgencia clínica).
    /// Criterio 2: Marca de tiempo (menor fecha/hora = llamada recibida antes, principio FIFO).
    /// </para>
    /// </summary>
    public int CompareTo(Emergencia? other)
    {
        if (other is null) return -1;
        if (ReferenceEquals(this, other)) return 0;

        // Comparación primaria: Triaje (1 = Rojo, 2 = Amarillo, 3 = Verde)
        int comparacionTriaje = NivelTriaje.CompareTo(other.NivelTriaje);
        if (comparacionTriaje != 0)
        {
            return comparacionTriaje;
        }

        // Comparación secundaria: Marca de tiempo (desempate FIFO)
        return FechaHora.CompareTo(other.FechaHora);
    }

    public override string ToString()
    {
        string etiquetaTriaje = NivelTriaje switch
        {
            NivelTriaje.Rojo => "[ROJO - CRÍTICO]",
            NivelTriaje.Amarillo => "[AMARILLO - URGENTE]",
            NivelTriaje.Verde => "[VERDE - LEVE]",
            _ => "[DESCONOCIDO]"
        };

        return $"{Id} | {etiquetaTriaje} Paciente: {Paciente} | Nodo: {UbicacionNodoId} | Hora: {FechaHora:HH:mm:ss}";
    }
}
