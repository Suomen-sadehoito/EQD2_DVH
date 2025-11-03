using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace EQD2_DVH
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ScriptContext _context;
        private bool _showOriginalDVH = true;
        private bool _showEQD2DVH = true;
        private EQD2MeanCalculationMethod _calculationMethod = EQD2MeanCalculationMethod.Simple;
        private readonly List<(PlanSetup plan, Structure structure, DVHData dvhData)> _originalDvhCache = new List<(PlanSetup, Structure, DVHData)>();

        public PlotModel PlotModel { get; private set; }
        public ObservableCollection<DVHSummary> SummaryData { get; } = new ObservableCollection<DVHSummary>();
        public ObservableCollection<StructureAlphaBetaViewModel> StructureSettings { get; } = new ObservableCollection<StructureAlphaBetaViewModel>();
        public bool HasData { get; private set; } // Tämä on nyt aina 'true'

        public bool ShowOriginalDVH
        {
            get => _showOriginalDVH;
            set { _showOriginalDVH = value; OnPropertyChanged(); UpdatePlotVisibility(); }
        }

        public bool ShowEQD2DVH
        {
            get => _showEQD2DVH;
            set { _showEQD2DVH = value; OnPropertyChanged(); UpdatePlotVisibility(); }
        }

        public RelayCommand CalculateEQD2Command { get; }
        public RelayCommand ClearPlotCommand { get; }
        public RelayCommand AddStructuresCommand { get; }
        public RelayCommand ExportCSVCommand { get; }
        public RelayCommand ShowSettingsCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        // KORJAUS: Tämä konstruktori vastaanottaa datan suoraan.
        public MainViewModel(PlanSetup plan, IEnumerable<Structure> structures, ScriptContext context)
        {
            _context = context;
            InitializePlotModel();
            CalculateEQD2Command = new RelayCommand(CalculateAndPlotAllEQD2, () => _originalDvhCache.Any());
            ClearPlotCommand = new RelayCommand(ClearAndRestart);
            AddStructuresCommand = new RelayCommand(ShowSelectionWindow);
            ExportCSVCommand = new RelayCommand(ExportToCSV, () => SummaryData.Any());
            ShowSettingsCommand = new RelayCommand(ShowSettingsWindow);

            // Data ladataan TÄSSÄ, eikä ShowSelectionWindow-metodissa.
            HasData = true; // Oletamme, että dataa on, koska pääsimme tänne
            PlotDVH(plan, structures);
        }

        private void InitializePlotModel()
        {
            PlotModel = new PlotModel { Title = "DVH-käyrät" };
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Annos (Gy)", Minimum = 0, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Tilavuus (%)", Minimum = 0, Maximum = 101, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
            PlotModel.Legends.Add(new OxyPlot.Legends.Legend { LegendPosition = OxyPlot.Legends.LegendPosition.RightTop });
        }

        private void ShowSettingsWindow()
        {
            var settingsWindow = new SettingsWindow(_calculationMethod);
            if (settingsWindow.ShowDialog() == true)
            {
                _calculationMethod = settingsWindow.SelectedMethod;
                if (SummaryData.Any(s => s.Type == "EQD2"))
                {
                    CalculateAndPlotAllEQD2();
                }
            }
        }

        // Tämä metodi KUTSUU nyt valintaikkunan "Lisää rakenteita..." -napista.
        private void ShowSelectionWindow()
        {
            var selectionWindow = new SelectionWindow(_context);
            if (selectionWindow.ShowDialog() == true)
            {
                PlotDVH(selectionWindow.SelectedPlan, selectionWindow.SelectedStructures);
            }
        }

        private void PlotDVH(PlanSetup plan, IEnumerable<Structure> structures)
        {
            if (plan == null || !structures.Any()) return;
            foreach (var structure in structures)
            {
                if (_originalDvhCache.Any(t => t.plan.Id == plan.Id && t.structure.Id == structure.Id)) continue;
                DVHData dvhData = plan.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, VolumePresentation.Relative, 0.01);
                if (dvhData != null)
                {
                    _originalDvhCache.Add((plan, structure, dvhData));
                    double defaultAlphaBeta = (structure.DicomType == "PTV" || structure.DicomType == "CTV" || structure.DicomType == "GTV") ? 10.0 : 3.0;
                    StructureSettings.Add(new StructureAlphaBetaViewModel(structure, defaultAlphaBeta));
                    var summary = new DVHSummary { StructureId = structure.Id, PlanId = plan.Id, Type = "Orig", DMax = dvhData.MaxDose.Dose, DMean = dvhData.MeanDose.Dose, DMin = dvhData.MinDose.Dose, Volume = dvhData.Volume };
                    SummaryData.Add(summary);
                    var series = new LineSeries { Title = $"{structure.Id} ({plan.Id})", Tag = $"Original_{plan.Id}_{structure.Id}", Color = structure.Color.ToOxyColor() };
                    series.Points.AddRange(dvhData.CurveData.Select(p => new DataPoint(p.DoseValue.Dose, p.Volume)));
                    PlotModel.Series.Add(series);
                }
            }
            RefreshPlot();
        }

        private void CalculateAndPlotAllEQD2()
        {
            var oldEqdSeries = PlotModel.Series.Where(s => (s.Tag as string)?.StartsWith("EQD2_") ?? false).ToList();
            foreach (var s in oldEqdSeries) PlotModel.Series.Remove(s);
            var oldEqdSummaries = SummaryData.Where(s => s.Type == "EQD2").ToList();
            foreach (var s in oldEqdSummaries) SummaryData.Remove(s);

            foreach (var (plan, structure, dvhData) in _originalDvhCache)
            {
                int numberOfFractions = plan.NumberOfFractions ?? 1;
                var setting = StructureSettings.FirstOrDefault(s => s.Structure.Id == structure.Id);
                double alphaBeta = setting?.AlphaBeta ?? 3.0;

                double eqd2_Dmin = DVHCalculator.CalculateEQD2ForPoint(dvhData.MinDose.Dose, numberOfFractions, alphaBeta);
                double eqd2_Dmax = DVHCalculator.CalculateEQD2ForPoint(dvhData.MaxDose.Dose, numberOfFractions, alphaBeta);

                double eqd2_Dmean;
                if (_calculationMethod == EQD2MeanCalculationMethod.Differential)
                {
                    eqd2_Dmean = DVHCalculator.CalculateMeanEQD2FromDVH(dvhData.CurveData, numberOfFractions, alphaBeta);
                }
                else
                {
                    eqd2_Dmean = DVHCalculator.CalculateEQD2ForPoint(dvhData.MeanDose.Dose, numberOfFractions, alphaBeta);
                }

                var eqd2Summary = new DVHSummary { StructureId = structure.Id, PlanId = plan.Id, Type = "EQD2", DMin = eqd2_Dmin, DMean = eqd2_Dmean, DMax = eqd2_Dmax, Volume = dvhData.Volume };
                SummaryData.Add(eqd2Summary);

                var eqd2CurvePoints = DVHCalculator.GetEQD2Curve(dvhData.CurveData, numberOfFractions, alphaBeta);
                var eqd2series = new LineSeries { Title = $"{structure.Id} EQD2 (α/β={alphaBeta:F1})", LineStyle = LineStyle.Dash, Tag = $"EQD2_{plan.Id}_{structure.Id}", Color = structure.Color.ToOxyColor() };
                eqd2series.Points.AddRange(eqd2CurvePoints.Select(p => new DataPoint(p.DoseValue.Dose, p.Volume)));
                PlotModel.Series.Add(eqd2series);
            }
            RefreshPlot();
        }

        private void ClearAndRestart()
        {
            // Tyhjennetään nykyiset tiedot
            _originalDvhCache.Clear();
            StructureSettings.Clear();
            PlotModel.Series.Clear();
            SummaryData.Clear();
            RefreshPlot();

            // Avataan valintaikkuna uudelleen "Lisää rakenteita..." -napin toiminnallisuudella
            ShowSelectionWindow();
        }

        private void UpdatePlotVisibility()
        {
            foreach (var series in PlotModel.Series)
            {
                if (series.Tag is string tag)
                {
                    series.IsVisible = (tag.StartsWith("Original_") && ShowOriginalDVH) || (tag.StartsWith("EQD2_") && ShowEQD2DVH);
                }
            }
            PlotModel.InvalidatePlot(true);
        }

        private void RefreshPlot()
        {
            UpdatePlotVisibility();
            CalculateEQD2Command.RaiseCanExecuteChanged();
            ExportCSVCommand.RaiseCanExecuteChanged();
        }

        private void ExportToCSV()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV-tiedosto (*.csv)|*.csv",
                Title = "Tallenna yhteenveto",
                FileName = $"EQD2_DVH_Summary_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("StructureId,PlanId,Type,DMax_Gy,DMean_Gy,DMin_Gy,Volume_cm3");

                    foreach (var summary in SummaryData)
                    {
                        sb.AppendLine($"{summary.StructureId},{summary.PlanId},{summary.Type},{summary.DMax:F2},{summary.DMean:F2},{summary.DMin:F2},{summary.Volume:F2}");
                    }

                    File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Tiedosto tallennettu onnistuneesti!", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Virhe tallennuksessa: {ex.Message}", "Virhe", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// This is the helper class that was missing. It connects a Structure with its alpha/beta value for the UI.
    /// </summary>
    public class StructureAlphaBetaViewModel : INotifyPropertyChanged
    {
        public Structure Structure { get; }
        private double _alphaBeta;

        public string Id => Structure.Id;
        public string DicomType => Structure.DicomType;

        public double AlphaBeta
        {
            get => _alphaBeta;
            set { _alphaBeta = value; OnPropertyChanged(); }
        }

        public StructureAlphaBetaViewModel(Structure structure, double alphaBeta)
        {
            Structure = structure;
            _alphaBeta = alphaBeta;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}