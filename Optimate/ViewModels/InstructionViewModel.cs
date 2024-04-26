using OptiMate;
using OptiMate.ViewModels;
using OptiMate.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static OptiMate.ViewModels.TemplateStructureViewModel;
using System.Windows.Input;

namespace OptiMate.ViewModels
{
    public enum OperatorTypes
    {
        undefined,
        convertResolution,
        margin,
        asymmetricMargin,
        or,
        and,
        crop,
        asymmetricCrop,
        sub,
        subfrom,
        convertDose,
        partition,
        setHU
    }

    public class RemovingInstructionViewModelEvent : PubSubEvent<InstructionViewModel> { }

    public class InstructionViewModel : ObservableObject
    {
        private IEventAggregator _ea;
        private IInstructionModel _instructionModel;
        private IGeneratedStructureModel _parentGeneratedStructure;
        public SometimesObservableCollection<OperatorTypes> Operators { get; set; } = new SometimesObservableCollection<OperatorTypes>()
        {
              OperatorTypes.convertResolution,
              OperatorTypes.margin,
              OperatorTypes.asymmetricMargin,
              OperatorTypes.or,
              OperatorTypes.and,
              OperatorTypes.crop,
              OperatorTypes.asymmetricCrop,
              OperatorTypes.sub,
              OperatorTypes.subfrom,
              OperatorTypes.convertDose,
              OperatorTypes.partition,
              OperatorTypes.setHU
        };
        public SometimesObservableCollection<MarginTypes> MarginTypeOptions { get; set; } = new SometimesObservableCollection<MarginTypes>()
        {
            MarginTypes.Outer,
            MarginTypes.Inner
        };

        private OperatorTypes _selectedOperator;
        public OperatorTypes SelectedOperator
        {
            get
            {
                return _selectedOperator;
            }
            set
            {
                if (_selectedOperator != value)
                {
                    isModified = true;
                    _instructionModel = _instructionModel.ReplaceInstruction(value);
                    InitializeViewModel();
                    _selectedOperator = value; // order of setter matters because we don't want to invoke property changed events before the instruction is updated
                    ValidateOperator();
                }
            }
        }

        private string _HUValue;
        public string HUValue
        {

            get
            {
                return _HUValue;
            }
            set
            {
                _HUValue = value;
                bool error = true;
                if (short.TryParse(value, out short result))
                {
                    _instructionModel.HUValue = result;
                    if (_instructionModel.IsHUValueValid && result == _instructionModel.HUValue)
                    {
                        ClearErrors(nameof(HUValue));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(HUValue), "Invalid HU value");
                }
                RaisePropertyChangedEvent(nameof(HUValueBackgroundColor));
            }
        }

        public SolidColorBrush HUValueBackgroundColor
        {
            get
            {
                if (HasError(nameof(HUValue)))
                {
                    return Brushes.DarkOrange;
                }
                else
                {
                    return Brushes.LightGoldenrodYellow;
                }
            }
        }

