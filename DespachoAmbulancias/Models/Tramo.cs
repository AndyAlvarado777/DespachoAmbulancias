namespace DespachoAmbulancias.Models;


public class Tramo
{
    private int _tiempoMinutos;
    public string OrigenId { get; set; } = string.Empty;
    public string DestinoId { get; set; } = string.Empty;
    public int TiempoMinutos
    {
        get => _tiempoMinutos;
        set
        {
            ValidarPesoNoNegativo(value);
            _tiempoMinutos = value;
        }
    }
    public Tramo() { }
    public Tramo(string origenId, string destinoId, int tiempoMinutos)
    {
        if (string.IsNullOrWhiteSpace(origenId))
            throw new ArgumentException("El Id de origen no puede estar vacío.", nameof(origenId));

        if (string.IsNullOrWhiteSpace(destinoId))
            throw new ArgumentException("El Id de destino no puede estar vacío.", nameof(destinoId));

        ValidarPesoNoNegativo(tiempoMinutos);

        OrigenId = origenId.Trim().ToUpperInvariant();
        DestinoId = destinoId.Trim().ToUpperInvariant();
        _tiempoMinutos = tiempoMinutos;
    }
    
    public static void ValidarPesoNoNegativo(int peso)
    {
        if (peso < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(peso),
                "El tiempo de recorrido no puede ser negativo. Dijkstra requiere pesos no negativos (w >= 0).");
        }
    }

    public override string ToString() => $"{OrigenId} ---> {DestinoId} ({TiempoMinutos} min)";
}
