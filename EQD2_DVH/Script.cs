using System;
using System.Windows;
using VMS.TPS.Common.Model.API;
using System.Reflection;
using EQD2_DVH; // Viittaus SelectionWindow- ja MainWindow-luokkiin

namespace VMS.TPS
{
    public class Script
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context)
        {
            // 1. Tarkistetaan, että potilas on auki
            if (context.Patient == null || context.PlanSetup == null)
            {
                MessageBox.Show("Avaa potilas ja suunnitelma ennen skriptin suorittamista.",
                                "Huomio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 2. Luodaan ja näytetään valintaikkuna               
                var selectionWindow = new SelectionWindow(context);

                // ShowDialog pysäyttää suorituksen tähän, kunnes ikkuna suljetaan.
                // Jos käyttäjä painaa "Hyväksy" (DialogResult = true), jatketaan.
                if (selectionWindow.ShowDialog() == true)
                {
                    // 3. Haetaan valitut tiedot
                    var selectedPlan = selectionWindow.SelectedPlan;
                    var selectedStructures = selectionWindow.SelectedStructures;

                    // 4. Avataan pääikkuna valituilla tiedoilla
                    var mainWindow = new MainWindow(selectedPlan, selectedStructures, context);

                    // Näytetään pääikkuna
                    mainWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tapahtui virhe:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                                "EQD2 Script Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}