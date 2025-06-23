using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using OptiMate.Models;
using OptiMate.ViewModels;
using OptiMate.Logging;
using PropertyChanged;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Prism.Events;
using System.Windows.Navigation;


[assembly: ESAPIScript(IsWriteable = true)]

namespace OptiMate.ViewModels
{

    public class TemplatePointer
    {
        public string TemplateDisplayName { get; set; }
        public string TemplatePath { get; set; }
    }

    public class SometimesObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotfication = false;
        public bool SuppressNotification
        {
            get { return _suppressNotfication; }
            set
            {
                _suppressNotfication = value;
                if (value == false)
                    base.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }
        [SuppressPropertyChangedWarnings]
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotfication)
                base.OnCollectionChanged(e);
        }
    }

    public class DataValidationRequiredEvent : PubSubEvent { }

    [AddINotifyPropertyChangedInterface]

    public class ViewModel : ObservableObject
    {
        private MainModel _model;
        private IEventAggregator _ea = new EventAggregator();
        private Dispatcher _ui;
        public bool IsErrorAcknowledged { get; set; } = true;
        public string CurrentStructureSet { get; set; }

        public bool CanLoadTemplates { get; set; } = true;
        public bool CanSaveTemplates { get; private set; } = true;
        public bool PopupLock { get; private set; } = true;

        public bool StructureEditMode { get; private set; }

        private List<string> _warnings = new List<string>();

        private SolidColorBrush WarningColour = new SolidColorBrush(Colors.DarkOrange);
        public ObservableCollection<string> EclipseIds { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<TemplatePointer> TemplatePointers { get; set; } = new ObservableCollection<TemplatePointer>();

        public TemplatePointer _selectedTemplate;
        public TemplatePointer SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        _selectedTemplate = value;
                        InitializeProtocol(value);
                        GC.Collect();  // this seems to solve the problem of InstructionModel objects getting stuck in memory when the template is changed
                        GC.WaitForFullGCComplete();
                    }
                    else
                        StatusMessage = "Please select template...";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
                }
            }
        }

        private TemplateViewModel _activeTemplate;
        public TemplateViewModel ActiveTemplate
        {
            get { return _activeTemplate; }
            set
            {
                _activeTemplate = value;
            }
        }

        public SaveNewTemplateViewModel SaveTemplateVM { get; set; }

        public GeneratedStructureOptionsViewModel GeneratedStructureOptionsVM { get; set; }// generate it here due to length of the structure code list


        private void ValidateControls(object sender, DataErrorsChangedEventArgs e)
        {
            CheckAllInputsValid();
        }
        public bool HasTemplateStructures
        {
            get
            {
                if (ActiveTemplate != null && ActiveTemplate.HasTemplateStructures)
                    return true;
                else
                    return false;
            }
        }

        public bool HasTemplateStructuresWarning
        {
            get
            {
                if (ActiveTemplate != null && !ActiveTemplate.HasTemplateStructures)
                    return true;
                else
                    return false;
            }
        }

        public bool HasGeneratedStructures
        {
            get
            {
                if (ActiveTemplate != null && ActiveTemplate.HasGeneratedStructures)
                    return true;
                else
                    return false;
            }
        }

        public bool HasGeneratedStructuresWarning
        {
            get
            {
                if (ActiveTemplate != null && !ActiveTemplate.HasGeneratedStructures)
                    return true;
                else
                    return false;
            }
        }

        private bool _working;
        public bool Working
        {
            get
            {
                return _working;
            }
            set
            {
                _working = value;
                RaisePropertyChangedEvent(nameof(StartButtonText));
            }
        }

        public bool HasWarnings
        {
            get
            {
                return warnings.Count > 0;
            }
        }

        public string StartButtonText
        {
            get
            {
                if (_working)
                    return "Please wait...";
                else
                    return "Generate structures";
            }
        }

        public bool ScriptDone { get; set; } = false;

        public bool StatusMessageVisibility
        {
            get
            {
                return (!string.IsNullOrEmpty(StatusMessage) && Working == false);
            }
        }
        private string _statusMessage = @"Please select a template...";
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                RaisePropertyChangedEvent(nameof(StatusMessageVisibility));
            }
        }
        public string WaitMessage { get; set; }
        private List<string> warnings = new List<string>();
        public bool AllInputsValid { get; private set; }
        private void CheckAllInputsValid()
        {
            //ClearErrors(nameof(allInputsValid));
            warnings = new List<string>();
            if (ActiveTemplate != null)
            {
                if (ActiveTemplate.ValidateInputs(warnings))
                {
                    StatusMessage = "Ready...";
                    AllInputsValid= true;
                }
                else
                {
                    StatusMessage = "Please review input parameters before continuing, click for details...";
                    AllInputsValid = false;
                }
            }
            else
            {
                string error = "Unable to load template, please click for details...";
                warnings.Add(error);
                AllInputsValid = false;
                //AddError(nameof(allInputsValid), error);
            }
            RaisePropertyChangedEvent(nameof(HasWarnings));
            RaisePropertyChangedEvent(nameof(StatusBorderColor));
            RaisePropertyChangedEvent(nameof(Working));

        }
        public SolidColorBrush StatusBorderColor
        {
            get
            {
                return HasWarnings ? WarningColour : new SolidColorBrush(Colors.Transparent);
            }
        }

        public DescriptionViewModel ReviewWarningsVM { get; set; } = new DescriptionViewModel("Review generated structure warnings", "");

        public bool ReviewWarningsPopupVisibility { get; set; } = false;

        public bool ReviewInputValidationPopupVisibility { get; set; } = false;

        public ViewModel()
        {
            // These values only instantiated for XAML Design
            ActiveTemplate = new TemplateViewModel() { TemplateDisplayName = "Design" };
        }

        public ViewModel(MainModel model)
        {
            StartWait("Loading structures...");
            _ui = Dispatcher.CurrentDispatcher;
            _model = model;
            _model.SetEventAggregator(_ea);
            RegisterEvents();
            _model.Initialize();
            LoadStructureCodeNames();
        }

        private async void LoadStructureCodeNames()
        {
            GeneratedStructureOptionsVM = new GeneratedStructureOptionsViewModel(await _model.GetStructureCodeDisplayNames(), _ea);
        }

        private void RegisterEvents()
        {
            _ea.GetEvent<StructureGeneratingEvent>().Subscribe(UpdateStatus_GeneratingStructure);
            _ea.GetEvent<NewTemplateStructureEvent>().Subscribe(AddedTemplateStructure);
            _ea.GetEvent<NewGeneratedStructureEvent>().Subscribe(AddedGeneratedStructure);
            _ea.GetEvent<RemovedGeneratedStructureEvent>().Subscribe(RemovedGeneratedStructure);
            _ea.GetEvent<RemovedTemplateStructureEvent>().Subscribe(RemovedTemplateStructure);
            _ea.GetEvent<GeneratedStructureCleaningUpEvent>().Subscribe(UpdateStatus_TempStructureCleanup);
            _ea.GetEvent<DataValidationRequiredEvent>().Subscribe(CheckAllInputsValid);
            _ea.GetEvent<ModelInitializedEvent>().Subscribe(Initialize);
            _ea.GetEvent<TemplateSavedEvent>().Subscribe(OnTemplateSaved);
            _ea.GetEvent<StructureEditEvent>().Subscribe(OpenStructureEditWindow);
            _ea.GetEvent<LockingPopupEvent>().Subscribe(LockForPopup);
            _ea.GetEvent<StructureOptionsClosing>().Subscribe(CloseStructureEditWindow);
        }

        private void CloseStructureEditWindow()
        {
            StructureEditMode = false;
        }

        private void OpenStructureEditWindow(GeneratedStructureEditEventInfo info)
        {
            StructureEditMode = true;
            GeneratedStructureOptionsVM.SetGeneratedStructure(info.model);
        }

        private void RemovedGeneratedStructure(RemovedGeneratedStructureEventInfo info)
        {
            RefreshViewModelVisibility();
        }

        private void AddedGeneratedStructure(NewGeneratedStructureEventInfo info)
        {
            RefreshViewModelVisibility();
        }

        private void RemovedTemplateStructure(RemovedTemplateStructureEventInfo info)
        {
            RefreshViewModelVisibility();
        }

        private void AddedTemplateStructure(NewTemplateStructureEventInfo info)
        {
            RefreshViewModelVisibility();
        }

        private void UpdateStatus_TempStructureCleanup(string structureRemoving)
        {
            WaitMessage = $"Removing temporary structure {structureRemoving}...";
            SeriLogUI.AddLog(WaitMessage);
        }

        private void RefreshViewModelVisibility()
        {
            RaisePropertyChangedEvent(nameof(HasTemplateStructures));
            RaisePropertyChangedEvent(nameof(HasTemplateStructuresWarning));
            RaisePropertyChangedEvent(nameof(HasGeneratedStructures));
            RaisePropertyChangedEvent(nameof(HasGeneratedStructuresWarning));
        }

        private void LockForPopup(bool isLockingPopupActive)
        {
            PopupLock = !isLockingPopupActive;
        }

        private void OnTemplateSaved()
        {
            ReloadTemplates();
            _ea.GetEvent<StructureOptionsClosing>().Publish();
        }

        private void UpdateStatus_GeneratingStructure(StructureGeneratingEventInfo info)
        {
            WaitMessage = $"Creating {info.Structure.StructureId}... ({info.IndexInQueue+1}/{info.TotalToGenerate})";
            SeriLogUI.AddLog(WaitMessage);
        }

        private void InitializeProtocol(TemplatePointer value)
        {
            warnings.Clear();
            _ea.GetEvent<StructureOptionsClosing>().Publish();
            // Cleanup
            if (ActiveTemplate != null)
            {
                ActiveTemplate.Dispose();
            }
            var templateModel = _model.LoadTemplate(value);
            if (templateModel != null)
            {
                ActiveTemplate = new TemplateViewModel(templateModel, _ea);
                ValidateControls(null, null);
            }
            else
            {
                StatusMessage = "Unable to load template, please click info icon for details...";
                warnings.AddRange(_model.ValidationErrors);
                ActiveTemplate = null;
            }
            RaisePropertyChangedEvent(nameof(ActiveTemplate));
            RefreshViewModelVisibility();
        }
        private void StartWait(string message)
        {
            WaitMessage = message;
            Working = true;
            RaisePropertyChangedEvent(nameof(StatusMessageVisibility));
        }
        private void EndWait()
        {
            WaitMessage = "";
            Working = false;
            RaisePropertyChangedEvent(nameof(StatusMessageVisibility));
        }
        private void Initialize()
        {
            try
            {
                SeriLogUI.Initialize(_model.LogPath, _model.CurrentUser); //    Initialize logger to same directory 
                _ui.Invoke(() =>
                {
                    ReloadTemplates();
                    CurrentStructureSet = _model.StructureSetId;
                });
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Exception during ViewModel initialization: {0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace);
                MessageBox.Show(errorMessage);
            }
            EndWait();

        }

        private void ViewModel_ErrorAcknowledged(object sender, EventArgs e)
        {
            IsErrorAcknowledged = true;
        }

        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(Start);
            }
        }

        public ICommand ShowWarningsCommand
        {
            get
            {
                return new DelegateCommand(ShowWarnings);
            }
        }

        private void ShowWarnings(object obj)
        {
            ReviewWarningsPopupVisibility ^= true;
            if (ScriptDone)
            {
                ReviewWarningsVM.Id = "Review generated structure warnings";
                ReviewWarningsVM.Description = Helpers.HTMLWarningFormatter(warnings);
            }
            else
            {
                ReviewWarningsVM.Id = "Review input validation warnings";
                ReviewWarningsVM.Description = Helpers.HTMLWarningFormatter(warnings);
            }
        }

     
        public ICommand OpenTemplateFolderCommand
        {
            get
            {
                return new DelegateCommand(OpenTemplateFolder);
            }
        }

        public ICommand SaveAsPersonalCommand
        {
            get
            {
                return new DelegateCommand(SaveAsPersonal);
            }
        }

        private void SaveAsPersonal(object param = null)
        {
            if (ActiveTemplate == null)
            {
                StatusMessage = "No template loaded.";
            }
            else
            {
                SaveTemplateVM = new SaveNewTemplateViewModel(_ea, _model, ActiveTemplate.TemplateDisplayName);
                RaisePropertyChangedEvent(nameof(SaveTemplateVM));
                NewTemplateIdPopupVisibility ^= true;
            }
        }

        public bool NewTemplateIdPopupVisibility { get; set; } = false;
        public ICommand ReloadTemplateCommand
        {
            get
            {
                return new DelegateCommand(ReloadTemplates);
            }
        }

        public async void Start(object param = null)
        {
            if (Working)
            {
                return;
            }
            CanLoadTemplates = false;
            Working = true;
            ScriptDone = false;
            _warnings.Clear();
            try
            {
                (warnings) = await ActiveTemplate?.GenerateStructures();
                if (HasWarnings)
                {
                    StatusMessage = "Structures generated with warnings, click for details";
                }
                else
                {
                    StatusMessage = "Structures generated successfully";
                }
                WaitMessage = "";
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error during structure creation...");
                SeriLogUI.AddError(errorMessage);
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
                WaitMessage = errorMessage;
            }
            RaisePropertyChangedEvent(nameof(StatusBorderColor));
            RaisePropertyChangedEvent(nameof(HasWarnings));
            CanLoadTemplates = true;
            Working = false;
            ScriptDone = true;

        }


        public async void OpenTemplateFolder(object param = null)
        {
            try
            {
                await Task.Run(() => Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", _model.GetUserTemplatePath()));
            }
            catch (Exception ex)
            {
                SeriLogUI.AddError(string.Format("Error opening template folder: {0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
                MessageBox.Show(string.Format("Error opening template folder: {0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));

            }
        }

        public void ReloadTemplates(object param = null)
        {
            _ea.GetEvent<StructureOptionsClosing>().Publish();
            ActiveTemplate = null;
            warnings.Clear();
            RaisePropertyChangedEvent(nameof(HasWarnings));
            Working = true;
            ScriptDone = false;
            TemplatePointers.Clear();
            TemplatePointers.AddRange(_model.GetTemplates());
            Working = false;
            WaitMessage = "";
        }

     

    }
}
