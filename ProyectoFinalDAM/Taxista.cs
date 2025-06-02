using Newtonsoft.Json;

namespace ProyectoFinalDAM
{
    public class Taxista
    {
        public string CCC { get; set; }
        public string DNI { get; set; }
        public string Direccion { get; set; }
        public int ID { get; set; } // Este campo puede ser opcional si Firebase genera IDs automáticamente
        public string Nombre { get; set; }
        public int NumeroTelefono { get; set; }
        public string Poblacion { get; set; }
        public string Provincia { get; set; }
        [JsonProperty("email")]  // Mapea el campo "email" de Firebase a la propiedad Email
        public string Email { get; set; }
    }
}