# PLANAZO - Eventos

Aplicación de consola en C# y .NET para consultar eventos, reservar entradas, cancelar reservas y revisar el resumen de ventas activas.

## Funcionalidades

- Catálogo inicial con dos películas, dos conciertos, dos talleres y un evento deportivo.
- Consulta de eventos, precios finales y cupos disponibles.
- Reservas asociadas a usuarios, con validación de cantidades y disponibilidad.
- Cancelación única de reservas, con devolución de cupos.
- Consulta de reservas por usuario y resumen de reservas activas.
- Manejo de errores de entrada sin cerrar inesperadamente el programa.

## Ejecución

Requiere .NET 10 SDK.

Para descargar el proyecto desde GitHub, debes de abrir la terminal en vs y ejecutar:

```bash
git clone https://github.com/Nizz0z1/PLANAZO-POO-CSharp.git
```
Despues, en la terminal, vas a elegir el programa:

```bash
cd PLANAZO-POO-CSharp
```

Y por ulitmo lo ejecutas con este codigo:

```bash
dotnet run
```

## Clases y relaciones

- `Evento` es la clase abstracta con los atributos comunes y el control encapsulado de cupos.
- `Cine`, `Concierto`, `Taller` y `EventoDeportivo` heredan de `Evento` e implementan `CalcularPrecioFinal()`.
- `Usuario` representa a la persona que reserva; el gestor reutiliza el mismo usuario cuando coincide el nombre, sin distinguir mayúsculas.
- `Reserva` asocia un usuario con un evento, conserva la cantidad, el valor calculado al confirmar y el estado.
- `GestorReservas` administra las colecciones de eventos, usuarios y reservas, aplica las reglas de negocio y construye el resumen.
- `Program` presenta el menú y delega las operaciones al gestor.

Todos los valores monetarios se manejan con `decimal`.

## Conceptos de POO

- **Encapsulamiento:** las propiedades de los modelos son de solo lectura; los cupos y el estado de una reserva solo cambian mediante operaciones controladas. `Evento.ReservarCupos` impide cantidades inválidas o superiores a la disponibilidad.
- **Abstracción:** `Evento` establece los datos y el contrato `CalcularPrecioFinal()` comunes, sin representar un tipo concreto.
- **Herencia:** los cuatro tipos concretos extienden `Evento` y reutilizan sus datos y reglas de cupos.
- **Polimorfismo:** el catálogo contiene elementos `Evento`, pero cada instancia calcula su precio de forma distinta: cine mantiene el precio, concierto agrega 15 %, taller descuenta 10 % y evento deportivo agrega 8 %.
- **Asociación y composición:** una reserva referencia un usuario y un evento; el gestor contiene y administra las listas de objetos.
- **Colecciones y excepciones:** `GestorReservas` utiliza `List<T>` internamente y expone vistas de solo lectura; las reglas inválidas producen excepciones que el menú presenta sin terminar el proceso.

## Diagrama UML

```mermaid
classDiagram
    class Evento {
        <<abstract>>
        +int Id
        +string Nombre
        +DateTime Fecha
        +decimal PrecioBase
        +int CapacidadMaxima
        +int CuposDisponibles
        +CalcularPrecioFinal() decimal
        +ReservarCupos(cantidad) void
        +DevolverCupos(cantidad) void
    }
    class Cine
    class Concierto
    class Taller
    class EventoDeportivo
    class Usuario {
        +int Id
        +string Nombre
    }
    class Reserva {
        +int Id
        +int CantidadEntradas
        +decimal ValorTotal
        +EstadoReserva Estado
    }
    class GestorReservas {
        -List~Evento~ eventos
        -List~Usuario~ usuarios
        -List~Reserva~ reservas
        +CrearReserva(nombre, eventoId, cantidad) Reserva
        +CancelarReserva(reservaId) void
        +BuscarReservasPorUsuario(nombre) List~Reserva~
        +ObtenerResumen() ResumenReservas
    }
    Evento <|-- Cine
    Evento <|-- Concierto
    Evento <|-- Taller
    Evento <|-- EventoDeportivo
    Usuario "1" <-- "0..*" Reserva : realiza
    Evento "1" <-- "0..*" Reserva : reservado en
    GestorReservas "1" o-- "0..*" Evento : administra
    GestorReservas "1" o-- "0..*" Usuario : registra
    GestorReservas "1" o-- "0..*" Reserva : gestiona
```

## Sustentación

Utilicé una clase abstracta `Evento` porque Cine, Concierto, Taller y EventoDeportivo comparten identificador, nombre, fecha, precio base, capacidad y reglas de cupos. Así, `GestorReservas` puede almacenarlos juntos en `List<Evento>` y operar con todos mediante la misma interfaz.

La diferencia de precio se resuelve con el método polimórfico `CalcularPrecioFinal()`: `Cine` retorna `PrecioBase`, `Concierto` aplica `1.15m` y `Taller` aplica `0.90m`. Para agregar un tipo nuevo como `EventoDeportivo`, basta con heredar de `Evento` e implementar su regla (en este caso `1.08m`); el gestor y los tipos existentes no necesitan cambios para calcular el precio. Esto evita duplicar los datos y reglas comunes y facilita extender el catálogo.

## Evidencia

Menu

<img width="265" height="203" alt="{84D869C3-41A8-4B9A-A040-4B630EA07FA7}" src="https://github.com/user-attachments/assets/b656963a-a903-4a68-b117-045caa344f70" />


Eventos Disponibles


<img width="726" height="389" alt="{6CAC0861-DB70-408A-8C5B-8A605384059C}" src="https://github.com/user-attachments/assets/6aa29ce6-d4b7-4512-a84b-7a8a340d9185" />


Consultar Eventos


<img width="310" height="369" alt="{F051D3AF-AD89-44D9-B2B0-5063CBCA1620}" src="https://github.com/user-attachments/assets/8b139f5a-f3c3-41a9-ab43-cde82e8fa1a1" />


Reservar cupos


<img width="457" height="346" alt="{1FC5D5EC-1F5D-4EC7-AC69-635255BE371C}" src="https://github.com/user-attachments/assets/753f78f8-6dce-4464-bc9d-ff53a147f6a3" />


Cancelacion de cupo


<img width="493" height="305" alt="{4046AD10-AF09-409D-899B-01338A0031CA}" src="https://github.com/user-attachments/assets/a633682c-cbc5-47c1-a5b8-cd6de703ce4d" />


Resumen


<img width="365" height="326" alt="{73415E3F-A972-4E7A-9748-B45245190DE3}" src="https://github.com/user-attachments/assets/fc1e531a-862b-4b3a-9927-3ae6627a4ca7" />




## Estructura

```text
PLANAZO-POO-CSharp/
├── Modelos/
├── Servicios/
├── Pruebas/
├── Capturas/
├── Program.cs
├── README.md
└── .gitignore
```
