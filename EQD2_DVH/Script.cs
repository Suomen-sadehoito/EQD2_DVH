using System;
using System.Runtime.CompilerServices;
using System.Windows;
using VMS.TPS.Common.Model.API;
using System.Reflection;
using System.IO;
using System.Threading; // Tarvitaan tätä varten

namespace VMS.TPS
{
    public class Script
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        // KORJAUS 1: Poistetaan 'Window window' -parametri.
        // Tämä on nyt "ikkunaton" skripti Eclipsen näkökulmasta.
        public void Execute(ScriptContext context)
        {
            if (context.Patient == null)
            {
                MessageBox.Show("Avaa potilas ennen skriptin suorittamista.", "Potilasta ei avattu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // KORJAUS 2: Käynnistetään koko WPF-sovellus omassa säikeessään.
                Thread t = new Thread(() =>
                {
                    // Luodaan oma Application-instanssi, jotta emme ole
                    // riippuvaisia Eclipsen ikkunanhallinnasta.
                    var app = new System.Windows.Application();

                    // 1. Näytä ENSIN VAIN valintaikkuna.
                    var selectionWindow = new EQD2_DVH.SelectionWindow(context);
                    // (WindowStartupLocation="CenterScreen" hoitaa keskityksen)

                    if (selectionWindow.ShowDialog() == true)
                    {
                        // 2. JOS käyttäjä painoi "OK"...
                        // ...luo ja näytä pääikkuna VALITUILLA TIEDOILLA.
                        var mainWindow = new EQD2_DVH.MainWindow(
                            selectionWindow.SelectedPlan,
                            selectionWindow.SelectedStructures,
                            context);

                        mainWindow.ShowDialog();
                    }

                    // 3. Kun viimeinen ikkuna (Selection tai Main) suljetaan,
                    // sulje oma Application-instanssimme siististi.
                    app.Shutdown();
                });

                // KORJAUS 3: Aseta säie "Single-Threaded Apartment" -tilaan (pakollinen WPF:lle).
                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                // KORJAUS 4: Odota, että WPF-säie päättyy (käyttäjä sulkee ikkunan),
                // ennen kuin Execute-metodi päättyy.
                t.Join();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Odottamaton virhe:\n\n{ex.ToString()}", "Skriptin suoritus epäonnistui", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}