using System;

namespace ProyectoFinalDAM
{
    public class FacturaFirebase
    {
        // Constructor vacío necesario para Firebase
        public FacturaFirebase()
        {
        }

        // Identificador único de la factura (clave en Firebase)
        public string ID { get; set; }

        // Código de factura: FAC-FECHA-NOMBRETAXISTA
        public string CodigoFactura { get; set; }

        // Datos del taxista
        public string TaxistaID { get; set; }
        public string NombreTaxista { get; set; }

        // Fecha de emisión de la factura
        public string FechaFacturaStr { get; set; } // Formato: dd/MM/yyyy

        // Datos del servicio
        public double KilometrosTotales { get; set; }
        public double HorasEsperaTotales { get; set; }

        // Total calculado
        public double Total { get; set; }
    }
}