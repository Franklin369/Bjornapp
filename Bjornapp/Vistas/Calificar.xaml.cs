using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bjornapp.VistasModelo;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bjornapp.Modelo;
using Rg.Plugins.Popup.Services;
namespace Bjornapp.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Calificar : PopupPage
    {
        public Calificar()
        {
            InitializeComponent();
        }
        public static string idusuario;
        public static string idnegocio;
        public static string reseña="";
        public static string calificacion;
        public static string idcalificacion;

        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            if (reseña!="")
            {
                await EditarReseña();
            }
            else
            {
                if (!string.IsNullOrEmpty(txtreseña.Text))
                {
                    await InsertarReseñas();
                }
                else
                {
                    await DisplayAlert("Alerta", "Ingrese una reseña", "OK");
                }
            }
        }
        protected override void OnAppearing()
        {
            if (reseña!="")
            {
                txtreseña.Text = reseña;
                EstCalificacion.Rating =Convert.ToInt32( calificacion);
                lblTitulo.Text = "Edita tu reseña cuando desees";
            }
        }
        private async Task EditarReseña()
        {
            var funcion = new VMresenias();
            var parametros = new Mresenias();
            parametros.reseña = txtreseña.Text;
            parametros.calificacion = EstCalificacion.Rating.ToString();
            parametros.idcalificacion = idcalificacion;
            await funcion.EditarReseña(parametros);
            await PopupNavigation.Instance.PopAsync();
        }
        private async Task InsertarReseñas()
        {
            var funcion = new VMresenias();
            var parametros = new Mresenias();
            parametros.calificacion = EstCalificacion.Rating.ToString();
            parametros.reseña = txtreseña.Text;
            parametros.idusuario = idusuario;
            parametros.idnegocio = idnegocio;
            await funcion.InsertarReseña(parametros);
            await PopupNavigation.Instance.PopAsync();
        }

    }
}