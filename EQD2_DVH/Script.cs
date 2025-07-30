using System.Runtime.CompilerServices;
using System.Windows;
using VMS.TPS.Common.Model.API;
using System;
using System.Reflection; // Tarvitaan Assembly-luokkaa varten
using System.IO;         // Tarvitaan Path-luokkaa varten

namespace VMS.TPS
{
    public class Script
    {
        // MUOKKAUS ALKAA TÄSTÄ
        public Script()
        {
            // Rekisteröidään tapahtumankäsittelijä, joka auttaa löytämään puuttuvat .dll-tiedostot.
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);
        }

        /// <summary>
        /// Tämä metodi suoritetaan, kun .NET-ympäristö ei löydä jotain viitattua kirjastoa (assembly).
        /// Koodi etsii puuttuvaa .dll-tiedostoa skriptin omasta suorituskansiosta ja lataa sen manuaalisesti.
        /// </summary>
        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            // Otetaan ladattavan assemblyn nimi (esim. "OxyPlot")
            string assemblyName = new AssemblyName(args.Name).Name;

            // Haetaan tämän hetkisen skriptin sijainti
            string scriptPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Muodostetaan koko polku oletettuun .dll-tiedostoon
            string dllPath = Path.Combine(scriptPath, assemblyName + ".dll");

            // Jos tiedosto löytyy polusta, ladataan se
            if (File.Exists(dllPath))
            {
                return Assembly.LoadFrom(dllPath);
            }

            // Jos tiedostoa ei löydy, palautetaan null, jolloin ohjelma jatkaa normaalia virheenkäsittelyä
            return null;
        }
        // MUOKKAUS PÄÄTTYY TÄHÄN

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context, Window window)
        {
            if (context.Patient == null)
            {
                MessageBox.Show("Avaa potilas ennen skriptin suorittamista.", "Potilasta ei avattu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Piilotetaan Eclipsen tarjoama tyhjä skripti-ikkuna.
                window.Hide();

                // Luodaan uusi instanssi pääikkunastamme ja näytetään se.
                // ShowDialog() pysäyttää suorituksen, kunnes ikkuna suljetaan.
                var app = new EQD2_DVH.MainWindow(context);
                app.ShowDialog();

                // Kun oma ikkuna suljetaan, suljetaan myös piilotettu Eclipsen ikkuna,
                // jotta skripti päättyy siististi.
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Odottamaton virhe:\n\n{ex.ToString()}", "Skriptin suoritus epäonnistui", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}