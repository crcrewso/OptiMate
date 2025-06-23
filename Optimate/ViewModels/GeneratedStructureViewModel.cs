using OptiMate;
using OptiMate.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using Prism.Events;
using OptiMate.ViewModels;
using OptiMate.Logging;
using System.Windows;

namespace OptiMate.ViewModels
{


    public class GeneratedStructureModelTest : IGeneratedStructureModel
    {
        public string StructureId { get; set; }
        public bool ClearFirst { get; set; }
        public bool IsTemporary { get; set; }
        public Color StructureColor { get; set; }

        public bool isStructureIdValid => throw new NotImplementedException();

        public bool OverwriteColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CleanupOptions CleanupOption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public InstructionModel AddInstruction(OperatorTypes op, int index) { return null; }
        public InstructionModel ReplaceInstruction(Instruction inst, OperatorTypes op) { return null; }
        public void RemoveInstruction(Instruction instruction) { }

        public int GetInstructionNumber(InstructionModel instruction) { return 0; }
        public List<IInstructionModel> GetInstructions()
        {
            return new List<IInstructionModel>();
        }

        public string StructureCodeDisplayName { get; set; }

        public Color GetStructureColor()
        {
            throw new NotImplementedException();
        }

        public int GetInstructionNumber(IInstructionModel instruction)
        {
            throw new NotImplementedException();
        }

        public InstructionModel ReplaceInstruction(IInstructionModel instruction, OperatorTypes selectedOperator)
        {
            throw new NotImplementedException();
        }

        public bool GenStructureWillBeEmpty()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetStructureCodeDisplayNames()
        {
            throw new NotImplementedException();
        }

        
    }

    public class GeneratedStructureViewModel : ObservableObject
    {
        private IGeneratedStructureModel _generatedStructureModel;
        private IEventAggregator _ea;
        public GeneratedStructureViewModel(GeneratedStructureModel genStructureModel, IEventAggregator ea, bool isNew = false)
        {
            _generatedStructureModel = genStructureModel;
            EditMode = isNew;
            //_model = model;
            _ea = ea;
            foreach (var instructionModel in _generatedStructureModel.GetInstructions())
            {
                InstructionViewModels.Add(new InstructionViewModel(instructionModel, _generatedStructureModel, _ea));
            }
            RegisterEvents();
        }

       
        public GeneratedStructureViewModel()
        {
            _generatedStructureModel = new GeneratedStructureModelTest()
            {
                StructureId = "DesignStructure1",
                ClearFirst = true,
                IsTemporary = false,
                StructureColor = Colors.Black
            };
            InstructionViewModels = new ObservableCollection<InstructionViewModel>();
            EditMode = true;
            InstructionViewModels.Add(new InstructionViewModel());
            InstructionViewModels.Add(new InstructionViewModel());
            InstructionViewModels.Add(new InstructionViewModel());
            InstructionViewModels.Add(new InstructionViewModel());
            InstructionViewModels.Add(new InstructionViewModel());
            InstructionViewModels.Add(new InstructionViewModel());

        }

        private void RegisterEvents()
        {
            _ea.GetEvent<RemovingInstructionViewModelEvent>().Subscribe(OnRemovingInstructionViewModel);
        }

        private void OnRemovingInstructionViewModel(InstructionViewModel ivm)
        {
            if (InstructionViewModels.Contains(ivm))
            {
                InstructionViewModels.Remove(ivm);
                GC.Collect();
                GC.WaitForFullGCComplete();
                RaisePropertyChangedEvent(nameof(InstructionViewModels));
                _ea.GetEvent<DataValidationRequiredEvent>().Publish();
            }
        }

        public bool OverwriteColor
        {
            get
            {
                return _generatedStructureModel.OverwriteColor;
            }
            set
            {
                if (value != _generatedStructureModel.OverwriteColor)
                {
                    _generatedStructureModel.OverwriteColor = value;
                    isModified = true;
                }
            }
        }

        public List<string> StructureCodeDisplayNames { get; set; } = new List<string>();
        public bool StructureCodeDisplayNameVisibility { get; set; } = false;
      
        public bool EditMode { get; set; } = false;
        public bool isModified { get; private set; } = false;

       

        public string StructureId
        {
            get
            {
                return _generatedStructureModel.StructureId;
            }
            set
            {
                if (value != _generatedStructureModel.StructureId)
                {
                    _generatedStructureModel.StructureId = value;
                    if (!_generatedStructureModel.isStructureIdValid)
                    {
                        AddError(nameof(StructureId), $"Id of generated structure {value} is invalid. Please correct.");
                    }
                    else
                    {
                        RemoveError(nameof(StructureId));
                    }
                    isModified = true;
                    RaisePropertyChangedEvent(nameof(isStructureIdValid));
                    RaisePropertyChangedEvent(nameof(StructureIdColor));
                    RaisePropertyChangedEvent(nameof(WarningVisibility_GenStructureChanged));
                    _ea.GetEvent<DataValidationRequiredEvent>().Publish();
                }
            }
        }

