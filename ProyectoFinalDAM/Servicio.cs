public class Servicio
{
    public string FirebaseID { get; set; } // 🔥 Clave única de Firebase (necesaria para actualizar/eliminar)
    public int ID { get; set; }
    public string Fecha { get; set; }
    public int Kilometros { get; set; }
    public int HorasEspera { get; set; }
    public string Taxista { get; set; } // Nombre o ID del taxista
    public string Paciente { get; set; } // Nombre o ID del paciente
    public string LugarOrigen { get; set; } // Nombre o ID del lugar
    public string LugarDestino { get; set; } // Nombre o ID del lugar
}