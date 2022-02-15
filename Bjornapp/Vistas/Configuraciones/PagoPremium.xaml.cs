using Bjornapp.Modelo;
using Bjornapp.VistasModelo;
using Firebase.Auth;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;
using Xamarin.Forms.Xaml;

namespace Bjornapp.Vistas.Configuraciones
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PagoPremium : ContentPage
    {
        public PagoPremium()
        {
            InitializeComponent();
           
        }
        MediaFile file=null;
        string Ruta;
        string idnegocio;
        string Iduser;
        string total = "0";
        string fechafin;
        string estado="";
        string comprobante;
        protected override async void OnAppearing()
        {
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Pagos();
          await  obtenerIdUser();
        }
        private async Task obtenerIdUser()
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
                var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                Iduser = savedfirebaseauth.User.LocalId;
                await ObtenerdatosNegocio();
                await ObtenerdatosPago();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await App.Current.MainPage.DisplayAlert("Alerta", "Oh no !  sesion expirada", "OK");
            }
        }
        private async Task ObtenerdatosNegocio()
        {
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idusuario = Iduser;
            var dt = await funcion.MostrarNegociosPorIduser(parametros);
            foreach (var data in dt)
            {
                idnegocio = data.idnegocio;

            }
        }
        private async Task ObtenerdatosPago()
        {
            var funcion = new VMpagos();
            var parametros = new Mpagos();
            parametros.idnegocio = idnegocio;
            var dt = await funcion.ListarPagos(parametros);
            foreach (var data in dt)
            {
                total = data.total;
                fechafin = data.fechafin;
                comprobante = data.comprobantepago;
                estado = data.estado;
            }
            if (estado == "PENDIENTE")
            {
                frameCuenta.IsVisible = false;
                stackPlanes.IsVisible = false;
                foto.IsVisible = false;
                btnenviar.IsVisible = false;
                btnAgregarFotos.IsEnabled = false;
                lblEstado.Text = "Se envio tu pago, espera la respuesta de asistencia Bjorn";
            }
            else if (estado =="APROBADO" && total !="0")
            {
                frameCuenta.IsVisible = false;
                stackPlanes.IsVisible = false;
                foto.IsVisible = false;
                btnenviar.IsVisible = false;
                btnAgregarFotos.IsEnabled = false;
                lblEstado.Text = "Tu pago fue aceptado, estaras en destacados hasta el " + fechafin;

            }
            else if (estado=="RECHAZADO")
            {
                frameCuenta.IsVisible = true;
                stackPlanes.IsVisible = true;
                foto.IsVisible = true;
                btnenviar.IsVisible = true;
                btnAgregarFotos.IsEnabled = true;
                lblEstado.Text = "Tu pago fue rechazado por: "+comprobante+" ,Vuelve a enviar foto del Comp.Pago";

            }
        }


        private async void btnAgregarFotos_Clicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.Medium
                });
                if (file == null)
                {
                    return;

                }

            }
            catch (Exception)
            {

                await DisplayAlert("Alerta", "Vuelva a intentarlo", "OK");
            }
        }
        private async Task SubirImagenesStore()
        {
            var funcion = new VMpagos();
            Ruta = await funcion.SubirImagenesStorage(file.GetStream(), idnegocio);
        }
        private async Task Insertarpago()
        {
            var funcion = new VMpagos();
            var parametros = new Mpagos();
            parametros.foto = Ruta;
            parametros.idnegocio = idnegocio;
            parametros.periodo = "-";
            parametros.comprobantepago = "-";
            parametros.total = "SIN CONFIRMAR";
            parametros.fechafin = "-";
            parametros.estado = "PENDIENTE";
            await funcion.InsertarPagos(parametros);
            await DisplayAlert("Listo", "Foto enviada, vuelva aquí para obtener respuesta de aprobación", "OK");
            await Navigation.PopAsync();



        }

        private void btnWts_Clicked(object sender, EventArgs e)
        {
            Chat.Open("+51940308023", "Mas información...");
        }

        private void btnllamar_Clicked(object sender, EventArgs e)
        {
            PhoneDialer.Open("940308023");
        }

        private async void btnenviar_Clicked(object sender, EventArgs e)
        {
            if (file !=null)
            {
                await SubirImagenesStore();
                await Insertarpago();
            }
        }

        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}