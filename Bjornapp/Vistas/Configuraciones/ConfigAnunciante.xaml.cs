using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bjornapp.VistasModelo;
using Bjornapp.Modelo;
using Acr.UserDialogs;
using Firebase.Auth;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Diagnostics;

namespace Bjornapp.Vistas.Configuraciones
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigAnunciante : ContentPage
    {
        public ConfigAnunciante()
        {
            InitializeComponent();
            ListarCategorias();
        }

        public static string ciudadPais;
        string iduser;
        string idnegocio;
        string categoria;
        string Idcategoria;
       public static string Idlocalizacion;
        public static string DireccionLatLong;
        string latitud;
        string longitud;
        MediaFile file;
        string idgaleria;
        string rutaFoto;
        public static bool Actualizar = true;
        protected override async void OnAppearing()
        {
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Negocios();
            if (Actualizar ==true)
            {
            
             await obtenerIdUser();
             Actualizar = false;
            }
           
        }
        private  void btncerrar_Clicked(object sender, EventArgs e)
        {
            Preferences.Remove("MyFirebaseRefreshToken");       
            Process.GetCurrentProcess().CloseMainWindow();
          
        }

        private async void btnlocalizar_Clicked(object sender, EventArgs e)
        {
            if(DireccionLatLong!="-")
            {
                Actualizar = false;
                string cadena = DireccionLatLong;
                string[] separadas = cadena.Split('|');
                latitud = separadas[0];
                longitud = separadas[1];
                Paglocalizar.longitud =Convert.ToDouble(longitud);
                Paglocalizar.latitud = Convert.ToDouble(latitud);
                await Navigation.PushAsync(new Paglocalizar());

            }
            else
            {
                Actualizar = false;
                await Navigation.PushAsync(new Paglocalizar());
            }
        }

        private async void btneliminarFoto_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Eliminando...");
            idgaleria = (sender as ImageButton).CommandParameter.ToString();
            await EliminarGaleria();
            await EliminarFotoStorage();
        }
        private async Task EliminarFotoStorage()
        {
            var funcion = new VMgaleriaTrab();
            await funcion.EliminarFotoStorage(idgaleria + ".jpg");
            await MostrarFotosGaleria();

        }
        private async Task EliminarGaleria()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idgaleria = idgaleria;
            await funcion.EliminarGaleriaTrab(parametros);
        }
        private async void btnAgregarFotos_Clicked(object sender, EventArgs e)
        {
            Actualizar = false;
            await TraerFoto();
        }
        private async Task TraerFoto()
        {
            await CrossMedia.Current.Initialize();
            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions());
                if (file != null)
                {
                    
                    await InsertarGaleria();
                    await ObtenerIdgaleria();
                    await SubirImagenesStore();
                    await EditarGaleria();
                    await MostrarFotosGaleria();
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Alerta", "Intentelo de nuevo", "OK");

            }
        }

        private async Task SubirImagenesStore()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idgaleria = idgaleria;
            rutaFoto = await funcion.SubirImagenesStorage(file.GetStream(), parametros);
        }
        private async Task InsertarGaleria()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.foto = "-";
            parametros.idnegocio = idnegocio;
            await funcion.InsertarGaleriaTrab(parametros);
            UserDialogs.Instance.ShowLoading("Agregando foto...");

        }
        private async Task ObtenerIdgaleria()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idnegocio = idnegocio;
            var dt = await funcion.ObtenerIdgaleria(parametros);
            foreach (var data in dt)
            {
                idgaleria = data.idgaleria;
            }

        }
        private async Task EditarGaleria()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idgaleria = idgaleria;
            parametros.foto = rutaFoto;
            await funcion.EditarGaleriaTrabajos(parametros);
        }

        private async Task obtenerIdUser()
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
            var guardarId = JsonConvert.DeserializeObject<FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            var RefrescarContenido = await authProvider.RefreshAuthAsync(guardarId);
            Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefrescarContenido));
            iduser = guardarId.User.LocalId;
            await MostrarNegociosxIduser();        
            await ObtenerCategoriaXId();
            await MostrarFotosGaleria();
        }
        private async Task ObtenerCategoriaXId()
        {
            if (Idcategoria != "-")
            {
                var funcion = new VMcategorias();
                var parametros = new Mcategorias();
                parametros.Idcategoria = Idcategoria;
                categoria = await funcion.ObtenerCategorixId(parametros);
                PickerCateg.SelectedItem = categoria;
            }

        }

        private async Task MostrarNegociosxIduser()
        {
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idusuario = iduser;
            var dt = await funcion.MostrarNegociosPorIduser(parametros);
            foreach (var data in dt)
            {
                idnegocio = data.idnegocio;
                txtcelular.Text = data.celular;
                txtdescripcion.Text = data.descripcion;
                Idcategoria = data.idcategoria;
                Idlocalizacion = data.idlocalidad;
                DireccionLatLong = data.direccion;
            }
        }

        private async Task MostrarFotosGaleria()
        {
            var funcion = new VMgaleriaTrab();
            var parametros = new MgaleriaTrab();
            parametros.idnegocio = idnegocio;
            var dt = await funcion.ListarFotosTrabajos(parametros);
            CarroselGaleria.ItemsSource = dt;
            UserDialogs.Instance.HideLoading();

        }
        private async Task ListarCategorias()
        {
            var funcion = new VMcategorias();
            var dt = await funcion.MostrarCategorias();
            foreach (var data in dt)
            {
                PickerCateg.Items.Add(data.Categoria.ToString());
                PickerCateg.SelectedItem = data.Categoria;
            }
            PickerCateg.SelectedIndexChanged += PickerCateg_SelectedIndexChanged;
        }

        private async void PickerCateg_SelectedIndexChanged(object sender, EventArgs e)
        {
            categoria = PickerCateg.SelectedItem.ToString();
            await ObtenerIdcategoria();
            Actualizar = true;
        }
        private async Task ObtenerIdcategoria()
        {
            var funcion = new VMcategorias();
            var parametros = new Mcategorias();
            parametros.Categoria = categoria;
            Idcategoria = await funcion.ObtenerIdcategoria(parametros);
        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            if (Idcategoria != "-")
            {
                if (txtcelular.Text != "-")
                {
                    if (txtdescripcion.Text != "-")
                    {
                        if (Idlocalizacion != "-")
                        {
                          await  EditarNegocios();
                          Categorias.Actualizar = true;
                            Actualizar = false;
                        }
                        else
                        {
                            await DisplayAlert("Localizacion", "Presione el boton de Localizar para indicar su dirección", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Descripcion", "Agrege una descripcion de lo que hace", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Celular", "Agrege nro de celular", "OK");
                }
            }
            else
            {
                await DisplayAlert("Categoria", "Seleccione una categoria", "OK");
            }
        }
        private async Task EditarNegocios()
        {
            UserDialogs.Instance.ShowLoading("Guardando...");
            var funcion = new VMnegocios();
            var parametros = new Mnegocios();
            parametros.idusuario = iduser;
            parametros.celular = txtcelular.Text;
            parametros.descripcion = txtdescripcion.Text;
            parametros.direccion = DireccionLatLong;
            parametros.idcategoria = Idcategoria;
            parametros.idlocalidad = Idlocalizacion;
            await funcion.EditarNegocios(parametros);
            UserDialogs.Instance.HideLoading();
        }

        private void txtcelular_TextChanged(object sender, TextChangedEventArgs e)
        {
            Actualizar = true;
        }

        private void txtdescripcion_TextChanged(object sender, TextChangedEventArgs e)
        {
            Actualizar = true;
        }
    }
}