using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MassenaenderungPMSv2Gui.Helpers
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // Fehler-Speicher
        private readonly Dictionary<string, List<string>> _errors = new();


        /// <summary>
        /// Aktualisiert den Wert einer Eigenschaft und benachrichtigt die View bei Änderungen.
        /// </summary>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            ValidateProperty(propertyName);   // Wichtig für Fehlerverwaltung
            return true;
        }

        /// <summary>
        /// Löst das PropertyChanged-Ereignis manuell aus.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // -----------------------------
        // Fehlerverwaltung
        // -----------------------------
        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && _errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return null;
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        // -----------------------------
        // Validierungshook
        // -----------------------------
        /// <summary>
        /// Wird automatisch nach SetProperty() aufgerufen.
        /// Abgeleitete ViewModels überschreiben diese Methode.
        /// </summary>
        protected virtual void ValidateProperty(string propertyName)
        {
            // Standard: keine Validierung
            // Abgeleitete Klassen implementieren hier ihre Logik
        }

        /// <summary>
        /// Kann von ViewModels genutzt werden, um eine Gesamtvalidierung auszuführen.
        /// </summary>
        public virtual bool ValidateAll()
        {
            // Abgeleitete ViewModels implementieren hier ihre Gesamtvalidierung
            return !HasErrors;
        }
    }
}