        public bool ClearFirst
        {
            get
            {

                return _generatedStructureModel.ClearFirst;
            }
            set
            {
                if (value != _generatedStructureModel.ClearFirst)
                {
                    _generatedStructureModel.ClearFirst = value;
                    isModified = true;
                    RaisePropertyChangedEvent(nameof(WarningVisibility_GenStructureChanged));
                }
            }
        }

        

        public CleanupOptions CleanupOption
        {
            get
            {
                return _generatedStructureModel.CleanupOption;
            }
            set
            {
                if (value != _generatedStructureModel.CleanupOption)
                {
                    _generatedStructureModel.CleanupOption = value;
                    isModified = true;
                    RaisePropertyChangedEvent(nameof(WarningVisibility_GenStructureChanged));
                }
            }
        }

        
        public ObservableCollection<InstructionViewModel> InstructionViewModels { get; set; } = new ObservableCollection<InstructionViewModel>();

        public bool isStructureIdValid
        {
            get
            {
                var temp = !HasError(nameof(StructureId));
                return !HasError(nameof(StructureId));
            }
        }

        public SolidColorBrush StructureIdColor
        {
            get
            {
                if (HasError(nameof(StructureId)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }
        public string StructureIdError
        {
            get
            {
                var errors = GetErrors(nameof(StructureId));
                if (errors == null)
                    return null;
                else
                    return string.Join("\r", (List<string>)errors);
            }
        }

        public Color StructureColor
        {
            get
            {
                return _generatedStructureModel.StructureColor;
            }
            set
            {
                if (value != _generatedStructureModel.StructureColor)
                {
                    _generatedStructureModel.StructureColor = value;
                    isModified = true;
                    RaisePropertyChangedEvent(nameof(StructureColor));
                }
            }
        }


        public bool ConfirmRemoveStructurePopupVisibility { get; set; }

        public ICommand AddInstructionCommand
        {
            get
            {
                return new DelegateCommand(AddInstruction);
            }
        }
        public void AddInstruction(object param = null)
        {
            var priorInstruction = (param as object[])[0] as InstructionViewModel;
            int index;
            try
            {
                index = InstructionViewModels.IndexOf(priorInstruction);
            }
            catch (Exception e)
            {
                SeriLogModel.AddError("Could not find prior instruction when inserting new instruction, using index=0", e);
                index = 0;
            }
            var newInstructionModel = _generatedStructureModel.AddInstruction(OperatorTypes.undefined, index + 1);
            InstructionViewModels.Insert(index + 1, new InstructionViewModel(newInstructionModel, _generatedStructureModel, _ea));
            RaisePropertyChangedEvent(nameof(InstructionViewModels));
            _ea.GetEvent<DataValidationRequiredEvent>().Publish();
            SeriLogModel.AddLog($"Added new instruction to {_generatedStructureModel.StructureId}");
        }


        public int NumInstructions
        {
            get
            {
                return InstructionViewModels.Count;
            }
        }



        public ICommand EditGenStructureCommand
        {
            get
            {
                return new DelegateCommand(ToggleEditMode);
            }
        }

       

        private void ToggleEditMode(object obj = null)
        {
            if (!HasErrors)
            {
                EditMode ^= true;
                if (!EditMode)
                    _ea.GetEvent<StructureOptionsClosing>().Publish();
            }
        }


        public ICommand OpenOptionsCommand
        {
            get
            {
                return new DelegateCommand(OpenOptions);
            }
        }

        private void OpenOptions(object obj = null)
        {
            if (!HasErrors)
            {
                _ea.GetEvent<StructureEditEvent>().Publish(new GeneratedStructureEditEventInfo() { model = _generatedStructureModel, EditModeActive = EditMode });
            }
        }

        public Visibility WarningVisibility_GenStructureChanged
        {
            get
            {
                return (hasWarnings || isModified) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool hasWarnings = false;

        internal bool ValidateInputs(List<string> aggregateWarnings)
        {
            bool isValid = true;
            hasWarnings = false;
            ClearErrors(nameof(ValidateInputs));
            if (HasErrors)
            {
                aggregateWarnings.AddRange(GetAllErrors());
                isValid = false;
                hasWarnings = true;
            }
            foreach (var ivm in InstructionViewModels)
            {
                if (ivm.HasErrors)
                {
                    aggregateWarnings.AddRange(ivm.GetAllErrors());
                    isValid = false;
                    hasWarnings = true;
                    AddError(nameof(ValidateInputs), "One or more instructions have warnings or errors");
                }
                if (ivm.isModified)
                {
                    hasWarnings = true;
                }
            }
            if (!isValid)
                EditMode = true;
            RaisePropertyChangedEvent(nameof(WarningVisibility_GenStructureChanged));
            return isValid;
        }
    }

}
