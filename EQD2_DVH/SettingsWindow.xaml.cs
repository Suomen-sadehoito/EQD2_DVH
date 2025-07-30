using System.Windows;

namespace EQD2_DVH
{
    public partial class SettingsWindow : Window
    {
        public EQD2MeanCalculationMethod SelectedMethod { get; private set; }

        public SettingsWindow(EQD2MeanCalculationMethod currentMethod)
        {
            InitializeComponent();
            SetInitialSelection(currentMethod);
        }

        private void SetInitialSelection(EQD2MeanCalculationMethod method)
        {
            if (method == EQD2MeanCalculationMethod.Differential)
            {
                DifferentialRadioButton.IsChecked = true;
            }
            else
            {
                SimpleRadioButton.IsChecked = true;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DifferentialRadioButton.IsChecked == true)
                SelectedMethod = EQD2MeanCalculationMethod.Differential;
            else
                SelectedMethod = EQD2MeanCalculationMethod.Simple;

            DialogResult = true;
        }
    }
}