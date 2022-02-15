using System;
using System.Collections.Generic;
using System.Text;

namespace Bjornapp.Modelo
{
   public class Mnegocios
    {
        public string idnegocio { get; set; } 
        public string  celular { get; set; }
        public string descripcion { get; set; }
        public string direccion { get; set; }
        public string foto { get; set; }
        public string idcategoria { get; set; }
        public string idlocalidad { get; set; }
        public string idusuario { get; set; }
        public string nombre { get; set; }
        public string prioridad { get; set; }
        //Parametros de localidades
        public string localidad { get; set; }
        //Parametros de reseñas
        public string contCalificaciones { get; set; }
        public string calificacion { get; set; }
        public string calificacionLabel { get; set; }


    }
}
