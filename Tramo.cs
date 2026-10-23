using System;

public class Tramo
{
    public string Destino { get; set; }
    public double Tiempo { get; set; } // Tiempo en minutos

    public Tramo(string destino, double tiempo)
    {
        Destino = destino;
        Tiempo = tiempo;
    }
}
