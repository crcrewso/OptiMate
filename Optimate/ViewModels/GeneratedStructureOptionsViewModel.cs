using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using OptiMate.ViewModels;
using OptiMate.Logging;
using OptiMate.Models;
using System.Windows.Media;
using System.Collections.ObjectModel;
using PropertyChanged;
using OptiMate;
using System.Windows.Input;
using System.Collections;

namespace OptiMate.ViewModels
{
    public class GeneratedStructureOptionsViewModel : ObservableObject
    {

        private IGeneratedStructureModel _generatedStructure;

        private IEventAggregator _ea;

        public string StructureId
        {
            get
            {
                if (_generatedStructure != null)
                    return _generatedStructure.StructureId;
                else
                    return "Not loaded";
            }
        }
        public bool ClearFirst
        {
            get
            {
                if (_generatedStructure == null)
                {
                    return false;
                }
                else
                    return _generatedStructure.ClearFirst;
            }
            set
            {
                if (_generatedStructure == null)
                    return;
                else
                    _generatedStructure.ClearFirst = value;
            }
        }
        public Color StructureColor
        {
            get
            {
                if (_generatedStructure == null)
                {
                    return Colors.Transparent;
                }
                else
                    return _generatedStructure.StructureColor;
            }
            set
            {
                if (_generatedStructure == null)
                    return;
                else
                    _generatedStructure.StructureColor = value;
            }
        }

        public bool OverwriteColor
        {
            get
            {
                if (_generatedStructure == null)
                {
                    return false;
                }
                else
                    return _generatedStructure.OverwriteColor;
            }
            set
            {
                if (_generatedStructure == null)
                    return;
                else
                    _generatedStructure.OverwriteColor = value;
            }
        }
        public CleanupOptions CleanupOption
        {
            get
            {
                if (_generatedStructure == null)
                {
                    return CleanupOptions.None;
                }
                else
                    return _generatedStructure.CleanupOption;
            }
            set
            {
                if (_generatedStructure == null)
                    return;
                else
                    _generatedStructure.CleanupOption = value;
            }
        }

        public string StructureCodeDisplayName
        {
            get
            {
                if (_generatedStructure == null)
                {
                    return "";
                }
                else
                    return _generatedStructure.StructureCodeDisplayName;
            }
            set
            {
                if (_generatedStructure == null)
                    return;
                else
                    _generatedStructure.StructureCodeDisplayName = value;
            }
        }

        public List<string> StructureCodeDisplayNames { get; private set; }

        public GeneratedStructureOptionsViewModel(List<string> structureCodeDisplayNames, IEventAggregator ea)
        {
            StructureCodeDisplayNames = structureCodeDisplayNames;
            _ea = ea;
        }

        public ObservableCollection<CleanupOptions> CleanupOptionsList { get; set; } = new ObservableCollection<CleanupOptions> { OptiMate.CleanupOptions.None, OptiMate.CleanupOptions.WhenEmpty, OptiMate.CleanupOptions.Always };

        public void SetGeneratedStructure(IGeneratedStructureModel generatedStructure)
        {
            _generatedStructure = generatedStructure;
            RaisePropertyChangedEvent(nameof(StructureColor));
            RaisePropertyChangedEvent(nameof(StructureCodeDisplayName));
            RaisePropertyChangedEvent(nameof(ClearFirst));
            RaisePropertyChangedEvent(nameof(StructureId));
            RaisePropertyChangedEvent(nameof(CleanupOption));
            
        }

       public ICommand CloseWindowCommand
        {
            get
            {
                return new DelegateCommand(CloseWindow);
            }
        }

        public void CloseWindow(object param = null)
        {
            _ea.GetEvent<StructureOptionsClosing>().Publish();
        }

    }


}
