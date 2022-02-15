using Bjornapp.Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Firebase.Storage;
using System.IO;
using Xamarin.Forms;
using System.Linq;

namespace Bjornapp.VistasModelo
{
    public class VMgaleriaTrab
    {
        string rutafoto;
        public async Task InsertarGaleriaTrab(MgaleriaTrab parametros)
        {

            await Constantes.firebase
              .Child("Galeriatrabajos")
              .PostAsync(new MgaleriaTrab()
              {
                  foto = parametros.foto,
                  idnegocio = parametros.idnegocio,
              });
            ;
        }

        public async Task<List<MgaleriaTrab>> ListarFotosTrabajos(MgaleriaTrab parametros)
        {
            var GaleriaTra = new List<MgaleriaTrab>();
            var data = (await Constantes.firebase
             .Child("Galeriatrabajos")
             .OrderByKey()
             .OnceAsync<MgaleriaTrab>()).Where(a => a.Object.idnegocio == parametros.idnegocio);
            foreach (var dino in data)
            {
                var parametro2 = new MgaleriaTrab();
                parametro2.foto = dino.Object.foto;
                parametro2.idnegocio = dino.Object.idnegocio;
                parametro2.idgaleria = dino.Key;
                GaleriaTra.Add(parametro2);

            }
            return GaleriaTra;
        }
        public async Task<List<MgaleriaTrab>> ObtenerIdgaleria(MgaleriaTrab parametros)
        {
            var galariafotos = new List<MgaleriaTrab>();
            var data = (await Constantes.firebase
             .Child("Galeriatrabajos")
             .OrderByKey()
             .OnceAsync<MgaleriaTrab>()).Where(a => a.Object.idnegocio == parametros.idnegocio);
            foreach (var dt in data)
            {

                parametros.idgaleria = dt.Key;
                galariafotos.Add(parametros);

            }
            return galariafotos;
        }
        public async Task<string> SubirImagenesStorage(Stream imageStream,MgaleriaTrab parametros)
        {
            var Imagen = await new FirebaseStorage(Constantes.storage)
                .Child("Galeriatrab")
                .Child(parametros.idgaleria + ".jpg")
                .PutAsync(imageStream);
            rutafoto = Imagen;
            return rutafoto;
        }
        public async Task EliminarGaleriaTrab(MgaleriaTrab parametros )
        {
            var dataEliminar = (await Constantes.firebase
                .Child("Galeriatrabajos")
                .OnceAsync<MgaleriaTrab>()).Where(a => a.Key == parametros.idgaleria).FirstOrDefault();
            await Constantes.firebase.Child("Galeriatrabajos").Child(dataEliminar.Key).DeleteAsync();
        }
        public async Task EliminarFotoStorage(string nombre)
        {
            await new FirebaseStorage(Constantes.storage)
                .Child("Galeriatrab")
                .Child(nombre)
                .DeleteAsync();
        }
        public async Task EditarGaleriaTrabajos(MgaleriaTrab parametros)
        {
            var data = (await Constantes.firebase
                .Child("Galeriatrabajos")
                .OnceAsync<MgaleriaTrab>()).Where(a => a.Key == parametros.idgaleria).FirstOrDefault();
            data.Object.foto = parametros.foto;
            await Constantes.firebase
                .Child("Galeriatrabajos")
                .Child(data.Key)
                .PutAsync(data.Object);

        }
    }
}
