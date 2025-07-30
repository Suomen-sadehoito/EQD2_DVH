using System;
using System.Windows.Input;

namespace EQD2_DVH
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        // Tapahtuma, joka kertoo UI:lle, että CanExecute-tila on saattanut muuttua.
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Kertoo, voiko komennon suorittaa.
        /// </summary>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Suorittaa komennon.
        /// </summary>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// Tämä metodi on avainasemassa. Kutsu tätä, kun haluat päivittää painikkeen
        /// aktiivisen/passiivisen tilan.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}