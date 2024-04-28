using OptiMate;
using OptiMate.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Events;
using OptiMate.ViewModels;
using OptiMate.Logging;
using System.Windows;

namespace OptiMate.ViewModels
{
    public class TemplateViewModel : ObservableObject
    {

        private TemplateModel _templateModel;

        private IEventAggregator _ea;
        public ObservableCollection<GeneratedStructureViewModel> GeneratedStructuresVM { get; set; } = new ObservableCollection<GeneratedStructureViewModel>();
        public ObservableCollection<TemplateStructureViewModel> TemplateStructuresVM { get; set; } = new ObservableCollection<TemplateStructureViewModel>();

        public bool HasTemplateStructures
        {
            get
            {
                return _templateModel.GetTemplateStructureIds().Count() > 0;
            }
        }

        public int SelectedTSIndex { get; set; }
        public int SelectedGSIndex { get; set; }
        public ConfirmationViewModel ConfirmRemoveTemplateStructureVM { get; set; }
        public ConfirmationViewModel ConfirmRemoveGeneratedStructureVM { get; set; }

        public string TemplateDisplayName
        {
            get
            {
                return _templateModel.TemplateDisplayName;
            }
            set
            {
                if (value != _templateModel.TemplateDisplayName)
                {
                    _templateModel.TemplateDisplayName = value;
                }
            }
        }
        public TemplateViewModel(TemplateModel templateModel, IEventAggregator ea)
        {
            _templateModel = templateModel;
            _ea = ea;
            RegisterEvents();
            GeneratedStructuresVM = new ObservableCollection<GeneratedStructureViewModel>();
            TemplateStructuresVM = new ObservableCollection<TemplateStructureViewModel>();
            foreach (var structureId in _templateModel.GetGeneratedStructureIds())
            {
                var genStructureModel = _templateModel.GetGeneratedStructureModel(structureId);
                GeneratedStructuresVM.Add(new GeneratedStructureViewModel(genStructureModel, _ea));
            }
            foreach (var structureId in _templateModel.GetTemplateStructureIds())
            {
                var tempStructureModel = _templateModel.GetTemplateStructureModel(structureId);
                TemplateStructuresVM.Add(new TemplateStructureViewModel(tempStructureModel, _templateModel, _ea));
            }
            RaisePropertyChangedEvent(nameof(HasTemplateStructures));
        }

        private void RegisterEvents()
        {
            ErrorsChanged += PublishToEventAggregator;
            _ea.GetEvent<RemovedGeneratedStructureEvent>().Subscribe(RemoveGeneratedStructureFromList);


        }

        private void RemoveGeneratedStructureFromList(RemovedGeneratedStructureEventInfo info)
        {
            var genStructureVMtoRemove = GeneratedStructuresVM.FirstOrDefault(x => x.StructureId == info.RemovedStructureId);
            GeneratedStructuresVM.Remove(genStructureVMtoRemove);
        }

        private void PublishToEventAggregator(object sender, DataErrorsChangedEventArgs e)
        {
            _ea.GetEvent<DataValidationRequiredEvent>().Publish();
        }

        public TemplateViewModel()
        {
            // Design use only
            GeneratedStructuresVM.Add(new GeneratedStructureViewModel());
            GeneratedStructuresVM.Add(new GeneratedStructureViewModel());
            GeneratedStructuresVM.Add(new GeneratedStructureViewModel());
            TemplateStructuresVM.Add(new TemplateStructureViewModel());
            TemplateStructuresVM.Add(new TemplateStructureViewModel());
        }

        private bool _editAllToggle = false;
        public ICommand ToggleEditAllGeneratedStructuresCommand
        {
            get
            {
                return new DelegateCommand(ToggleEditAllGeneratedStructures);
            }
        }

        private void ToggleEditAllGeneratedStructures(object obj)
        {
            _editAllToggle = !_editAllToggle;
            foreach (var genStructure in GeneratedStructuresVM)
            {
                genStructure.EditMode = _editAllToggle;
            }
        }

        public ICommand AddGeneratedStructureCommand
        {
            get
            {
                return new DelegateCommand(AddGeneratedStructure);
            }
        }
        public void AddGeneratedStructure(object param = null)
        {

            GeneratedStructureModel genStructure = _templateModel.AddGeneratedStructure();
            GeneratedStructuresVM.Add(new GeneratedStructureViewModel(genStructure, _ea, true));
            _ea.GetEvent<DataValidationRequiredEvent>().Publish();

        }

