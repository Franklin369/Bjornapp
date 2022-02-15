using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using Bjornapp.VistasModelo;
using Bjornapp.Modelo;
using Xamarin.Essentials;
using Xamarin.Forms.OpenWhatsApp;
using Plugin.ExternalMaps;
using Rg.Plugins.Popup.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerPerfil : ContentPage
    {
        public VerPerfil()
        {
            InitializeComponent();
        }
        string celular;
        public static string idnegocio;
        string codigoPais;
        string DireccionLatLong;
        string reseña;
        string calificacion;
        string idcalificacion;
        string idusuario;
        protected override async void OnAppearing()
        {
            await ObtenerDatosPerfil();
            await MostrarFotosGal();
            await ObtenerIdusuario();
            await MostrarReseñas();
        }
        private async Task MostrarReseñas()
        {
            var funcion = new VMresenias();
            var parametros = new Mresenias();
            parametros.idnegocio = idnegocio;

            var dt = await funcion.MostrarReseñasConFoto(parametros);
            if (dt.Count >0)
            {
                listaCalificaciones.ItemsSource = dt;
            }
        }
        private async Task ObtenerIdusuario()
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
                var guardarId = JsonConvert.DeserializeObject<FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefrescarContenido = await authProvider.RefreshAuthAsync(guardarId);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefrescarContenido));
                idusuario = guardarId.User.LocalId;
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Sesion expirada", "OK");
            }


        }
        private async Task ObtenerDatosPerfil()
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idnegocio = idnegocio;
            var dt = await funcion.VerPerfil(parametros);
            foreach (var data in dt)
            {
                lblcelular.Text = data.celular;
                celular = data.celular;
                ImagenPerfil.Source = data.foto;
                lbldescripcion.Text = data.descripcion;
                lblUbicacion.Text = data.localidad;
                lblnombreTitulo.Text = data.nombre;
                lblnombresencund.Text = data.nombre;
                lblpuntuacion.Text = data.calificacionLabel;
                DireccionLatLong = data.direccion;
               
            }
           
        }
        private async Task MostrarFotosGal()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idnegocio = idnegocio;
            var dt = await funcion.ListarFotosTrabajos(parametros);
            CarouselGaleria.ItemsSource = dt;
            UserDialogs.Instance.HideLoading();
        }
        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void btnllamar_Clicked(object sender, EventArgs e)
        {
            PhoneDialer.Open(lblcelular.Text);
        }

        private async void btnCalificar_Clicked(object sender, EventArgs e)
        {
            var funcion = new VMresenias();
            var parametros = new Mresenias();
            parametros.idusuario = idusuario;
            var dt = await funcion.MostrarReseñasXIdusuario(parametros);
            if(dt.Count >0)
            {
                foreach (var data in dt)
                {
                    reseña = data.reseña;
                    calificacion = data.calificacion;
                    idcalificacion = data.idcalificacion;
                }
                Calificar.idcalificacion = idcalificacion;
                Calificar.idusuario = idusuario;
                Calificar.idnegocio = idnegocio;
                Calificar.reseña = reseña;
                Calificar.calificacion = calificacion;
                await PopupNavigation.Instance.PushAsync(new Calificar());
            }
            else
            {
               await  DisplayAlert("Idusaurio", idusuario, "OK");
                Calificar.idusuario = idusuario;
                Calificar.idnegocio = idnegocio;
                await PopupNavigation.Instance.PushAsync(new Calificar());
            }
        }

        private void btnLlamarSecun_Clicked(object sender, EventArgs e)
        {
            PhoneDialer.Open(lblcelular.Text);

        }

        private async void btnwtp_Clicked(object sender, EventArgs e)
        {
            try
            {
            codigoPais = "+51";
            string CodigoPaisCelular;
            CodigoPaisCelular = codigoPais + lblcelular.Text;
            Chat.Open(CodigoPaisCelular, "Mas informacion...");
            }
            catch (Exception)
            {

                await DisplayAlert("Alerta", "Numero invalido", "OK");
            }
           
        }

        private async void btnmapa_Clicked(object sender, EventArgs e)
        {
            string latitud;
            string longitud;
            string cadena = DireccionLatLong;
            string[] separadas = cadena.Split('|');
            latitud = separadas[0];
            longitud = separadas[1];
            var proceso = await CrossExternalMaps.Current.
                NavigateTo(lblnombreTitulo.Text, Convert.ToDouble(latitud), Convert.ToDouble(longitud));
        }
    }
}