using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace EQD2_DVH
{
    public partial class SelectionWindow : Window
    {
        public PlanSetup SelectedPlan { get; private set; }
        public IEnumerable<Structure> SelectedStructures { get; private set; }

        public SelectionWindow(ScriptContext context)
        {
            InitializeComponent();
            DataContext = new SelectionViewModel(context);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SelectionViewModel;
            if (vm != null)
            {
                SelectedPlan = vm.SelectedPlan;
                SelectedStructures = StructureListBox.SelectedItems.Cast<Structure>();
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }
}