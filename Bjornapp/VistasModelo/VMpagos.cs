using System;
using System.Collections.Generic;
using System.Text;
using Bjornapp.Modelo;
using Firebase.Database.Query;
using System.Linq;
using Firebase.Storage;
using System.IO;
using System.Threading.Tasks;

namespace Bjornapp.VistasModelo
{
   public class VMpagos
    {
        string rutafoto;
        public async Task InsertarPagos(Mpagos parametros)
        {

            await Constantes.firebase
              .Child("Pagos")
              .PostAsync(new Mpagos()
              {
                  foto = parametros.foto,
                  idnegocio = parametros.idnegocio,
                  periodo = parametros.periodo,
                  comprobantepago = parametros.comprobantepago,
                  total = parametros.total,
                  fechafin = parametros.fechafin,
                  estado=parametros.estado 
                  
              });
            ;
        }
        public async Task<string> SubirImagenesStorage(Stream imageStream, string idnegocio)
        {
            var stroageImage = await new FirebaseStorage(Constantes.storage)
                .Child("Pagos")
                .Child(idnegocio + ".jpg")
                .PutAsync(imageStream);
            rutafoto = stroageImage;
            return rutafoto;

        }
        public async Task<List<Mpagos>> ListarPagos(Mpagos parametrosPedir)
        {
            var Pagos = new List<Mpagos>();
            {
                var data = (await Constantes.firebase
               .Child("Pagos")
               .OrderByKey()
               .OnceAsync<Mpagos>()).Where(a => a.Object.idnegocio == parametrosPedir.idnegocio);
                foreach (var dino in data)
                {
                    var parametros = new Mpagos();
                    parametros.periodo = dino.Object.periodo;
                    parametros.comprobantepago = dino.Object.comprobantepago;
                    parametros.total = dino.Object.total;
                    parametros.fechafin = dino.Object.fechafin;
                    parametros.estado = dino.Object.estado;
                    Pagos.Add(parametros);
                }
                return Pagos;
            }
        }
    }
}
