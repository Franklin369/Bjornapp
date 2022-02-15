using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bjornapp.Modelo;
using Bjornapp.VistasModelo;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Acr.UserDialogs;

namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MiPerfil : ContentPage
    {
        public MiPerfil()
        {
            InitializeComponent();
        }
        string iduser;
        string correo;
        string idnegocio;
        MediaFile file;
        string rutafoto;
        string rutafotoComparar;
        bool Actualizar=true;
        private async Task obtenerIdUser()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                iduser = savedfirebaseauth.User.LocalId;
                correo = savedfirebaseauth.User.Email;
                txtcorreo.Text = correo;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await App.Current.MainPage.DisplayAlert("Alerta", "Oh no !  sesion expirada", "OK");
            }
        }
        protected override async void OnAppearing()
        {
            if(Actualizar ==true)
            {
                await obtenerIdUser();
                await ObtenerdatosNegocio();
                Actualizar = false;
            }
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.MiPerfil();
            
        }
        private async Task ObtenerdatosNegocio()
        {
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idusuario = iduser;
            var dt = await funcion.MostrarNegociosPorIduser(parametros);
            foreach (var data in dt)
            {
                txtNombres.Text = data.nombre;
                foto.Source = data.foto;
                idnegocio = data.idnegocio;
                rutafoto = data.foto;
                rutafotoComparar = data.foto;
            }
            UserDialogs.Instance.HideLoading();
        }

        private async void btnsubifoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                Actualizar = false;
                await CrossMedia.Current.Initialize();
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null)
                    return;

                foto.Source = ImageSource.FromStream(() =>
                 {
                     var imageStream = file.GetStream();
                     rutafotoComparar = imageStream.ToString();
                   
                     return imageStream;
                 });
                if (file!=null)
                {
                   await GuardarDatos();
                }

            }
            catch (Exception)
            {

                await DisplayAlert("Alerta", "Vuelva a intentarlo", "OK");
            }
        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {

            await GuardarDatos();
            Actualizar = false;
        }
        private async Task GuardarDatos()
        {
            if (!string.IsNullOrEmpty(txtNombres.Text))
            {
                if (rutafoto == rutafotoComparar)
                {
                    UserDialogs.Instance.ShowLoading("Guardando cambios...");
                    await EditarNegociosBasico();

                }
                else
                {
                    await EliminarFotoStorage();
                    await SubirFotoStorage();
                    await EditarNegociosBasico();
                }

            }
            else
            {
                await DisplayAlert("Alerta", "Ingrese un nombre", "OK");
            }
        }
        private async Task SubirFotoStorage()
        {
            UserDialogs.Instance.ShowLoading("Guardando foto...");
            var funcion = new VMnegocios();
            rutafoto = await funcion.SubirImagenStorage(file.GetStream(), iduser);
        }
        private async Task EliminarFotoStorage()
        {
            if (rutafoto != "-")
            {
                VMnegocios funcion = new VMnegocios();
                await funcion.EliminarFotoStorage(iduser + ".jpg");
            }

        }
        private async Task EditarNegociosBasico()
        {
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idnegocio = idnegocio;
            parametros.nombre = txtNombres.Text;
            parametros.foto = rutafoto;
            await funcion.EditarNegociosBasico(parametros);
            UserDialogs.Instance.HideLoading();
        }

        private void txtNombres_TextChanged(object sender, TextChangedEventArgs e)
        {
            Actualizar = true;
        }
    }
}