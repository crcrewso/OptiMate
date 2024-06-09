using OptiMate.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OptiMate.ViewModels
{
    public class EclipseSelectedStructure
    {
        public bool IsSelected { get; set; }
        public EclipseStructureProperties StructureProperties { get; set; }

        public string StructureId
        {
            get { return StructureProperties.StructureId; }
        }

    }

    public enum EclipseStructureImportMode
    {
        NotSet,
        Template,
        Generated
    }

    public class EclipseStructureSelectionViewModel
    {
        public ObservableCollection<EclipseSelectedStructure> EclipseStructures { get; set; }
        public EclipseSelectedStructure SelectedStructure { get; set; }

        private TemplateViewModel _viewModel;
        private TemplateModel _templateModel;
        private EclipseStructureImportMode _importMode = EclipseStructureImportMode.NotSet;
        private IEventAggregator _ea;

        public EclipseStructureImportMode ImportMode
        {
            get { return _importMode; }
            set 
            { 
                _importMode = value; 
                foreach (var structure in EclipseStructures)
                {
                    structure.IsSelected = false;
                }
            }
        }
        public EclipseStructureSelectionViewModel()
        {
            // 
        }

        public EclipseStructureSelectionViewModel(TemplateModel templateModel, TemplateViewModel viewModel, IEventAggregator ea)
        {
            EclipseStructures = new ObservableCollection<EclipseSelectedStructure>();
            _templateModel = templateModel;
            _viewModel = viewModel;
            _ea = ea;
            PopulateList();
        }

        private async void PopulateList()
        {
            List<EclipseStructureProperties> structures = await _templateModel.GetEclipseStructureProperties();
            foreach (var structure in structures.OrderBy(x=>x.StructureId))
            {
                EclipseStructures.Add(new EclipseSelectedStructure() { StructureProperties = structure });
            }
        }

        public ICommand ImportCommand
        {
            get 
            {
                return new DelegateCommand(ImportStructure);
            }
        }
        private void ImportStructure(object obj)
        {
            (obj as Popup).IsOpen = false;
            foreach (var structure in EclipseStructures)
            {
                if (structure.IsSelected)
                {
                    switch (ImportMode)
                    {
                        case EclipseStructureImportMode.Template:
                            var tsm = _templateModel.AddTemplateStructure(structure.StructureProperties.StructureId);
                            var tsvm = new TemplateStructureViewModel(tsm, _templateModel, _ea);
                            _viewModel.TemplateStructuresVM.Add(tsvm);
                            break;
                        case EclipseStructureImportMode.Generated:
                            var gsm = _templateModel.AddGeneratedStructure(structure.StructureProperties);
                            var gsvm = new GeneratedStructureViewModel(gsm, _ea);
                            _viewModel.GeneratedStructuresVM.Add(gsvm);
                            break;
                        default:
                            throw new Exception("Import mode not set");
                    }
                   
                }
            }
            ImportMode = EclipseStructureImportMode.NotSet; // reset
        }
    }
}
