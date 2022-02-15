using Bjornapp.Modelo;
using Bjornapp.VistasModelo;
using Firebase.Auth;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrearCorreo : ContentPage
    {
        public CrearCorreo()
        {
            InitializeComponent();
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.CrearCuenta();
            CerrarSesion();
        }
        MediaFile file;
        string Idusuario;
        string rutafoto;
        private async void btnCrearcuenta_Clicked(object sender, EventArgs e)
        {
            if (file != null)
            {
                if (!string.IsNullOrEmpty(txtnombre.Text))
                {
                    if (!string.IsNullOrEmpty(txtCorreo.Text))
                    {
                        if (!string.IsNullOrEmpty(txtContraseña.Text))
                        {
                            await Crearcuenta();
                            await IniciarSesion();
                            await ObtenerIdusuario();
                            await SubirFotoStorage();
                            await InsertarNegocios();
                        }
                        else
                        {
                            await DisplayAlert("Alerta", "Agregue una contraseña", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Agregue un correo", "OK");
                    }


                }
                else
                {
                    await DisplayAlert("Alerta", "Agregue un nombre", "OK");
                }

            }
            else
            {
                await DisplayAlert("Alerta", "Agregue una imagen", "OK");
            }


        }
        private void CerrarSesion()
        {
            Preferences.Remove("MyFirebaseRefreshToken");
        }
        private async Task Crearcuenta()
        {
            UserDialogs.Instance.ShowLoading("Creando cuenta...");
            var funcion = new VMcrearcuenta();
            await funcion.Crearcuenta(txtCorreo.Text, txtContraseña.Text);
        }
        private async Task IniciarSesion()
        {
            var funcion = new VMcrearcuenta();
            await funcion.ValidarCuenta(txtCorreo.Text, txtContraseña.Text);
        }
        private async Task ObtenerIdusuario()
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
                var guardarId = JsonConvert.DeserializeObject<FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefrescarContenido = await authProvider.RefreshAuthAsync(guardarId);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefrescarContenido));
                Idusuario = guardarId.User.LocalId;
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Sesion expirada", "OK");
            }


        }
        private async Task SubirFotoStorage()
        {
            var funcion = new VMnegocios();
            rutafoto = await funcion.SubirImagenStorage(file.GetStream(), Idusuario);
        }
        private async Task InsertarNegocios()
        {
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idusuario = Idusuario;
            parametros.idcategoria = "-";
            parametros.celular = "-";
            parametros.descripcion = "-";
            parametros.direccion = "-";
            parametros.foto = rutafoto;
            parametros.nombre = txtnombre.Text;
            parametros.idlocalidad = "-";
            parametros.prioridad = "0";
            await funcion.InsertarNegocios(parametros);
            UserDialogs.Instance.HideLoading();
            Application.Current.MainPage = new NavigationPage(new Contenedor());
        }
        private async void btnSubirfoto_Clicked(object sender, EventArgs e)
        {
         
            await CrossMedia.Current.Initialize();
            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null)
                    return;
                Fotoperfil.Source = ImageSource.FromStream(file.GetStream);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");

            }
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}