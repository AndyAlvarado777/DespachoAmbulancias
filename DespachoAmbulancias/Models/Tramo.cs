namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa una arista dirigida \( e = (u, v) \in E \) con ponderación no negativa dentro del Grafo Vial \( G \).
/// <para>
/// <b>Justificación Teórica:</b>
/// En el modelo de tránsito urbano, las calles pueden ser de un solo sentido (arista dirigida)
/// o doble sentido (dos aristas dirigidas opuestas). El peso representa el tiempo promedio en minutos.
/// La no-negatividad (\( w \ge 0 \)) es condición matemáticamente obligatoria para la validez
/// y optimalidad del Algoritmo de Dijkstra.
/// </para>
/// </summary>
public class Tramo
{
    private int _tiempoMinutos;

    /// <summary>
    /// Identificador del nodo de origen \( u \).
    /// </summary>
    public string OrigenId { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del nodo de destino \( v \).
    /// </summary>
    public string DestinoId { get; set; } = string.Empty;

    /// <summary>
    /// Tiempo estimado de viaje en minutos (peso de la arista).
    /// Debe ser mayor o igual a cero.
    /// </summary>
    public int TiempoMinutos
    {
        get => _tiempoMinutos;
        set
        {
            ValidarPesoNoNegativo(value);
            _tiempoMinutos = value;
        }
    }

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Tramo() { }

    /// <summary>
    /// Crea una nueva arista dirigida con su correspondiente ponderación temporal.
    /// </summary>
    /// <param name="origenId">Id del nodo origen.</param>
    /// <param name="destinoId">Id del nodo destino.</param>
    /// <param name="tiempoMinutos">Peso o coste de desplazamiento en minutos.</param>
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

    /// <summary>
    /// Valida que el tiempo no sea negativo, requisito indispensable para el algoritmo de Dijkstra.
    /// </summary>
    /// <param name="peso">Valor del tiempo a validar.</param>
    /// <exception cref="ArgumentOutOfRangeException">Se lanza si el peso es menor a cero.</exception>
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
