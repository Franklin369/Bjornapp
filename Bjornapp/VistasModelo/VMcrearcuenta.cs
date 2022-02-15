using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using Bjornapp.VistasModelo;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace Bjornapp.VistasModelo
{
   public class VMcrearcuenta
    {
        bool estado;
        public async Task Crearcuenta(string correo,string contraseña)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
            await authProvider.CreateUserWithEmailAndPasswordAsync(correo, contraseña);

        }
        public async Task<bool> ValidarCuenta(string correo, string contraseña)
        {
            try
            {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Constantes.WebapyFirebase));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(correo, contraseña);
            var serializartoken = JsonConvert.SerializeObject(auth);
            Preferences.Set("MyFirebaseRefreshToken", serializartoken);
                return estado = true;
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Datos incorrectos", "OK");
                return estado = false;

            }

        }
      
    }
}
