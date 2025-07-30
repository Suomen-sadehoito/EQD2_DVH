using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;

namespace EQD2_DVH
{
    public class SelectionViewModel : INotifyPropertyChanged
    {
        private PlanSetup _selectedPlan;
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<PlanSetup> Plans { get; private set; }
        public IEnumerable<Structure> Structures { get; private set; }

        public PlanSetup SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                _selectedPlan = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSelect));
                LoadStructures();
            }
        }

        public bool CanSelect => SelectedPlan != null;

        public SelectionViewModel(ScriptContext context)
        {
            LoadPlans(context);
        }

        private void LoadPlans(ScriptContext context)
        {
            // Hakee kaikki potilaan suunnitelmat, joilla on rakennesetti.
            Plans = context.Patient.Courses
                                .SelectMany(c => c.PlanSetups)
                                .Where(p => p.StructureSet != null)
                                .OrderBy(p => p.Id);
        }

        /// <summary>
        /// KORJATTU METODI:
        /// Tämä metodi lataa valitun suunnitelman rakenteet.
        /// Koodi poistaa päällekkäiset rakenteet nimen perusteella, suodattaa pois tyhjät
        /// rakenteet ja järjestää ne aakkosjärjestykseen.
        /// Tämä toimii kaikissa ESAPI-versioissa.
        /// </summary>
        private void LoadStructures()
        {
            if (_selectedPlan != null && _selectedPlan.StructureSet != null)
            {
                Structures = _selectedPlan.StructureSet.Structures
                                        .Where(s => !s.IsEmpty)      // 1. Suodata pois tyhjät.
                                        .GroupBy(s => s.Id)          // 2. Ryhmittele nimen mukaan.
                                        .Select(g => g.First())      // 3. Ota kustakin ryhmästä ensimmäinen (poistaa duplikaatit).
                                        .OrderBy(s => s.Id);         // 4. Järjestä aakkosjärjestykseen.

                OnPropertyChanged(nameof(Structures));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}