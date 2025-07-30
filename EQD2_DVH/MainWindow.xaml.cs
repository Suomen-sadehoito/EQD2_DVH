using System.Windows;
using VMS.TPS.Common.Model.API;

namespace EQD2_DVH
{
    public partial class MainWindow : Window
    {
        public MainWindow(ScriptContext context)
        {
            InitializeComponent();
            var viewModel = new MainViewModel(context);
            this.DataContext = viewModel;

            // Suljetaan pääikkuna heti, jos käyttäjä peruuttaa ensimmäisen valinnan.
            this.Loaded += (s, e) =>
            {
                if (!viewModel.HasData)
                {
                    this.Close();
                }
            };
        }
    }
}