        public ICommand ConfirmRemoveGeneratedStructureCommand
        {
            get
            {
                return new DelegateCommand(ConfirmRemoveGeneratedStructure);
            }
        }

        private void ConfirmRemoveGeneratedStructure(object param = null)
        {
            var genStructureVM = param as GeneratedStructureViewModel;
            object[] RemoveStructureParam = new object[] { genStructureVM, _templateModel, GeneratedStructuresVM, _ea };
            ConfirmRemoveGeneratedStructureVM = new ConfirmationViewModel("Removing this will also remove all operators referencing this structure. Continue?", new DelegateCommand(RemoveGeneratedStructure), RemoveStructureParam);
            genStructureVM.ConfirmRemoveStructurePopupVisibility ^= true;
        }

        internal void RemoveGeneratedStructure(object param = null)
        {
            var genStructureVM = (param as object[])[0] as GeneratedStructureViewModel;
            var _templateModel = (param as object[])[1] as TemplateModel;
            var GeneratedStructuresVM = (param as object[])[2] as ObservableCollection<GeneratedStructureViewModel>;
            var _ea = (param as object[])[3] as IEventAggregator;

            _templateModel.RemoveGeneratedStructure(genStructureVM.StructureId);
            GeneratedStructuresVM.Remove(genStructureVM);
            _ea.GetEvent<DataValidationRequiredEvent>().Publish();
        }

        public ICommand AddTemplateStructureCommand
        {
            get
            {
                return new DelegateCommand(AddTemplateStructure);
            }
        }

        private void AddTemplateStructure(object obj)
        {
            TemplateStructureModel t = _templateModel.AddTemplateStructure();
            var tSVM = new TemplateStructureViewModel(t, _templateModel, _ea, true);
            TemplateStructuresVM.Add(tSVM);
            NotifyErrorsChanged(nameof(AddTemplateStructure));
            RaisePropertyChangedEvent(nameof(HasTemplateStructures));
        }

        public ICommand RemoveTemplateStructureCommand
        {
            get
            {
                return new DelegateCommand(ConfirmRemoveTemplateStructure);
            }
        }

        public bool HasGeneratedStructures
        {
            get
            {
                return _templateModel.GetGeneratedStructureIds().Count() > 0;
            }
        }

        private void ConfirmRemoveTemplateStructure(object obj)
        {
            var tSVM = obj as TemplateStructureViewModel;
            if (tSVM != null)
            {
                object[] RemoveStructureParam = new object[] { tSVM, _templateModel, TemplateStructuresVM, _ea };
                ConfirmRemoveTemplateStructureVM = new ConfirmationViewModel("Removing this will remove all operations referencing this structure. Continue?", new DelegateCommand(RemoveTemplateStructure), RemoveStructureParam);
                tSVM.ConfirmRemoveStructurePopupVisibility = true;
            }
            RaisePropertyChangedEvent(nameof(HasTemplateStructures));
        }
        private void RemoveTemplateStructure(object param)
        {
            var tSVM = (param as object[])[0] as TemplateStructureViewModel;
            var _templateModel = (param as object[])[1] as TemplateModel;
            var TemplateStructuresVM = (param as object[])[2] as ObservableCollection<TemplateStructureViewModel>;
            var _ea = (param as object[])[3] as IEventAggregator;
            _templateModel.RemoveTemplateStructure(tSVM.TemplateStructureId);
            TemplateStructuresVM.Remove(tSVM);
            _ea.GetEvent<DataValidationRequiredEvent>().Publish();

        }


        internal bool ValidateInputs(List<string> aggregateWarnings)
        {
            bool isValid = true;
            foreach (var structure in GeneratedStructuresVM)
            {
                if (!structure.ValidateInputs(aggregateWarnings))
                {
                    isValid = false;
                }
            }
            foreach (var structure in TemplateStructuresVM)
            {
                if (!structure.ValidateInputs(aggregateWarnings))
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        internal void ReorderTemplateStructures(int a, int b)
        {
            TemplateStructuresVM.Move(a, b);
            _templateModel.ReorderTemplateStructures(a, b);
        }

        internal void ReorderGenStructures(int a, int b)
        {
            GeneratedStructuresVM.Move(a, b);
            _templateModel.ReorderGeneratedStructures(a, b);
        }

        internal async Task<List<string>> GenerateStructures()
        {
            return await _templateModel.GenerateStructures();
        }



    }
}
