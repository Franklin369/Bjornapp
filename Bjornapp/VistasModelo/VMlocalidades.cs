using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Linq;
using Bjornapp.Modelo;

namespace Bjornapp.VistasModelo
{
    public class VMlocalidades
    {
        public async Task<List<Mlocalidades>> Mostrarlocalidades()
        {
            var Listado = new List<Mlocalidades>();
            var data = (await Constantes.firebase
                .Child("Localidad")
                .OrderByKey()
                .OnceAsync<Mlocalidades>()).Where(a => a.Object.partepais != "-");
            foreach (var dt in data)
            {
                Mlocalidades parametros = new Mlocalidades();
                parametros.partepais = dt.Object.partepais;
                parametros.idlocalidad = dt.Key;
                Listado.Add(parametros);
            }
            return Listado;

        }
        public async Task<List<Mlocalidades>> MostrarlocalidadesXid(string idlocalidad)
        {
            var Listado = new List<Mlocalidades>();
            var data = (await Constantes.firebase
                .Child("Localidad")
                .OrderByKey()
                .OnceAsync<Mlocalidades>()).Where(a => a.Object.partepais != "-").Where(b => b.Key == idlocalidad);
            foreach (var dt in data)
            {
                Mlocalidades parametros = new Mlocalidades();
                parametros.partepais = dt.Object.partepais;
                parametros.idlocalidad = dt.Key;
                Listado.Add(parametros);
            }
            return Listado;

        }
        public async Task InsertarLocalidad(Mlocalidades parametros, string partePais)
        {
            int contador = 0;
            var data = (await Constantes.firebase
                .Child("Localidad")
                .OrderByKey()
                .OnceAsync<Mlocalidades>()).Where(a => a.Object.partepais == partePais);
            contador = data.Count();
            if (contador == 0)
            {
                await Constantes.firebase
                    .Child("Localidad")
                    .PostAsync(new Mlocalidades()
                    {
                        partepais = parametros.partepais
                    });
            }
        }
        public async Task<string> ObtenerIdlocalidad(string partepais)
        {
            string Idlocalidad = "-";
            var data = (await Constantes.firebase
    .Child("Localidad")
    .OrderByKey()
    .OnceAsync<Mlocalidades>()).Where(a => a.Object.partepais == partepais);
            foreach (var dt in data)
            {

                Idlocalidad = dt.Key;
            }
            return Idlocalidad;

        }
    }
}
