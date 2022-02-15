using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Bjornapp.VistasModelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btncrearcuenta_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new CrearCorreo());
        }
        protected override void OnAppearing()
        {
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Login();

        }
        private async void btniniciar_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtlogin.Text))
            {
                if (!string.IsNullOrEmpty(txtcontraseña.Text))
                {
                    UserDialogs.Instance.ShowLoading("Validando datos...");
                    await validarDatos();
                }
                else
                {
                    await DisplayAlert("Alerta", "Ingrese su contraseña", "OK");
                }
            }
            else
            {
                await DisplayAlert("Alerta", "Ingrese su correo", "OK");
            }


        }
        private async Task validarDatos()
        {
            bool estado;

            var funcion = new VMcrearcuenta();
            estado = await funcion.ValidarCuenta(txtlogin.Text, txtcontraseña.Text);
            if (estado == true)
            {
                UserDialogs.Instance.HideLoading();
                Application.Current.MainPage = new NavigationPage(new Contenedor());
            }
            else
            {
                UserDialogs.Instance.HideLoading();
            }





        }

        private void txtlogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            animacion.Progress = (float)e.NewTextValue.Length / 10;
        }

        private void txtcontraseña_TextChanged(object sender, TextChangedEventArgs e)
        {
            animacion.Progress = (float)e.NewTextValue.Length / 10;

        }
    }
}