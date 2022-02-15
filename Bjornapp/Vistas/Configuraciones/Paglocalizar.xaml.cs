using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;
using Bjornapp.VistasModelo;
using Bjornapp.Modelo;
namespace Bjornapp.Vistas.Configuraciones
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Paglocalizar : ContentPage
    {
        public Paglocalizar()
        {
            InitializeComponent();
            punto = new Pin()
            {
                Label = "Tu ubicación",
                Type= PinType.Place,
                Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("punto.png") : BitmapDescriptorFactory.FromView(new Image() {Source="punto.png",WidthRequest=32,HeightRequest=32 }),
            Position=new Position(0,0),
            IsDraggable=true 
            };
            map.Pins.Add(punto);

        }
        Pin punto = new Pin();
        public static double latitud=0;
        public static double longitud=0;
        string ciudad;
        string pais;
        string Idlocalidad;
        protected override async void OnAppearing()
        {
            if (latitud ==0 && longitud==0)
            {
                await LocalizacionActual();
            }
            else
            {
                await LocalizarconDatos();
            }
        }
       private async Task LocalizarconDatos()
        {
            try
            {
                var posicion = new Position(latitud, longitud);
                punto.Position = new Position(latitud, longitud);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(posicion, Distance.FromMeters(500)));
                await ObtenerCiudadPais();
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Active su GPS", "OK");
                await Navigation.PopAsync();

            }
        }
        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async  void btnGuardar_Clicked(object sender, EventArgs e)
        {
           if (ciudad!=""&& pais !="")
            {
                if (pais=="Perú")
                {
                 await  InsertarLocalidades();
                    ConfigAnunciante.Idlocalizacion = Idlocalidad;
                 ConfigAnunciante.DireccionLatLong = latitud + "|" + longitud;
                ConfigAnunciante.ciudadPais = lblUbicacion.Text;
                await Navigation.PopAsync();
                }
                
            }
            else
            {
                await DisplayAlert("Sin datos", "Ubique una localización valida", "Ok");
            }
        }
        private async Task InsertarLocalidades()
        {
            var funcion = new VMlocalidades();
            var parametros = new Mlocalidades();
            parametros.partepais = lblUbicacion.Text;
            await funcion.InsertarLocalidad(parametros, lblUbicacion.Text);
            await ObtenerIdLocalidad();
        }
        private async Task ObtenerIdLocalidad()
        {
            var funcion = new VMlocalidades();
            Idlocalidad = await funcion.ObtenerIdlocalidad(lblUbicacion.Text);
        }
        private async void map_PinDragEnd(object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {
            var positions = new Position(e.Pin.Position.Latitude, e.Pin.Position.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(positions, Distance.FromMeters(500)));
            latitud = e.Pin.Position.Latitude;
            longitud = e.Pin.Position.Longitude;
            await ObtenerCiudadPais();
        }
        public async Task LocalizacionActual()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });


                 }
                if (location == null)
                {
                    await DisplayAlert("Alerta", "Sin acceso al GPS", "OK");
                }
                else
                {
                    latitud = location.Latitude;
                    longitud = location.Longitude;
                    var posicion = new Position(latitud, longitud);
                    punto.Position = new Position(latitud, longitud);
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(posicion, Distance.FromMeters(500)));
                    await ObtenerCiudadPais();
                }

            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Sin acceso al GPS", "OK");
            }
        }
        private async Task ObtenerCiudadPais()
        {
            try
            {
                var data = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var dataprocesada = data?.FirstOrDefault();
                if (dataprocesada !=null)
                {
                    ciudad = dataprocesada.SubAdminArea;
                    pais = dataprocesada.CountryName;
                    lblUbicacion.Text = dataprocesada.SubAdminArea + "-" + dataprocesada.CountryName;
                }
            }
            catch (Exception)
            {

               
            }
        }

    }
}