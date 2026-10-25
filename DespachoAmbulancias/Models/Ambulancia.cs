namespace DespachoAmbulancias.Models;

public class Ambulancia
{
    public string Id { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public EstadoAmbulancia Estado { get; set; } = EstadoAmbulancia.Disponible;
    public string UbicacionActualNodoId { get; set; } = string.Empty;

    public Ambulancia() { }

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

    public bool HayDisponible() => Estado == EstadoAmbulancia.Disponible;

    public bool TomarDisponible()
    {
        if (HayDisponible())
        {
            Estado = EstadoAmbulancia.EnRuta;
            return true;
        }
        return false;
    }

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
