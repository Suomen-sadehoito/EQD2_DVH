using System.Windows;
using VMS.TPS.Common.Model.API;
using System.Collections.Generic; // Lisätty tämä

namespace EQD2_DVH
{
    public partial class MainWindow : Window
    {
        // Konstruktori ottaa vastaan valmiit tiedot Script.cs:ltä.
        public MainWindow(PlanSetup plan, IEnumerable<Structure> structures, ScriptContext context)
        {
            InitializeComponent();

            // Luodaan ViewModel VASTA NYT, kun tiedot on valittu.
            var viewModel = new MainViewModel(plan, structures, context);
            this.DataContext = viewModel;
        }
    }
}