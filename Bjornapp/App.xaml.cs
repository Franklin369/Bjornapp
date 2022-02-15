using Bjornapp.Vistas;
using Bjornapp.Vistas.Configuraciones;
using Bjornapp.Vistas.TutorialIntro;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bjornapp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Presentacion();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
