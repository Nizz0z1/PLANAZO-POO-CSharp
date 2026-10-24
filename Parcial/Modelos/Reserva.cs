namespace Planazo.Modelos;

public enum EstadoReserva { Activa, Cancelada }

public sealed class Reserva
{
    internal Reserva(int id, Usuario usuario, Evento evento, int cantidadEntradas)
    {
        Id = id;
        Usuario = usuario;
        Evento = evento;
        CantidadEntradas = cantidadEntradas;
        ValorTotal = evento.CalcularPrecioFinal() * cantidadEntradas;
        Estado = EstadoReserva.Activa;
    }

    public int Id { get; }
    public Usuario Usuario { get; }
    public Evento Evento { get; }
    public int CantidadEntradas { get; }
    public decimal ValorTotal { get; }
    public EstadoReserva Estado { get; private set; }

    internal void MarcarCancelada()
    {
        if (Estado == EstadoReserva.Cancelada) throw new InvalidOperationException("La reserva ya fue cancelada.");
        Estado = EstadoReserva.Cancelada;
    }
}
