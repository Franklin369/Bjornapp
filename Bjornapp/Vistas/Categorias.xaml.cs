using Bjornapp.VistasModelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Categorias : ContentPage
    {
        public Categorias()
        {
            InitializeComponent();
          
        }
        public static bool Actualizar = true;
        string Idcategoria;
        private async Task mostrarCategorias()
        {
            UserDialogs.Instance.ShowLoading("Actualizando...");
            var funcion = new VMcategorias();
            var dt = await funcion.MostrarCategoriasNormal();
            var dtTop = await funcion.MostrarCategoriasTop();
            listaCategoriasNormal.ItemsSource = dt;
            listaCategoriasTop.ItemsSource = dtTop;
            UserDialogs.Instance.HideLoading();
        }
        protected override async void OnAppearing()
        {
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Categorias();
            //if (Actualizar==true)
            //{
               await  mostrarCategorias();
               Actualizar = false;
            //}
        
        }

        private async void btnCategoriaTop_Clicked(object sender, EventArgs e)
        {
            Idcategoria = (sender as ImageButton).CommandParameter.ToString();
            Negocios.Idcategoria = Idcategoria;
            await Navigation.PushAsync(new Negocios());
        }

        private async void btnCategoriasNormal_Clicked(object sender, EventArgs e)
        {
            Idcategoria = (sender as ImageButton).CommandParameter.ToString();
            Negocios.Idcategoria = Idcategoria;
            await Navigation.PushAsync(new Negocios());
        }
    }
}