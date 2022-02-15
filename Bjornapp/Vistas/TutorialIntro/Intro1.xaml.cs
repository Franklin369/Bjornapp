using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bjornapp.VistasModelo;
namespace Bjornapp.Vistas.TutorialIntro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Intro1 : ContentPage
    {
        public Intro1()
        {
            InitializeComponent();
            DependencyService.Get<VMstatusbar>().MostrarStatusBar();
            var colorStatusbar = DependencyService.Get<VMstatusbar>();
            colorStatusbar.Intro();
        }

        private void btnSaltar_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Login());
        }
    }
}