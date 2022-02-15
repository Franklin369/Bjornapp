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
    public class VMnegocios
    {
        string rutafoto;
        int contador;
        public async Task InsertarNegocios(Mnegocios parametros)
        {
            await Constantes.firebase
                .Child("Negocios")
                .PostAsync(new Mnegocios()
                {
                    celular = parametros.celular,
                    descripcion = parametros.descripcion,
                    direccion = parametros.direccion,
                    foto = parametros.foto,
                    nombre = parametros.nombre,
                    idusuario = parametros.idusuario,
                    idlocalidad = parametros.idlocalidad,
                    idcategoria =parametros.idcategoria ,
                    prioridad = parametros.prioridad
                });
        }

        public async Task<string> SubirImagenStorage(Stream imageStrem, string Idusuario)
        {

            var Imagen = await new FirebaseStorage(Constantes.storage)
                .Child("Negocios")
                .Child(Idusuario + ".jpg")
                .PutAsync(imageStrem);
            rutafoto = Imagen;
            return rutafoto;
        }
        public async Task EliminarFotoStorage(string nombre)
        {
            await new FirebaseStorage(Constantes.storage)
                 .Child("Negocios")
                 .Child(nombre)
                 .DeleteAsync();

        }
        public async Task<int> ContarNegociosxCategoria(Mnegocios parametros)
        {
            var data = (await Constantes.firebase
                .Child("Negocios")
                .OrderByKey()
                .OnceAsync<Mnegocios>()).Where(a => a.Object.idcategoria == parametros.idcategoria);
            int total = 0;
            total = data.Count();
            contador = total;
            return contador;
        }
        public async Task<List<Mnegocios>> MostrarNegociosGratis(Mnegocios parametrosPedir)
        {
            var funcionLocalidades = new VMlocalidades();
            var Negocios = new List<Mnegocios>();
            var funcionResenia= new VMresenias();
       

            var data = (await Constantes.firebase
                .Child("Negocios")
                .OrderByKey()
                .OnceAsync<Mnegocios>()).Where(a => a.Object.idcategoria == parametrosPedir.idcategoria).Where(b => b.Object.prioridad == "0").Where(c => c.Object.idlocalidad == parametrosPedir.idlocalidad).Where (d =>d.Object.nombre !="-");
            foreach (var dt in data)
            {
                var parametros = new Mnegocios();
                var parametrosResenias = new Mresenias();
                parametros.nombre = dt.Object.nombre;
                parametros.foto = dt.Object.foto;
                parametros.idnegocio = dt.Key;
                parametros.idlocalidad = dt.Object.idlocalidad;
                //Obtenemos las localidades
                string localidad = "-";
                var dataLocalidades = await funcionLocalidades.MostrarlocalidadesXid(parametros.idlocalidad);
                foreach (var dtLocalidades in dataLocalidades)
                {
                    localidad = dtLocalidades.partepais;
                }
                //****
                //Obtenemos el promedio de las reseñas
                parametrosResenias.idnegocio = dt.Key;
                var dataResenias = await funcionResenia.ContarReseniasxNegocio(parametrosResenias);
                double Promedio = 0;
                int contador = dataResenias.Count();
                foreach (var dtResenias in dataResenias)
                {
                    Promedio += (Convert.ToDouble(dtResenias.calificacion)) / contador;
                }
                //********
                parametros.contCalificaciones ="("+ contador.ToString()+")";
                parametros.calificacion = Math.Truncate(Promedio).ToString();
                parametros.calificacionLabel = "(" + string.Format("{0:####.#}", Promedio) + ")";
                parametros.localidad = localidad;
                Negocios.Add(parametros);

            }
            return Negocios;
        }
        public async Task<List<Mnegocios>> MostrarNegociosPremium(Mnegocios parametros)
        {
            var funcionLocalidades = new VMlocalidades();
            var Negocios = new List<Mnegocios>();
            var funcionResenia = new VMresenias();


            var data = (await Constantes.firebase
                .Child("Negocios")
                .OrderByKey()
                .OnceAsync<Mnegocios>()).Where(a => a.Object.idcategoria == parametros.idcategoria).Where(b => b.Object.prioridad == "1").Where(c => c.Object.idlocalidad == parametros.idlocalidad).Where(d => d.Object.nombre != "-");
            foreach (var dt in data)
            {
                var parametrosResenias = new Mresenias();
                parametros.nombre = dt.Object.nombre;
                parametros.foto = dt.Object.foto;
                parametros.idnegocio = dt.Key;
                //Obtenemos las localidades
                string localidad = "-";
                var dataLocalidades = await funcionLocalidades.MostrarlocalidadesXid(parametros.idlocalidad);
                foreach (var dtLocalidades in dataLocalidades)
                {
                    localidad = dtLocalidades.partepais;
                }
                //****
                //Obtenemos el promedio de las reseñas
                parametrosResenias.idnegocio = dt.Key;
                var dataResenias = await funcionResenia.ContarReseniasxNegocio(parametrosResenias);
                double Promedio = 0;
                int contador = dataResenias.Count();
                foreach (var dtResenias in dataResenias)
                {
                    Promedio += (Convert.ToDouble(dtResenias.calificacion)) / contador;
                }
                //********
                parametros.contCalificaciones = "(" + contador.ToString() + ")";
                parametros.calificacion = Math.Truncate(Promedio).ToString();
                parametros.calificacionLabel = "(" + string.Format("{0:####.#}", Promedio) + ")";
                parametros.localidad = localidad;
                Negocios.Add(parametros);

            }
            return Negocios;
        }
        public async Task<List<Mnegocios>> MostrarNegociosPorIduser(Mnegocios parametrosPedir)
        {     
            var Negocios = new List<Mnegocios>();
            var data = (await Constantes.firebase
                .Child("Negocios")
                .OrderByKey()
                .OnceAsync<Mnegocios>()).Where(a => a.Object.idusuario == parametrosPedir.idusuario);
            foreach (var dt in data)
            {
                var parametros = new Mnegocios();
                parametros.foto = dt.Object.foto;
                parametros.nombre = dt.Object.nombre;
                parametros.celular = dt.Object.celular;
                parametros.direccion = dt.Object.direccion;
                parametros.idcategoria = dt.Object.idcategoria;
                parametros.idlocalidad = dt.Object.idlocalidad;
                parametros.idnegocio = dt.Key;
                parametros.descripcion = dt.Object.descripcion;
                parametros.prioridad = dt.Object.prioridad;
                Negocios.Add(parametros);

            }
            return Negocios;
        }
        public async Task EditarNegocios(Mnegocios parametros)
        {
            var data = (await Constantes.firebase
                .Child("Negocios")
                .OnceAsync<Mnegocios>()).Where(a => a.Object.idusuario == parametros.idusuario).FirstOrDefault();
            data.Object.celular = parametros.celular;
            data.Object.descripcion = parametros.descripcion;
            data.Object.direccion = parametros.direccion;
            data.Object.idcategoria = parametros.idcategoria;
            data.Object.idlocalidad = parametros.idlocalidad;
            await Constantes.firebase
                .Child("Negocios")
                .Child(data.Key)
                .PutAsync(data.Object);
        }
        public async Task EditarNegociosBasico(Mnegocios parametros)
        {
            var data = (await Constantes.firebase
                .Child("Negocios")
                .OnceAsync<Mnegocios>()).Where(a => a.Key == parametros.idnegocio).FirstOrDefault();
           
            data.Object.nombre = parametros.nombre;
            data.Object.foto = parametros.foto;
          
            await Constantes.firebase
                .Child("Negocios")
                .Child(data.Key)
                .PutAsync(data.Object);
        }
        public async Task<List<Mnegocios>> VerPerfil(Mnegocios parametrosPedir)
        {
            var funcionLocalidades = new VMlocalidades();
            var Negocios = new List<Mnegocios>();
            var funcionResenia = new VMresenias();


            var data = (await Constantes.firebase
                .Child("Negocios")
                .OrderByKey()
                .OnceAsync<Mnegocios>()).Where(a => a.Key == parametrosPedir.idnegocio);
            foreach (var dt in data)
            {
                var parametros = new Mnegocios();
                var parametrosResenias = new Mresenias();
                parametros.nombre = dt.Object.nombre;
                parametros.foto = dt.Object.foto;
                parametros.idnegocio = dt.Key;
                parametros.celular = dt.Object.celular;
                parametros.descripcion = dt.Object.descripcion;
                parametros.idlocalidad = dt.Object.idlocalidad;
                parametros.direccion = dt.Object.direccion;
                //Obtenemos las localidades
                string localidad = "-";
                var dataLocalidades = await funcionLocalidades.MostrarlocalidadesXid(parametros.idlocalidad);
                foreach (var dtLocalidades in dataLocalidades)
                {
                    localidad = dtLocalidades.partepais;
                }
                //****
                //Obtenemos el promedio de las reseñas
                parametrosResenias.idnegocio = dt.Key;
                var dataResenias = await funcionResenia.ContarReseniasxNegocio(parametrosResenias);
                double Promedio = 0;
                int contador = dataResenias.Count();
                foreach (var dtResenias in dataResenias)
                {
                    Promedio += (Convert.ToDouble(dtResenias.calificacion)) / contador;
                }
                //********
                parametros.contCalificaciones = "(" + contador.ToString() + ")";
                parametros.calificacion = Math.Truncate(Promedio).ToString();
                parametros.calificacionLabel = "(" + string.Format("{0:####.#}", Promedio) + ")";
                parametros.localidad = localidad;
              
                Negocios.Add(parametros);

            }
            return Negocios;
        }

    }
}
