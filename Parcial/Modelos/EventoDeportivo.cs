namespace Planazo.Modelos;

public sealed class EventoDeportivo(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
    : Evento(id, nombre, fecha, precioBase, capacidadMaxima)
{
    public override decimal CalcularPrecioFinal() => PrecioBase * 1.08m;
}
