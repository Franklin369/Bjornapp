using Bjornapp.Modelo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Linq;

namespace Bjornapp.VistasModelo
{
    public class VMcategorias
    {
        public async Task InsertarCategorias(Mcategorias parametros)
        {

            await Constantes.firebase
                .Child("Categorias")
                .PostAsync(new Mcategorias()
                {
                    Categoria = parametros.Categoria,
                    Color = parametros.Color,
                    Foto = parametros.Foto,
                    Prioridad = parametros.Prioridad
                });
        }
        public async Task<List<Mcategorias>> MostrarCategoriasNormal()
        {
            var Categorias = new List<Mcategorias>();
            var funcionNegocios = new VMnegocios();
            var parametrosNegocios = new Mnegocios();
            var data = (await Constantes.firebase
                .Child("Categorias")
                .OrderByKey()
                .OnceAsync<Mcategorias>()).Where(a => a.Object.Prioridad == "0").Where(b => b.Object.Categoria != "-");
            foreach (var dt in data)
            {
                Mcategorias parametros = new Mcategorias();
                parametros.Categoria = dt.Object.Categoria;
                parametros.Foto = dt.Object.Foto;
                parametros.Idcategoria = dt.Key;
                parametros.Color = dt.Object.Color;
                parametrosNegocios.idcategoria = dt.Key;

                int ContadorNegocios = await funcionNegocios.ContarNegociosxCategoria(parametrosNegocios);
                parametros.contador = "(" + ContadorNegocios + ")" + " Disponibles";
                Categorias.Add(parametros);
            }
            return Categorias;
        }
        public async Task<List<Mcategorias>> MostrarCategoriasTop()
        {
            var Categorias = new List<Mcategorias>();
            var funcionNegocios = new VMnegocios();
            var parametrosNegocios = new Mnegocios();
            var data = (await Constantes.firebase
                .Child("Categorias")
                .OrderByKey()
                .OnceAsync<Mcategorias>()).Where(a => a.Object.Prioridad == "1").Where(b => b.Object.Categoria != "-");
            foreach (var dt in data)
            {
                Mcategorias parametros = new Mcategorias();
                parametros.Categoria = dt.Object.Categoria;
                parametros.Foto = dt.Object.Foto;
                parametros.Idcategoria = dt.Key;
                parametros.Color = dt.Object.Color;
                parametrosNegocios.idcategoria = dt.Key;

                int ContadorNegocios = await funcionNegocios.ContarNegociosxCategoria(parametrosNegocios);
                parametros.contador = "(" + ContadorNegocios + ")" + " Disponibles";
                Categorias.Add(parametros);
            }
            return Categorias;
        }
        public async Task<List<Mcategorias>> MostrarCategorias()
        {
            var Categorias = new List<Mcategorias>();
            var funcionNegocios = new VMnegocios();
            var parametrosNegocios = new Mnegocios();
            var data = (await Constantes.firebase
                .Child("Categorias")
                .OrderByKey()
                .OnceAsync<Mcategorias>()).Where(b => b.Object.Categoria != "-");
            foreach (var dt in data)
            {
                Mcategorias parametros = new Mcategorias();
                parametros.Categoria = dt.Object.Categoria;
                parametros.Foto = dt.Object.Foto;
                parametros.Idcategoria = dt.Key;
                parametros.Color = dt.Object.Color;
                parametrosNegocios.idcategoria = dt.Key;

                int ContadorNegocios = await funcionNegocios.ContarNegociosxCategoria(parametrosNegocios);
                parametros.contador = "(" + ContadorNegocios + ")" + " Disponibles";
                Categorias.Add(parametros);
            }
            return Categorias;
        }
        public async Task <string> ObtenerIdcategoria(Mcategorias parametros)
        {
            var data = (await Constantes.firebase
           .Child("Categorias")
           .OrderByKey()
           .OnceAsync<Mcategorias>()).Where(a => a.Object.Categoria == parametros.Categoria);
            string Idcategoria="-";
            foreach (var dt in data)
            {
                Idcategoria = dt.Key;
            }
            return Idcategoria;
        }
        public async Task<string> ObtenerCategorixId(Mcategorias parametros)
        {
            var data = (await Constantes.firebase
           .Child("Categorias")
           .OrderByKey()
           .OnceAsync<Mcategorias>()).Where(a => a.Key == parametros.Idcategoria);
            string Categoria = "-";
            foreach (var dt in data)
            {
                Categoria = dt.Object.Categoria;
            }
            return Categoria;
        }
    }
}
