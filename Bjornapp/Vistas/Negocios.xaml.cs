using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjornapp.VistasModelo;
using Bjornapp.Modelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bjornapp.Vistas.Configuraciones;
namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Negocios : ContentPage
    {
        public Negocios()
        {
            InitializeComponent();         
            Mostrarlocalidades();
        }
        protected override  async void OnAppearing()
        {
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Negocios();       
            await MostrarNegocios();
        }
        public static string idnegocio;
        public static string Idcategoria;
        string partepais;
        string idlocalidad;
        private async Task MostrarNegocios()
        {
            await ObtenerIdlocalidad();
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idcategoria = Idcategoria;
            parametros.idlocalidad = idlocalidad;
            var dtGratis = await funcion.MostrarNegociosGratis(parametros);
            var dtPremium = await funcion.MostrarNegociosPremium(parametros);

            listaNegociogratis.ItemsSource = dtGratis;
            listanegociosPago.ItemsSource = dtPremium;
        }

        private async void btnFoto1_Clicked(object sender, EventArgs e)
        {
            idnegocio = (sender as ImageButton).CommandParameter.ToString();
            VerPerfil.idnegocio = idnegocio;
            await Navigation.PushAsync(new VerPerfil());
        }

        private async void btnfoto2_Clicked(object sender, EventArgs e)
        {
            idnegocio = (sender as ImageButton).CommandParameter.ToString();
            VerPerfil.idnegocio = idnegocio;
            await Navigation.PushAsync(new VerPerfil());
        }

        private  async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void PickerUbicacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            partepais = PickerUbicacion.SelectedItem.ToString();     
            await ObtenerIdlocalidad();
            await MostrarNegocios();
        }
        private async Task Mostrarlocalidades()
        {
            var funcion = new VMlocalidades();
            var dt = await funcion.Mostrarlocalidades();
            foreach (var data in dt)
            {
                PickerUbicacion.Items.Add(data.partepais.ToString());
                PickerUbicacion.SelectedItem = data.partepais;
                partepais = data.partepais;
            }
          
        }
        private async Task ObtenerIdlocalidad()
        {
            var funcion = new VMlocalidades();
            idlocalidad = await funcion.ObtenerIdlocalidad(partepais);
        }

        private async void btnAnunciate_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PagoPremium());
        }
    }
}