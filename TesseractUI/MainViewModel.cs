using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Tesseract;
using TesseractUI.Annotations;
using TesseractWrappers;
using TesseractWrappers.Models;

namespace TesseractUI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private EngineMode _selectedEngineMode = EngineMode.Default;
        private readonly PixHandlersManager _manager = new PixHandlersManager();
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        public EngineMode SelectedEngineMode
        {
            get => _selectedEngineMode;
            set
            {
                if (value == _selectedEngineMode) return;
                _selectedEngineMode = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<EngineMode> EngineModes => 
            new ObservableCollection<EngineMode>(Enum.GetValues(typeof(EngineMode)).Cast<EngineMode>());

        public ObservableCollection<PixHandler> Handlers => new ObservableCollection<PixHandler>(_manager.Handlers);


    }
}