using System;

public class Nodo
{
    public string IdNodo { get; set; }
    public string Nombre { get; set; }

    public Nodo(string idNodo, string nombre)
    {
        IdNodo = idNodo;
        Nombre = nombre;
    }
}