        private string _doseLevel;
        public string DoseLevel
        {
            get
            {
                return _doseLevel;
            }
            set
            {
                _doseLevel = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.DoseLevel = result;
                    if (result == _instructionModel.DoseLevel && _instructionModel.IsDoseLevelValid)
                    {
                        ClearErrors(nameof(DoseLevel));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(DoseLevel), $"Invalid dose level value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(DoseLevelBackgroundColor));
            }
        }

        public SolidColorBrush DoseLevelBackgroundColor
        {
            get
            {
                if (HasError(nameof(DoseLevel)))
                {
                    return Brushes.DarkOrange;
                }
                else
                {
                    return Brushes.LightGoldenrodYellow; 
                }
            }
        }

        public string marginValueToolTip
        {
            get
            {
                return "Enter a value between 0 and 50";
            }
        }
        private string _antMargin;
        public string AntMargin
        {
            get
            {
                return _antMargin;
            }
            set
            {
                _antMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.AntMargin = result;
                    if (_instructionModel.IsAntMarginValid && result == _instructionModel.AntMargin)
                    {
                        ClearErrors(nameof(AntMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(AntMargin), $"Invalid ant margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(AntMarginBackgroundColor));
            }
        }
        private string _postMargin;
        public string PostMargin
        {
            get
            {
                return _postMargin;
            }
            set
            {
                _postMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.PostMargin = result;
                    if (_instructionModel.IsPostMarginValid && result == _instructionModel.PostMargin)
                    {
                        ClearErrors(nameof(PostMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(PostMargin), $"Invalid posterior margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(PostMarginBackgroundColor));
            }
        }
        private string _supMargin;
        public string SupMargin
        {
            get
            {
                return _supMargin;
            }
            set
            {
                _supMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.SupMargin = result;
                    if (_instructionModel.IsSupMarginValid && result == _instructionModel.SupMargin)
                    {
                        ClearErrors(nameof(SupMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(SupMargin), $"Invalid superior margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(SupMarginBackgroundColor));
            }
        }
        private string _infMargin;
        public string InfMargin
        {
            get
            {
                return _infMargin;
            }
            set
            {
                _infMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.InfMargin = result;
                    if (_instructionModel.IsInfMarginValid && result == _instructionModel.InfMargin)
                    {
                        ClearErrors(nameof(InfMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(InfMargin), $"Invalid inferior margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(InfMarginBackgroundColor));
            }
        }
        private string _leftMargin;
        public string LeftMargin
        {
            get
            {
                return _leftMargin;
            }
            set
            {
                _leftMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.LeftMargin = result;
                    if (_instructionModel.IsLeftMarginValid && result == _instructionModel.LeftMargin)
                    {
                        ClearErrors(nameof(LeftMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(LeftMargin), $"Invalid left margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(LeftMarginBackgroundColor));
            }
        }
        private string _rightMargin;
        public string RightMargin
        {
            get
            {
                return _rightMargin;
            }
            set
            {
                _rightMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.RightMargin = result;
                    if (_instructionModel.IsRightMarginValid && result == _instructionModel.RightMargin)
                    {
                        ClearErrors(nameof(RightMargin));
                        error = false;
                        isModified = true;
                    }
                }
                if (error)
                {
                    AddError(nameof(RightMargin), $"Invalid right margin value for generated structure {_parentGeneratedStructure.StructureId}");
                }
                RaisePropertyChangedEvent(nameof(RightMarginBackgroundColor));
            }
        }


        private string _isoMargin;
        public string IsoMargin
        {
            get
            {
                return _isoMargin;
            }
            set
            {
                _isoMargin = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.IsoMargin = result;
                    if (_instructionModel.IsIsoMarginValid && result == _instructionModel.IsoMargin)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(IsoMargin));
                    }
                }
                if (error)
                    AddError(nameof(IsoMargin), $"Invalid margin value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(IsoMarginBackgroundColor));
            }

        }

        private string _supBound;
        public string SupBound
        {
            get
            {
                return _supBound;
            }
            set
            {
                _supBound = value;
                bool error = true;
                if (double.TryParse(value, out double result))
                {
                    _instructionModel.SupBound = result;
                    if (_instructionModel.IsSupBoundValid && result == _instructionModel.SupBound)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(SupBound));
                    }
                }
                if (error)
                    AddError(nameof(SupBound), $"Invalid superior bound value for generated structure {_parentGeneratedStructure.StructureId}");
                //RaisePropertyChangedEvent(nameof(IsoMarginBackgroundColor));
            }

        }

        private string _infBound;
        public string InfBound
        {
            get
            {
                return _infBound;
            }
            set
            {
                _infBound = value;
                bool error = true;
                if (double.TryParse(value, out double result))
                {
                    _instructionModel.InfBound = result;
                    if (_instructionModel.IsInfBoundValid && result == _instructionModel.InfBound)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(InfBound));
                    }
                }
                if (error)
                    AddError(nameof(InfBound), $"Invalid inferior bound value for generated structure {_parentGeneratedStructure.StructureId}");
                //RaisePropertyChangedEvent(nameof(IsoMarginBackgroundColor));
            }

        }

        public MarginTypes? SelectedMargin
        {
            get
            {
                return _instructionModel.MarginType;
            }
            set
            {
                _instructionModel.MarginType = value;
                isModified = true;
            }
        }


        public Visibility MarginVisibility
        {
            get
            {
                switch (SelectedOperator)
                {
                    case OperatorTypes.margin:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public Visibility DeleteButtonVisibiilty
        {
            get
            {
                int index = _parentGeneratedStructure.GetInstructionNumber(_instructionModel);
                if (index == 0)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public Visibility AsymmetricMarginVisibility
        {
            get
            {
                switch (SelectedOperator)
                {
                    case OperatorTypes.asymmetricMargin:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public bool? InternalCrop
        {
            get
            {
                return _instructionModel.InternalCrop;
            }
            set
            {
                if (value != _instructionModel.InternalCrop)
                {
                    _instructionModel.InternalCrop = value;
                    isModified = true;
                }
            }
        }

        private bool _isModified = false;
        public bool isModified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = value;
                RaisePropertyChangedEvent(nameof(WarningVisibility_OperatorChanged));
                _ea.GetEvent<DataValidationRequiredEvent>().Publish();
            }
        }
        private string _isoCropOffset;
        public string IsoCropOffset
        {
            get
            {
                return _isoCropOffset;
            }
            set
            {
                _isoCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.IsoCropOffset = result;
                    if (_instructionModel.IsIsoCropOffsetValid && result == _instructionModel.IsoCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(IsoCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(IsoCropOffset), $"Invalid iso crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(IsoCropOffsetBackgroundColor));
            }
        }

        private string _leftCropOffset;
        public string LeftCropOffset
        {
            get
            {
                return _leftCropOffset;
            }
            set
            {
                _leftCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.LeftCropOffset = result;
                    if (_instructionModel.IsLeftCropOffsetValid && result == _instructionModel.LeftCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(LeftCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(LeftCropOffset), $"Invalid left crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(LeftCropOffsetBackgroundColor));
            }
        }
        private string _rightCropOffset;
        public string RightCropOffset
        {
            get
            {
                return _rightCropOffset;
            }
            set
            {
                _rightCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.RightCropOffset = result;
                    if (_instructionModel.IsRightCropOffsetValid && result == _instructionModel.RightCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(RightCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(RightCropOffset), $"Invalid right crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(RightCropOffsetBackgroundColor));
            }
        }

        private string _antCropOffset;

        public string AntCropOffset
        {
            get
            {
                return _antCropOffset;
            }
            set
            {
                _antCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.AntCropOffset = result;
                    if (_instructionModel.IsAntCropOffsetValid && result == _instructionModel.AntCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(AntCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(AntCropOffset), $"Invalid anterior crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(AntCropOffsetBackgroundColor));
            }

        }

        private string _postCropOffset;
        public string PostCropOffset
        {
            get
            {
                return _postCropOffset;
            }
            set
            {
                _postCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.PostCropOffset = result;
                    if (_instructionModel.IsPostCropOffsetValid && result == _instructionModel.PostCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(PostCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(PostCropOffset), $"Invalid posterior crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(PostCropOffsetBackgroundColor));
            }
        }

        private string _infCropOffset;
        public string InfCropOffset
        {
            get
            {
                return _infCropOffset;
            }
            set
            {
                _infCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.InfCropOffset = result;
                    if (_instructionModel.IsInfCropOffsetValid && result == _instructionModel.InfCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(InfCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(InfCropOffset), $"Invalid inferior crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(InfCropOffsetBackgroundColor));
            }
        }

        // apply this change to supcropoffset
        private string _supCropOffset;
        public string SupCropOffset
        {
            get
            {
                return _supCropOffset;
            }
            set
            {
                _supCropOffset = value;
                bool error = true;
                if (ushort.TryParse(value, out ushort result))
                {
                    _instructionModel.SupCropOffset = result;
                    if (_instructionModel.IsSupCropOffsetValid && result == _instructionModel.SupCropOffset)
                    {
                        isModified = true;
                        error = false;
                        ClearErrors(nameof(SupCropOffset));
                    }
                }
                if (error)
                    AddError(nameof(SupCropOffset), $"Invalid superior crop offset value for generated structure {_parentGeneratedStructure.StructureId}");
                RaisePropertyChangedEvent(nameof(SupCropOffsetBackgroundColor));
            }
        }

        public Visibility CropVisibility
        {
            get
            {
                switch (SelectedOperator)
                {
                    case OperatorTypes.crop:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public string DefaultTemplateStructureId
        {
            get { return _instructionModel.DefaultInstructionTargetId; }
            set
            {
                _instructionModel.DefaultInstructionTargetId = value;
                isModified = true;
            }
        }

        public Visibility WarningVisibility_TargetChanged
        {
            get
            {
                if (isModified)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility VisibilityTargetSelection
        {
            get
            {
                switch (_selectedOperator)
                {
                    case OperatorTypes.margin:
                        return Visibility.Hidden;
                    case OperatorTypes.asymmetricMargin:
                        return Visibility.Hidden;
                    case OperatorTypes.convertResolution:
                        return Visibility.Hidden;
                    case OperatorTypes.convertDose:
                        return Visibility.Hidden;
                    case OperatorTypes.partition:
                        return Visibility.Hidden;
                    case OperatorTypes.setHU:
                        return Visibility.Hidden;
                    default:
                        return Visibility.Visible;
                }
            }
        }

        public Visibility WarningVisibility_OperatorChanged
        {
            get
            {
                if (isModified)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public SolidColorBrush IsoMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(IsoMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush IsoCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(IsoCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush LeftCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(LeftCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush RightCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(RightCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush AntCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(AntCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush PostCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(PostCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush SupCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(SupCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush InfCropOffsetBackgroundColor
        {
            get
            {
                if (HasError(nameof(InfCropOffset)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush LeftMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(LeftMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush RightMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(RightMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush AntMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(AntMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush PostMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(PostMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush SupMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(SupMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush InfMarginBackgroundColor
        {
            get
            {
                if (HasError(nameof(InfMargin)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public string TargetTemplateStructureId
        {
            get
            {
                return _instructionModel.InstructionTargetId;
            }
            set
            {
                _instructionModel.InstructionTargetId = value;
                ValidateTargetTemplateStructureId();
            }
        }

        private void ValidateTargetTemplateStructureId()
        {
            ClearErrors(nameof(TargetTemplateStructureId));
            if (_instructionModel.InstructionHasTarget())
            {
                if (!string.Equals(DefaultTemplateStructureId, TargetTemplateStructureId, StringComparison.OrdinalIgnoreCase))
                    isModified = true;
                else
                    isModified = false;
                if (string.IsNullOrEmpty(TargetTemplateStructureId))
                {
                    int instructionNumber = _parentGeneratedStructure.GetInstructionNumber(_instructionModel);
                    AddError(nameof(TargetTemplateStructureId), $"{_parentGeneratedStructure.StructureId} operator #{instructionNumber + 1} has an invalid target of operation.");
                }
            }
            RaisePropertyChangedEvent(nameof(TargetTemplateBackgroundColor));
        }

        private void ValidateOperator()
        {
            ClearErrors(nameof(SelectedOperator));
            switch (_selectedOperator)
            {
                case OperatorTypes.undefined:
                    AddError(nameof(SelectedOperator), $"{_parentGeneratedStructure.StructureId} has an invalid operator");
                    break;
                default:
                    break;
            }
            RaisePropertyChangedEvent(nameof(OperatorBackgroundColor));
            RaisePropertyChangedEvent(nameof(VisibilityTargetSelection));
        }
        public SolidColorBrush TargetTemplateBackgroundColor
        {
            get
            {
                if (HasError(nameof(TargetTemplateStructureId)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }

        public SolidColorBrush OperatorBackgroundColor
        {
            get
            {
                if (HasError(nameof(SelectedOperator)))
                {
                    return new SolidColorBrush(Colors.DarkOrange);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            }
        }



        public ObservableCollection<string> TargetTemplateIds
        {
            get
            {
                var structureMappings = _instructionModel.GetAvailableTargetIds();
                var ids = new ObservableCollection<string>(structureMappings.Select(x => x.TemplateStructureId));
                return ids;
            }
        }

        public ObservableCollection<TemplateStructure> TemplateStructures { get; set; } = new ObservableCollection<TemplateStructure>();
        public InstructionViewModel(IInstructionModel instruction, IGeneratedStructureModel parentStructure, IEventAggregator ea)
        {
            _instructionModel = instruction;
            _parentGeneratedStructure = parentStructure;
            _ea = ea;
            RegisterEvents();
            InitializeViewModel();
            ValidateOperator();
        }

        private void InitializeViewModel()
        {
            switch (_instructionModel.Operator)
            {
                case OperatorTypes.or:
                    _selectedOperator = OperatorTypes.or;
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    break;
                case OperatorTypes.and:
                    _selectedOperator = OperatorTypes.and;
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    break;
                case OperatorTypes.sub:
                    _selectedOperator = OperatorTypes.sub;
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    break;
                case OperatorTypes.subfrom:
                    _selectedOperator = OperatorTypes.subfrom;
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    break;
                case OperatorTypes.margin:
                    _selectedOperator = OperatorTypes.margin;
                    IsoMargin = _instructionModel.IsoMargin.ToString();
                    RaisePropertyChangedEvent(nameof(SelectedMargin));
                    RaisePropertyChangedEvent(nameof(IsoMargin));
                    break;
                case OperatorTypes.asymmetricMargin:
                    _selectedOperator = OperatorTypes.asymmetricMargin;
                    RaisePropertyChangedEvent(nameof(SelectedMargin));
                    RightMargin = _instructionModel.RightMargin.ToString();
                    LeftMargin = _instructionModel.LeftMargin.ToString();
                    PostMargin = _instructionModel.PostMargin.ToString();
                    AntMargin = _instructionModel.AntMargin.ToString();
                    SupMargin = _instructionModel.SupMargin.ToString();
                    InfMargin = _instructionModel.InfMargin.ToString();
                    break;
                case OperatorTypes.convertDose:
                    _selectedOperator = OperatorTypes.convertDose;
                    DoseLevel = _instructionModel.DoseLevel.ToString();
                    break;
                case OperatorTypes.convertResolution:
                    _selectedOperator = OperatorTypes.convertResolution;
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    break;
                case OperatorTypes.crop:
                    _selectedOperator = OperatorTypes.crop;
                    IsoCropOffset = _instructionModel.IsoCropOffset.ToString();
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    RaisePropertyChangedEvent(nameof(InternalCrop));
                    break;
                case OperatorTypes.asymmetricCrop:
                    _selectedOperator = OperatorTypes.asymmetricCrop;
                    RightCropOffset = _instructionModel.RightCropOffset.ToString();
                    LeftCropOffset = _instructionModel.LeftCropOffset.ToString();
                    PostCropOffset = _instructionModel.PostCropOffset.ToString();
                    AntCropOffset = _instructionModel.AntCropOffset.ToString();
                    SupCropOffset = _instructionModel.SupCropOffset.ToString();
                    InfCropOffset = _instructionModel.InfCropOffset.ToString();
                    RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
                    RaisePropertyChangedEvent(nameof(InternalCrop));
                    break;
                case OperatorTypes.partition:
                    _selectedOperator = OperatorTypes.partition;
                    SupBound = _instructionModel.SupBound.ToString();
                    InfBound = _instructionModel.InfBound.ToString();
                    break;
                case OperatorTypes.setHU:
                    _selectedOperator = OperatorTypes.setHU;
                    HUValue = _instructionModel.HUValue.ToString();
                    break;
                default:
                    _selectedOperator = OperatorTypes.undefined;
                    break;

            }
            isModified = false;
            ValidateTargetTemplateStructureId();
        }

        public InstructionViewModel()
        {
            // Design use only
            TargetTemplateStructureId = "DesignId";
            _instructionModel = new InstructionModelTest();
        }

        public void RegisterEvents()
        {
            _ea.GetEvent<NewTemplateStructureEvent>().Subscribe(OnNewTemplateStructureEvent);
            _ea.GetEvent<RemovedInstructionEvent>().Subscribe(OnRemovedInstruction);
            _ea.GetEvent<TemplateStructureIdChangedEvent>().Subscribe(OnTemplateStructureIdChanged);
            _ea.GetEvent<RemovedTemplateStructureEvent>().Subscribe(RemoveTemplateStructureFromList);
            _ea.GetEvent<RemovedGeneratedStructureEvent>().Subscribe(RemoveGeneratedStructureFromList);
            _ea.GetEvent<GeneratedStructureIdChangedEvent>().Subscribe(OnGeneratedStructureIdChanged);
            _ea.GetEvent<GeneratedStructureOrderChangedEvent>().Subscribe(OnGeneratedStructureOrderChanged);
            ErrorsChanged += (sender, args) => { _ea.GetEvent<DataValidationRequiredEvent>().Publish(); };
        }

        private void OnRemovedInstruction(InstructionRemovedEventInfo info)
        {
            if (_instructionModel == info.RemovedInstruction)
                _ea.GetEvent<RemovingInstructionViewModelEvent>().Publish(this);
        }

        private void RemoveGeneratedStructureFromList(RemovedGeneratedStructureEventInfo info)
        {
            if (_parentGeneratedStructure.StructureId != info.RemovedStructureId)
            {
                RaisePropertyChangedEvent(nameof(TargetTemplateIds));
            }
        }

        private void OnGeneratedStructureOrderChanged()
        {
            RaisePropertyChangedEvent(nameof(TargetTemplateIds));
        }

        private void OnGeneratedStructureIdChanged(GeneratedStructureIdChangedEventInfo info)
        {
            if (TargetTemplateStructureId == info.OldId)
            {
                DefaultTemplateStructureId = info.NewId;
                TargetTemplateStructureId = info.NewId;
                RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
            }
            RaisePropertyChangedEvent(nameof(TargetTemplateIds));
            // Update error messaging
            foreach (var errorProperty in PropertiesWithErrrors())
            {
                if (HasError(errorProperty))
                {
                    var propInfo = GetType().GetProperty(errorProperty);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(this);
                        propInfo.SetValue(this, null);
                        propInfo.SetValue(this, value); // need to change due to FODY
                    }
                }
            }
            
        }

        private void OnNewTemplateStructureEvent(NewTemplateStructureEventInfo info)
        {
            RaisePropertyChangedEvent(nameof(TargetTemplateIds));
        }


        private void RemoveTemplateStructureFromList(RemovedTemplateStructureEventInfo info)
        {
            if (info.RemovedTemplateStructureId != TargetTemplateStructureId)
            {
                // Leave this property unchanged if it's the one being removed, so it can be found and cleaned up.
                RaisePropertyChangedEvent(nameof(TargetTemplateIds));
            }
        }

        private void OnTemplateStructureIdChanged(TemplateStructureIdChangedEventInfo info)
        {
            RaisePropertyChangedEvent(nameof(TargetTemplateIds));
            if (info.OldId == TargetTemplateStructureId)
            {
                DefaultTemplateStructureId = info.NewId;
                TargetTemplateStructureId = info.NewId;
                RaisePropertyChangedEvent(nameof(TargetTemplateStructureId));
            }
        }


        public ICommand RemoveInstructionCommand
        {
            get
            {
                return new DelegateCommand(RemoveInstruction);
            }
        }
        private void RemoveInstruction(object param = null)
        {
            _instructionModel.Remove();
            _ea.GetEvent<RemovingInstructionViewModelEvent>().Publish(this);
        }


    }

}
