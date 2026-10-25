namespace DespachoAmbulancias.Models;


public class Emergencia : IComparable<Emergencia>
{
    public string Id { get; set; } = string.Empty;
    public string Paciente { get; set; } = string.Empty;
    public NivelTriaje NivelTriaje { get; set; } = NivelTriaje.Verde;
    public string UbicacionNodoId { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public Emergencia() { }
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


    public int CompareTo(Emergencia? other)
    {
        if (other is null) return -1;
        if (ReferenceEquals(this, other)) return 0;

        //Triaje (1 = Rojo, 2 = Amarillo, 3 = Verde)
        int comparacionTriaje = NivelTriaje.CompareTo(other.NivelTriaje);
        if (comparacionTriaje != 0)
        {
            return comparacionTriaje;
        }

        //Marca de tiempo 
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
