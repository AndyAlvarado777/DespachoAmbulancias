namespace DespachoAmbulancias.Models;

/// <summary>
/// Representa un vértice \( u \in V \) dentro de la red vial modelada como Grafo Dirigido \( G = (V, E) \).
/// <para>
/// <b>Justificación de Estructura de Datos:</b>
/// Cada nodo actúa como clave primaria de acceso en las listas de adyacencia implementadas
/// sobre tablas hash (<c>Dictionary&lt;string, List&lt;Tramo&gt;&gt;</c>), permitiendo búsquedas en \( O(1) \).
/// </para>
/// </summary>
public class NodoVial : IEquatable<NodoVial>
{
    /// <summary>
    /// Identificador único del nodo (ej. "NODO-01", "HOSP-01", "AMB-BASE-1").
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo o dirección del vértice (ej. "Av. Roosevelt y 49 Av. Sur").
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Categoría funcional del nodo en la red vial.
    /// </summary>
    public TipoNodo Tipo { get; set; } = TipoNodo.Interseccion;

    /// <summary>
    /// Constructor por defecto para deserialización y flexibilidad POO.
    /// </summary>
    public NodoVial() { }

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="NodoVial"/> con los valores requeridos.
    /// </summary>
    /// <param name="id">Identificador único del nodo vial.</param>
    /// <param name="nombre">Nombre descriptivo o cruce vial.</param>
    /// <param name="tipo">Tipo o categoría de infraestructura del nodo.</param>
    /// <exception cref="ArgumentException">Se lanza si el id o el nombre están vacíos.</exception>
    public NodoVial(string id, string nombre, TipoNodo tipo = TipoNodo.Interseccion)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("El Id del nodo vial no puede ser nulo ni vacío.", nameof(id));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del nodo vial no puede ser nulo ni vacío.", nameof(nombre));

        Id = id.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        Tipo = tipo;
    }

    /// <summary>
    /// Representación textual amigable para la interfaz de consola.
    /// </summary>
    public override string ToString() => $"[{Id}] {Nombre} ({Tipo})";

    public bool Equals(NodoVial? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as NodoVial);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Id);
}
