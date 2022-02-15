using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Linq;
using Bjornapp.Modelo;
namespace Bjornapp.VistasModelo
{
   public  class VMresenias
    {
        int Contador;
        int Suma;
        public async Task<List<Mresenias>>MostrarReseñas(Mresenias parametrosPedir)
        {
            var Resenias = new List<Mresenias>();
            var data = (await Constantes.firebase
                .Child("Resenias")
                .OrderByKey()
                .OnceAsync<Mresenias>()).Where(a => a.Object.idnegocio == parametrosPedir.idnegocio);
                foreach (var dt in data)
            {
                var parametros = new Mresenias();
                parametros.reseña = dt.Object.reseña;
                parametros.calificacion = dt.Object.calificacion;
                parametros.idcalificacion = dt.Key;
                Resenias.Add(parametros);
            }
            return Resenias;
        }
        public async Task<List<Mresenias>> MostrarReseñasXIdusuario(Mresenias parametrosPedir)
        {
            var Resenias = new List<Mresenias>();
            var data = (await Constantes.firebase
                .Child("Resenias")
                .OrderByKey()
                .OnceAsync<Mresenias>()).Where(a => a.Object.idusuario == parametrosPedir.idusuario);
            foreach (var dt in data)
            {
                var parametros = new Mresenias();
                parametros.reseña = dt.Object.reseña;
                parametros.calificacion = dt.Object.calificacion;
                parametros.idcalificacion = dt.Key;
                Resenias.Add(parametros);
            }
            return Resenias;
        }
        public async Task <List<Mresenias>>MostrarReseñasConFoto(Mresenias parametrosPedir)
        {
            var ListaReseñas = new List<Mresenias>();
            var data = (await Constantes.firebase
                .Child("Resenias")
                .OrderByKey()
                .OnceAsync<Mresenias>()).Where(a => a.Object.idnegocio == parametrosPedir.idnegocio);
            foreach (var dt in data)
            {
                var parametros = new Mresenias();
                parametros.reseña = dt.Object.reseña;
                parametros.calificacion = dt.Object.calificacion;
                parametros.idusuario = dt.Object.idusuario;
                var funcionNegocios = new VMnegocios();
                var parametrosNegocios = new Mnegocios ();
                parametrosNegocios.idusuario = parametros.idusuario;
                var listaNegocios = await funcionNegocios.MostrarNegociosPorIduser(parametrosNegocios);
                foreach (var dtNegocios in listaNegocios)
                {
                    parametros.foto = dtNegocios.foto;
                    parametros.nombre = dtNegocios.nombre;
                }
                ListaReseñas.Add(parametros);
            }

            return ListaReseñas;
        }
        public async Task<List<Mresenias>> ContarReseniasxNegocio(Mresenias parametros)
        {
            var Resenias = new List<Mresenias>();
            var data = (await Constantes.firebase
                .Child("Resenias")
                .OrderByKey()
                .OnceAsync<Mresenias>()).Where(a => a.Object.idnegocio == parametros.idnegocio);
           foreach (var dt in data)
            {
                parametros.calificacion = dt.Object.calificacion;
                Resenias.Add(parametros);
                Resenias.Count();
            }
            return Resenias;
        }
       public async Task InsertarReseña(Mresenias parametros)
        {
            await Constantes.firebase
                .Child("Resenias")
                .PostAsync(new Mresenias()
                {
                    calificacion = parametros.calificacion,
                    idnegocio = parametros.idnegocio,
                    idusuario = parametros.idusuario,
                    reseña = parametros.reseña
                }) ;
        }
        public async Task EditarReseña(Mresenias parametros)
        {
            var data = (await Constantes.firebase
                .Child("Resenias")
                .OnceAsync<Mresenias>()).Where(a => a.Key == parametros.idcalificacion).FirstOrDefault();
            data.Object.reseña = parametros.reseña;
            data.Object.calificacion = parametros.calificacion;
            await Constantes.firebase
                .Child("Resenias")
                .Child(data.Key)
                .PutAsync(data.Object);
        }

    }
}
