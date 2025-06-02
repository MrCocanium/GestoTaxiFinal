using System;
using Newtonsoft.Json;

namespace ProyectoFinalDAM
{
    public class Lugar
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        [JsonIgnore]
        public string FirebaseID { get; set; }

        public Lugar()
        {
        }

        public Lugar(int id, string descripcion, double latitud, double longitud)
        {
            ID = id;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Latitud = latitud;
            Longitud = longitud;
        }

        public bool CoordenadasValidas()
        {
            return !double.IsNaN(Latitud) && !double.IsInfinity(Latitud) &&
                   !double.IsNaN(Longitud) && !double.IsInfinity(Longitud);
        }

        public override string ToString()
        {
            return $"{Descripcion} ({Latitud}, {Longitud})";
        }
    }
}