using System;

namespace ProyectoFinalDAM
{
    public class Factura
    {
        // Identificador único de la factura
        public string ID { get; set; }

        // Código de factura: FAC-FECHA-NOMBRETAXISTA
        public string CodigoFactura { get; set; }

        // Fechas
        public DateTime FechaEmision { get; set; }

        // Datos del servicio
        public decimal Kilometros { get; set; }
        public decimal HorasEspera { get; set; }

        // Precios por unidad
        private const decimal PrecioKilometro = 1.2m;
        private const decimal PrecioHoraEspera = 15.00m;

        // Propiedades calculadas
        public decimal Subtotal => (Kilometros * PrecioKilometro) + (HorasEspera * PrecioHoraEspera);
        public decimal IVA => Subtotal * 0.21m;
        public decimal Total => Subtotal + IVA;

        // Opcional: Información adicional
        public string NombreTaxista { get; set; }
        public string NombrePaciente { get; set; }
        public string LugarOrigen { get; set; }
        public string LugarDestino { get; set; }
    }
}