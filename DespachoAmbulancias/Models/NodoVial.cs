namespace DespachoAmbulancias.Models;


public class NodoVial : IEquatable<NodoVial>
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TipoNodo Tipo { get; set; } = TipoNodo.Interseccion;
    public NodoVial() { }
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
