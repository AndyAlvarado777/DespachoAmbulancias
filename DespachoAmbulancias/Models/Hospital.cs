namespace DespachoAmbulancias.Models;

public class Hospital
{
    private int _camasDisponibles;
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
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

    public string UbicacionNodoId { get; set; } = string.Empty;
    public Hospital() { }
   
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

    public bool TieneCamasDisponibles() => CamasDisponibles > 0;

    public bool OcuparCama()
    {
        if (TieneCamasDisponibles())
        {
            CamasDisponibles--;
            return true;
        }
        return false;
    }

    public void LiberarCama()
    {
        CamasDisponibles++;
    }

    public override string ToString()
    {
        return $"[{Id}] {Nombre} | Nodo: {UbicacionNodoId} | Camas Libres: {CamasDisponibles}";
    }
}
