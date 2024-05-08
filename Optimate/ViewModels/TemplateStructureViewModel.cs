using OptiMate;
using OptiMate.ViewModels;
using OptiMate.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OptiMate.Logging;
using OptiMate.Controls;
using System.Windows.Input;
using PropertyChanged;

namespace OptiMate.ViewModels
{
    public class TemplateStructureViewModel : ObservableObject
    {
        private TemplateModel _templateModel;
        private ITemplateStructureModel _templateStructureModel;
        private IEventAggregator _ea;

        public ObservableCollection<EclipseStructureViewModel> EclipseIds { get; set; } = new ObservableCollection<EclipseStructureViewModel>()
        {
            new EclipseStructureViewModel(){EclipseId = "DesignEclipseId1" },
            new EclipseStructureViewModel(){EclipseId = "DesignEclipseId1" }
        };

        public ObservableCollection<string> Aliases { get; private set; } = new ObservableCollection<string>();

        public EditControlViewModel EditControlVM { get; set; }
        public string PrincipalAlias
        {
            get
            {
                if (Aliases.Count > 0)
                {
                    return Aliases[0];
                }
                else
                {
                    return "";
                }
            }
        }

        public string TemplateStructureId
        {
            get
            {
                return _templateStructureModel.TemplateStructureId;
            }
            set
            {
                _templateStructureModel.TemplateStructureId = value;
                RaisePropertyChangedEvent(nameof(StructureIdColor));
            }
        }

        public bool PerformSmallVolumeCheck
        {
            get { return _templateStructureModel.PerformSmallVolumeCheck; }
            set
            {
                _templateStructureModel.PerformSmallVolumeCheck = value;
            }
        }

        private EclipseStructureViewModel _selectedEclipseStructure;
        public EclipseStructureViewModel SelectedEclipseStructure
        {
            get
            {
                return _selectedEclipseStructure;
            }
            set
            {
                if (value != _selectedEclipseStructure)
                {
                    ClearErrors(nameof(SelectedEclipseStructure));
                    _selectedEclipseStructure = value;
                    _templateStructureModel.EclipseStructureId = value.EclipseId;
                    if (string.IsNullOrEmpty(_selectedEclipseStructure.EclipseId))
                    {
                        AddError(nameof(SelectedEclipseStructure), $"Please provide mapping for {TemplateStructureId}.");
                    }
                    RaisePropertyChangedEvent(nameof(TemplateStructureWarningVisibility));
                    RaisePropertyChangedEvent(nameof(MappedStructureWarningColor));
                    RunSmallVolumeCheck();
                    _ea.GetEvent<DataValidationRequiredEvent>().Publish();
                }
            }
        }

        private async void RunSmallVolumeCheck()
        {
            if (PerformSmallVolumeCheck)
            {
                var results = await _templateStructureModel.CheckSmallVolumes();
                if (results.Success && results.HasSmallVolumes)
                {
                    SmallVolumeDetected = true;
                    var smallVolumeWarningText = new List<string>();
                    foreach (var contour in results.SmallVolumesLocation)
                    {
                        smallVolumeWarningText.Add($"Centroid: X = {contour.x:0.0} cm, Y = {contour.y:0.0} cm, Z = {contour.z:0.0} cm");
                    }
                    ReviewSmallVolumeWarningsVM.Description = Helpers.HTMLWarningFormatter(smallVolumeWarningText);
                    ReviewSmallVolumeWarningsVisibility = Visibility.Visible;
                }
                else
                {
                    SmallVolumeDetected = false;
                    ReviewSmallVolumeWarningsVM.Description = "";
                    ReviewSmallVolumeWarningsVisibility = Visibility.Collapsed;
                }
            }
        }

        public Visibility ReviewSmallVolumeWarningsVisibility { get; set; } = Visibility.Collapsed;
        public bool ReviewSmallVolumeWarningsPopupVisibility { get; set; }
        public DescriptionViewModel ReviewSmallVolumeWarningsVM { get; set; } = new DescriptionViewModel("Small contours detected at the following locations:", "");

        public ICommand ReviewSmallVolumeWarningsCommand
        {
            get
            {
                return new DelegateCommand(ReviewSmallVolumeWarnings);
            }
        }

        private void ReviewSmallVolumeWarnings(object obj)
        {
            ReviewSmallVolumeWarningsPopupVisibility = true;
        }

