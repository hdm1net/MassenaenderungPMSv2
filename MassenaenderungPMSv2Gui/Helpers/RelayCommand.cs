using System.Windows.Input;

namespace MassenaenderungPMSv2Gui.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Erstellt einen neuen Befehl.
        /// </summary>
        /// <param name="execute">Die Methode, die ausgeführt wird.</param>
        /// <param name="canExecute">Optionale Logik, die prüft, ob der Befehl gerade ausgeführt werden darf (deaktiviert sonst den Button).</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Registriert Änderungen im WPF-CommandManager, damit Buttons automatisch aktiv/inaktiv werden.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }


}
