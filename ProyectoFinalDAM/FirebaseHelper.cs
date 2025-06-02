using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public static class FirebaseHelper
    {
        private const string ApiKey = "AIzaSyCi-GG4S9aNXyOapa8CZRFdVus-5UO-8Pw";
        private static FirebaseClient firebase;

        static FirebaseHelper()
        {
            // Ruta al archivo de credenciales de Firebase (clave privada del servicio)
            var pathToServiceAccount = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "taxibdreal-firebase-adminsdk-fbsvc-48ad1cf218.json");

            // Verifica si el archivo de credenciales existe
            if (!File.Exists(pathToServiceAccount))
            {
                throw new FileNotFoundException($"No se encontró el archivo de credenciales en la ruta {pathToServiceAccount}");
            }

            // Inicializar Firebase Admin SDK con la clave privada
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(pathToServiceAccount)
            });

            // Conectar a Realtime Database
            firebase = new FirebaseClient("https://taxibdreal-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        public static async Task<Taxista> ObtenerTaxista(string id)
        {
            return await firebase.Child("Taxista").Child(id).OnceSingleAsync<Taxista>();
        }

        public static async Task<Paciente> ObtenerPaciente(string id)
        {
            return await firebase.Child("Paciente").Child(id).OnceSingleAsync<Paciente>();
        }

        public static async Task<(bool success, string role)> LoginConNombreYPassword(string nombre, string contraseña)
        {
            try
            {
                // Validación básica
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contraseña))
                {
                    MessageBox.Show("Por favor ingrese nombre de usuario y contraseña.");
                    return (false, null);
                }

                // Buscar directamente el nodo del usuario por su nombre
                var usuario = await firebase
                    .Child("nombresUsuarios")
                    .Child(nombre)  // Acceso directo al nodo con el nombre de usuario
                    .OnceSingleAsync<UsuarioFirebase>();

                if (usuario == null)
                {
                    MessageBox.Show("No se encontró un usuario con ese nombre.");
                    return (false, null);
                }

                if (string.IsNullOrWhiteSpace(usuario.Email))
                {
                    MessageBox.Show("El usuario no tiene un email válido asociado.");
                    return (false, null);
                }

                // Autenticación con Firebase Auth
                using (var http = new HttpClient())
                {
                    var authRequest = new
                    {
                        email = usuario.Email,
                        password = contraseña,
                        returnSecureToken = true
                    };

                    var content = new StringContent(
                        JsonConvert.SerializeObject(authRequest),
                        Encoding.UTF8,
                        "application/json");

                    var response = await http.PostAsync(
                        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}",
                        content);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return (true, usuario.Rol);
                    }
                    else
                    {
                        var errorData = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        string errorMessage = errorData?.error?.message ?? "Credenciales incorrectas";

                        if (errorMessage.Contains("INVALID_PASSWORD"))
                        {
                            MessageBox.Show("La contraseña es incorrecta.");
                        }
                        else
                        {
                            MessageBox.Show($"Error de autenticación: {errorMessage}");
                        }

                        return (false, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, null);
            }
        }

        /// <summary>
        /// Inicia sesión directamente con correo y contraseña en Firebase Authentication.
        /// </summary>
        public static async Task<(bool success, string role)> LoginDirectoConEmail(string email, string password)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(new
                    {
                        email = email,
                        password = password,
                        returnSecureToken = true
                    });

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

   
                    var response = await http.PostAsync(
                        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}",
                        content);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Aquí puedes extraer el rol si lo necesitas desde otro nodo o desde claims
                        // Por ahora devolvemos un rol genérico o podrías traerlo desde RTDB si tienes una ruta específica
                        return (true, "admin"); // Puedes ajustarlo más tarde
                    }

                    MessageBox.Show($"Error de autenticación: {responseContent}");
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excepción: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (false, null);
            }
        }

        /// <summary>
        /// Modelo de usuario almacenado en RTDB
        /// </summary>
        public class UsuarioFirebase
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("rol")]
            public string Rol { get; set; }

            [JsonProperty("nombre")]
            public string Nombre { get; set; } // Nombre del usuario como "Administrador", "Trabajador", etc.
        }

        #region Servicios

        public static async Task<List<FirebaseObject<Servicio>>> ObtenerTodosLosServicios()
        {
            return (await firebase.Child("Servicio").OnceAsync<Servicio>()).ToList();
        }

        public static async Task<Servicio> ObtenerServicio(string id)
        {
            return await firebase.Child("Servicio").Child(id).OnceSingleAsync<Servicio>();
        }

        public static async Task<string> AñadirServicio(Servicio servicio)
        {
            var result = await firebase.Child("Servicio").PostAsync(servicio);
            return result.Key;
        }

        public static async Task ActualizarServicio(string id, Servicio servicio)
        {
            await firebase.Child("Servicio").Child(id).PutAsync(servicio);
        }

        public static async Task EliminarServicio(string id)
        {
            await firebase.Child("Servicio").Child(id).DeleteAsync();
        }

        #endregion

        #region Lugares

        public static async Task<List<FirebaseObject<Lugar>>> ObtenerTodosLosLugares()
        {
            return (await firebase.Child("Lugar").OnceAsync<Lugar>()).ToList();
        }

        public static async Task<string> AñadirLugar(Lugar lugar)
        {
            var result = await firebase.Child("Lugar").PostAsync(lugar);
            return result.Key;
        }

        public static async Task EliminarLugar(string id)
        {
            await firebase.Child("Lugar").Child(id).DeleteAsync();
        }

        public static async Task<Lugar> ObtenerLugar(string id)
        {
            return await firebase.Child("Lugar").Child(id).OnceSingleAsync<Lugar>();
        }

        public static async Task ActualizarLugar(string id, Lugar lugar)
        {
            await firebase.Child("Lugar").Child(id).PutAsync(lugar);
        }

        #endregion

        #region Taxistas

        public static async Task<List<FirebaseObject<Taxista>>> ObtenerTodosLosTaxistas()
        {
            return (await firebase.Child("Taxista").OnceAsync<Taxista>()).ToList();
        }

        public static async Task AñadirTaxista(Taxista taxista)
        {
            await firebase.Child("Taxista").PostAsync(taxista);
        }

        public static async Task ActualizarTaxista(string id, Taxista taxista)
        {
            await firebase.Child("Taxista").Child(id).PutAsync(taxista);
        }

        public static async Task EliminarTaxista(string id)
        {
            await firebase.Child("Taxista").Child(id).DeleteAsync();
        }

        #endregion

        #region Pacientes

        public static async Task<List<FirebaseObject<Paciente>>> ObtenerTodosLosPacientes()
        {
            return (await firebase.Child("Paciente").OnceAsync<Paciente>()).ToList();
        }

        public static async Task AñadirPaciente(Paciente paciente)
        {
            await firebase.Child("Paciente").PostAsync(paciente);
        }

        public static async Task ActualizarPaciente(string id, Paciente paciente)
        {
            await firebase.Child("Paciente").Child(id).PutAsync(paciente);
        }

        public static async Task EliminarPaciente(string id)
        {
            await firebase.Child("Paciente").Child(id).DeleteAsync();
        }

        #endregion

        #region Facturas
        public static async Task<List<FirebaseObject<Factura>>> ObtenerTodasLasFacturas()
        {
            return (await firebase.Child("Factura").OnceAsync<Factura>()).ToList();
        }
        public static async Task AñadirFactura(Factura factura)
        {
            await firebase.Child("Factura").PostAsync(factura);
        }
        public static async Task ActualizarFactura(string id, Factura factura)
        {
            await firebase.Child("Factura").Child(id).PutAsync(factura);
        }
        public static async Task EliminarFactura(string id)
        {
            await firebase.Child("Factura").Child(id).DeleteAsync();
        }

        public static async Task GuardarFactura(string idFactura, object factura)
        {
            await firebase.Child("Factura").Child(idFactura).PutAsync(factura);
        }

        public static async Task<Factura> ObtenerFactura(string id)
        {
            return await firebase.Child("Factura").Child(id).OnceSingleAsync<Factura>();
        }

        #endregion
    }
}