        public bool isNew { get; set; }
        public SolidColorBrush StructureIdColor
        {
            get
            {
                if (HasError(nameof(TemplateStructureId)))
                {
                    return new SolidColorBrush(Colors.Orange);
                }
                else
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public SolidColorBrush MappedStructureWarningColor
        {
            get
            {
                if (HasError(nameof(SelectedEclipseStructure)))
                {
                    return new SolidColorBrush(Colors.Orange);
                }
                else
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
            }
        }

        public Visibility TemplateStructureWarningVisibility
        {
            get
            {
                if (SelectedEclipseStructure == null)
                {
                    return Visibility.Collapsed;
                }
                if (_templateModel.IsEmpty(SelectedEclipseStructure.EclipseId))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public bool ConfirmRemoveStructurePopupVisibility { get; set; } = false;

        public TemplateStructureViewModel(TemplateStructureModel templateStructure, TemplateModel model, IEventAggregator ea, bool isNew = false)
        {
            _templateStructureModel = templateStructure;
            _templateModel = model;
            _ea = ea;
            RegisterEvents();
            this.isNew = isNew;
            EclipseIds.Clear();
            bool isMapped = false;
            foreach (var Id in _templateModel.GetEclipseStructureIds())
            {
                var esVM = new EclipseStructureViewModel(Id, _templateModel.IsEmpty(Id));
                EclipseIds.Add(esVM);
                if (_templateStructureModel.EclipseStructureId == Id)
                {
                    SelectedEclipseStructure = esVM;
                    isMapped = true;
                }
            }
            if (!isMapped)
            {
                AddError(nameof(SelectedEclipseStructure), $"Please provide mapping for {TemplateStructureId}.");
                RaisePropertyChangedEvent(nameof(MappedStructureWarningColor));
            }
            ea.GetEvent<StructureGeneratedEvent>().Subscribe(OnNewStructureCreated);
            RaisePropertyChangedEvent(nameof(MappedStructureWarningColor));
        }

        private void RegisterEvents()
        {
            ErrorsChanged += (sender, args) =>
            {
                _ea.GetEvent<DataValidationRequiredEvent>().Publish();
            };
            _ea.GetEvent<TemplateStructureIdChangedEvent>().Subscribe(OnTemplateStructureIdChanged);
        }
        [SuppressPropertyChangedWarnings]
        private void OnTemplateStructureIdChanged(TemplateStructureIdChangedEventInfo info)
        {
            if (string.Equals(info.NewId, TemplateStructureId, StringComparison.OrdinalIgnoreCase))
            {
                RaisePropertyChangedEvent(nameof(TemplateStructureId));
            }
        }
        [SuppressPropertyChangedWarnings]
        private void OnNewStructureCreated(StructureGeneratedEventInfo info)
        {
            if (!EclipseIds.Select(x => x.EclipseId).Contains(info.Structure.StructureId))
            {
                EclipseIds.Add(new EclipseStructureViewModel(info.Structure.StructureId, _templateModel.IsEmpty(info.Structure.StructureId)));
            }
        }

        internal bool ValidateInputs(List<string> aggregateWarnings)
        {
            if (HasErrors)
            {
                aggregateWarnings.AddRange(GetAllErrors());
                return false;
            }
            else
                return true;
        }

        public TemplateStructureViewModel()
        {
            //Design only
            _templateStructureModel = new TemplateStructureModelTest() { TemplateStructureId = "DesignTemplateId", Aliases = new List<string>() { "DesignAlias" } };
        }

        private bool _editTemplateStructurePopupVisibility = false;
        public bool EditTemplateStructurePopupVisibility
        {
            get
            {
                return _editTemplateStructurePopupVisibility;
            }
            set
            {
                _editTemplateStructurePopupVisibility = value;
                _ea.GetEvent<LockingPopupEvent>().Publish(value);
            }
        }

        public ICommand EditTemplateStructureCommand
        {
            get
            {
                return new DelegateCommand(EditTemplateStructure);
            }
        }

        public bool SmallVolumeDetected { get; private set; }

        private void EditTemplateStructure(object obj)
        {
            string templateStructureId = obj as string;
            EditTemplateStructurePopupVisibility ^= true;
            EditControlVM = new EditControlViewModel(templateStructureId, _templateModel.GetTemplateStructureModel(templateStructureId), _ea);
        }
    }
}
