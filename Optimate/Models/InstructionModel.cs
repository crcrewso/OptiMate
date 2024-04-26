using OptiMate.Logging;
using OptiMate.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public interface IInstructionModel
    {
        ushort? AntMargin { get; set; }
        string DefaultInstructionTargetId { get; set; }
        ushort? DoseLevel { get; set; }
        bool IsDoseLevelValid { get; }
        int Index { get; set; }
        ushort? InfMargin { get; set; }
        string InstructionTargetId { get; set; }
        bool IsAntMarginValid { get; }
        bool IsInfMarginValid { get; }
        bool isInstructionTargetIdValid { get; }
        bool IsIsoMarginValid { get; }
        bool IsLeftMarginValid { get; }
        ushort? IsoMargin { get; set; }
        bool IsPostMarginValid { get; }
        bool IsRightMarginValid { get; }
        bool IsSupMarginValid { get; }
        ushort? LeftMargin { get; set; }
        MarginTypes? MarginType { get; set; }
        OperatorTypes Operator { get; }
        ushort? PostMargin { get; set; }
        ushort? RightMargin { get; set; }
        ushort? SupMargin { get; set; }
        List<StructureMappingModel> GetAvailableTargetIds();
        InstructionModel ReplaceInstruction(OperatorTypes value);
        void Remove();
        ushort? IsoCropOffset { get; set; }
        bool IsIsoCropOffsetValid { get; }
        ushort? LeftCropOffset { get; set; }
        bool IsLeftCropOffsetValid { get; }
        ushort? RightCropOffset { get; set; }
        bool IsRightCropOffsetValid { get; }
        ushort? SupCropOffset { get; set; }
        bool IsSupCropOffsetValid { get; }
        ushort? InfCropOffset { get; set; }
        bool IsInfCropOffsetValid { get; }
        ushort? AntCropOffset { get; set; }
        bool IsAntCropOffsetValid { get; }
        ushort? PostCropOffset { get; set; }
        bool IsPostCropOffsetValid { get; }

        bool? InternalCrop { get; set; }

        bool InstructionHasTarget();
        Guid InstructionId { get; }
        double? SupBound { get; set; }
        double? InfBound { get; set; }
        bool IsSupBoundValid { get; }
        bool IsInfBoundValid { get; }
        short? HUValue { get; set; }
        bool IsHUValueValid { get; }
    }




    public class InstructionModel : IInstructionModel
    {
        private IEventAggregator _ea;
        public int Index { get; set; }
        private Instruction _instruction;
        private AvailableTargetModel _targetsModel;
        private GeneratedStructureModel _generatedStructure;
        public OperatorTypes Operator { get; private set; }

        // HU value

        public short? HUValue
        {
            get
            {
                if (Operator == OperatorTypes.setHU)
                    return (_instruction as SetHU).HU;
                else
                    return null;
            }
            set
            {
                if (Operator == OperatorTypes.setHU)
                {
                    if (isHUValueValid(value))
                    {
                        (_instruction as SetHU).HU = (short)value;
                        IsHUValueValid = true;
                    }
                    else
                    {
                        IsHUValueValid = false;
                    }


                }
            }
        }
        public bool IsHUValueValid { get; private set; }

        private bool isHUValueValid(short? value)
        {
            if (value == null || value < -1050 || value > 50000)
                return false;
            else
                return true;
        }

        // Dose Level

        public ushort? DoseLevel
        {
            get
            {
                if (Operator == OperatorTypes.convertDose)
                    return (_instruction as ConvertDose).DoseLevel;
                else
                    return null;
            }
            set
            {
                if (Operator == OperatorTypes.convertDose)
                {
                    if (isDoseLevelValid(value))
                    {
                        (_instruction as ConvertDose).DoseLevel = (ushort)value;
                        IsDoseLevelValid = true;
                    }
                    else
                    {
                        IsDoseLevelValid = false;
                    }
                }
            }
        }
        public bool IsDoseLevelValid { get; private set; }
        private bool isDoseLevelValid(ushort? value)
        {
            if (value == null || value < 0 || value > 50000)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Margin
        /// </summary>

        public ushort? IsoMargin
        {
            get
            {
                if (Operator == OperatorTypes.margin)
                {
                    if (ushort.TryParse((_instruction as Margin).IsotropicMargin, out ushort isoMargin))
                    {
                        return isoMargin;
                    }
                    else return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.margin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsIsoMarginValid = true;
                        (_instruction as Margin).IsotropicMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as Margin).IsotropicMargin = "0";
                        IsIsoMarginValid = false;
                    }
                }
            }
        }
        public bool IsIsoMarginValid { get; private set; }

        private bool isMarginValueValid(ushort? value)
        {
            if (value == null || value < 0 || value > 50)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Asymmetric Margin
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="genStructure"></param>
        /// <param name="ea"></param>
        /// 
        public ushort? AntMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).AntMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsAntMarginValid = true;
                        (_instruction as AsymmetricMargin).AntMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).AntMargin = "";
                        IsAntMarginValid = false;
                    }
                }
            }
        }
        public bool IsAntMarginValid { get; private set; }

        public ushort? PostMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).PostMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsPostMarginValid = true;
                        (_instruction as AsymmetricMargin).PostMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).PostMargin = "";
                        IsPostMarginValid = false;
                    }
                }
            }
        }
        public bool IsPostMarginValid { get; private set; }

        public ushort? SupMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).SupMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsSupMarginValid = true;
                        (_instruction as AsymmetricMargin).SupMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).SupMargin = "";
                        IsSupMarginValid = false;
                    }
                }
            }
        }
        public bool IsSupMarginValid { get; private set; }

        public ushort? InfMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).InfMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsInfMarginValid = true;
                        (_instruction as AsymmetricMargin).InfMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).InfMargin = "";
                        IsInfMarginValid = false;
                    }
                }
            }
        }
        public bool IsInfMarginValid { get; private set; }
        public ushort? LeftMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).LeftMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsLeftMarginValid = true;
                        (_instruction as AsymmetricMargin).LeftMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).LeftMargin = "";
                        IsLeftMarginValid = false;
                    }
                }
            }
        }
        public bool IsLeftMarginValid { get; private set; }

        public ushort? RightMargin
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (ushort.TryParse((_instruction as AsymmetricMargin).RightMargin, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else
                {
                    return null;
                }
            }
            set
            {

                if (Operator == OperatorTypes.asymmetricMargin)
                {
                    if (isMarginValueValid(value))
                    {
                        IsRightMarginValid = true;
                        (_instruction as AsymmetricMargin).RightMargin = value.ToString();
                    }
                    else
                    {
                        (_instruction as AsymmetricMargin).RightMargin = "";
                        IsRightMarginValid = false;
                    }
                }
            }
        }
        public bool IsRightMarginValid { get; private set; }

        public MarginTypes? MarginType
        {
            get
            {
                switch (Operator)
                {
                    case OperatorTypes.margin:
                        return (_instruction as Margin).MarginType;
                    case OperatorTypes.asymmetricMargin:
                        return (_instruction as AsymmetricMargin).MarginType;
                    default:
                        return null;
                }
            }
            set
            {
                switch (Operator)
                {
                    case OperatorTypes.margin:
                        (_instruction as Margin).MarginType = (MarginTypes)value;
                        break;
                    case OperatorTypes.asymmetricMargin:
                        (_instruction as AsymmetricMargin).MarginType = (MarginTypes)value;
                        break;
                }
            }
        }

        // Crop

        public ushort? IsoCropOffset
        {
            get
            {
                if (Operator == OperatorTypes.crop)
                {
                    if (ushort.TryParse((_instruction as Crop).IsotropicOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.crop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as Crop).IsotropicOffset = value.ToString();
                        IsIsoCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as Crop).IsotropicOffset = "";
                        IsIsoCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsIsoCropOffsetValid { get; private set; }

        // AsymmetricCrop

        public ushort? LeftCropOffset
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).LeftOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).LeftOffset = value.ToString();
                        IsLeftCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).LeftOffset = "";
                        IsLeftCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsLeftCropOffsetValid { get; private set; }

        public ushort? RightCropOffset
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).RightOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).RightOffset = value.ToString();
                        IsRightCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).RightOffset = "";
                        IsRightCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsRightCropOffsetValid { get; private set; }

        public ushort? AntCropOffset
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).AntOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).AntOffset = value.ToString();
                        IsAntCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).AntOffset = "";
                        IsAntCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsAntCropOffsetValid { get; private set; }

        public ushort? PostCropOffset
        {

            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).PostOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).PostOffset = value.ToString();
                        IsPostCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).PostOffset = "";
                        IsPostCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsPostCropOffsetValid { get; private set; }

        public ushort? SupCropOffset
        {

            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).SupOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).SupOffset = value.ToString();
                        IsSupCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).SupOffset = "";
                        IsSupCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsSupCropOffsetValid { get; private set; }

        public ushort? InfCropOffset
        {
            get
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (ushort.TryParse((_instruction as AsymmetricCrop).InfOffset, out ushort value))
                    {
                        return value;
                    }
                    else
                        return 0;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.asymmetricCrop)
                {
                    if (isMarginValueValid(value))
                    {
                        (_instruction as AsymmetricCrop).InfOffset = value.ToString();
                        IsInfCropOffsetValid = true;
                    }
                    else
                    {
                        (_instruction as AsymmetricCrop).InfOffset = "";
                        IsInfCropOffsetValid = false;
                    }
                }
            }
        }
        public bool IsInfCropOffsetValid { get; private set; }

        public bool? InternalCrop
        {
            get
            {
                switch (Operator)
                {
                    case OperatorTypes.crop:
                        return (_instruction as Crop).InternalCrop;
                    case OperatorTypes.asymmetricCrop:
                        return (_instruction as AsymmetricCrop).InternalCrop;
                    default:
                        return null;
                }
            }
            set
            {
                switch (Operator)
                {
                    case OperatorTypes.crop:
                        (_instruction as Crop).InternalCrop = (bool)value;
                        break;
                    case OperatorTypes.asymmetricCrop:
                        (_instruction as AsymmetricCrop).InternalCrop = (bool)value;
                        break;
                }
            }
        }

        public double? SupBound
        {
            get
            {
                if (Operator == OperatorTypes.partition)
                {
                    if (double.TryParse((_instruction as Partition).SuperiorBound, out double value))
                    {
                        return value;
                    }
                    else
                        return null;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.partition)
                {
                    if (value != null)
                    {
                        (_instruction as Partition).SuperiorBound = value.ToString();
                        IsSupBoundValid = true;
                    }
                    else
                    {
                        (_instruction as Partition).SuperiorBound = "";
                        IsSupBoundValid = false;
                    }
                }
            }
        }
        public bool IsSupBoundValid { get; private set; }
        
        public double? InfBound
        {

            get
            {
                if (Operator == OperatorTypes.partition)
                {
                    if (double.TryParse((_instruction as Partition).InferiorBound, out double value))
                    {
                        return value;
                    }
                    else
                        return null;
                }
                else return null;
            }
            set
            {
                if (Operator == OperatorTypes.partition)
                {
                    if (value != null)
                    {
                        (_instruction as Partition).InferiorBound = value.ToString();
                        IsInfBoundValid = true;
                    }
                    else
                    {
                        (_instruction as Partition).InferiorBound = "";
                        IsInfBoundValid = false;
                    }
                }
            }
        }
        public bool IsInfBoundValid { get; private set; }


        public Guid InstructionId { get; private set; }
        public InstructionModel(Instruction instruction, GeneratedStructureModel genStructure, AvailableTargetModel targetsModel, IEventAggregator ea)
        {
            _instruction = instruction;
            InstructionId = Guid.NewGuid();
            _targetsModel = targetsModel;
            _generatedStructure = genStructure;
            _ea = ea;
            SetOperatorType();
            RegisterEvents();
            SeriLogModel.AddLog($"Created new instruction {Operator} for structure {_generatedStructure.StructureId} [Guid: {InstructionId}]");
        }

        public List<StructureMappingModel> GetAvailableTargetIds()
        {
            return _targetsModel.GetAugmentedTemplateStructures(_generatedStructure.StructureId);
        }

        private void SetOperatorType()
        {
            switch (_instruction)
            {
                case And inst:
                    Operator = OperatorTypes.and;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case Or inst:
                    Operator = OperatorTypes.or;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case AsymmetricMargin _:
                    Operator = OperatorTypes.asymmetricMargin;
                    break;
                case Sub inst:
                    Operator = OperatorTypes.sub;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case Crop inst:
                    Operator = OperatorTypes.crop;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case Margin _:
                    Operator = OperatorTypes.margin;
                    break;
                case ConvertDose _:
                    Operator = OperatorTypes.convertDose;
                    break;
                case SubFrom inst:
                    Operator = OperatorTypes.subfrom;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case ConvertResolution _:
                    Operator = OperatorTypes.convertResolution;
                    break;
                case AsymmetricCrop inst:
                    Operator = OperatorTypes.asymmetricCrop;
                    DefaultInstructionTargetId = inst.TemplateStructureId;
                    break;
                case Partition _:
                    Operator = OperatorTypes.partition;
                    break;
                case SetHU _:
                    Operator = OperatorTypes.setHU;
                    break;
                default:
                    throw new Exception("Unknown instruction type being set");
                    
            }
        }

        private void RegisterEvents()
        {
            _ea.GetEvent<RemovedTemplateStructureEvent>().Subscribe(OnRemovedTemplateStructure);
        }

        private void OnRemovedTemplateStructure(RemovedTemplateStructureEventInfo info)
        {
            if (InstructionHasTarget())
            {
                switch (_instruction)
                {
                    case And inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    case Or inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            SeriLogModel.AddLog($"Calling RemoveInstruction Or instruction targetting {info.RemovedTemplateStructureId} from {_generatedStructure.StructureId}...");
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    case Sub inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    case Crop inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    case SubFrom inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    case AsymmetricCrop inst:
                        if (inst.TemplateStructureId == info.RemovedTemplateStructureId)
                        {
                            _generatedStructure.RemoveInstruction(this);
                        }
                        break;
                    default:
                        throw new Exception("Attempting to remove target from unknown instruction type");
                }
            }
        }

        public InstructionModel ReplaceInstruction(OperatorTypes value)
        {
            var instructionModel = this;
            return _generatedStructure.ReplaceInstruction(instructionModel, value);
        }
        internal void RemoveInstructionInternal()
        {
            _generatedStructure.RemoveInstruction(this);
        }

        public void Remove()
        {
            RemoveInstructionInternal();
        }
        public bool InstructionHasTarget()
        {
            switch (Operator)
            {
                case OperatorTypes.or:
                    return true;
                case OperatorTypes.and:
                    return true;
                case OperatorTypes.sub:
                    return true;
                case OperatorTypes.subfrom:
                    return true;
                case OperatorTypes.convertDose:
                    return false;
                case OperatorTypes.crop:
                    return true;
                case OperatorTypes.margin:
                    return false;
                case OperatorTypes.convertResolution:
                    return false;
                case OperatorTypes.asymmetricMargin:
                    return false;
                case OperatorTypes.asymmetricCrop:
                    return true;
                case OperatorTypes.partition:
                    return false;
                case OperatorTypes.setHU:
                    return false;
                default:
                    throw new Exception("Attempting to check target from unknown instruction type");
            }
        }

        public bool isInstructionTargetIdValid { get; private set; }

        public string DefaultInstructionTargetId { get; set; }
        public string InstructionTargetId
        {
            get
            {
                if (InstructionHasTarget())
                {
                    switch (_instruction)
                    {
                        case And inst:
                            return inst.TemplateStructureId;
                        case Or inst:
                            return inst.TemplateStructureId;
                        case Sub inst:
                            return inst.TemplateStructureId;
                        case Crop inst:
                            return inst.TemplateStructureId;
                        case SubFrom inst:
                            return inst.TemplateStructureId;
                        case AsymmetricCrop inst:
                            return inst.TemplateStructureId;
                        default:
                            return "";
                    }
                }
                else return string.Empty;
            }
            set
            {
                if (InstructionHasTarget())
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        isInstructionTargetIdValid = false;
                    }
                    else
                    {
                        isInstructionTargetIdValid = true;
                    }
                    switch (_instruction)
                    {
                        case And inst:
                            inst.TemplateStructureId = value;
                            break;
                        case Or inst:
                            inst.TemplateStructureId = value;
                            break;
                        case Sub inst:
                            inst.TemplateStructureId = value;
                            break;
                        case Crop inst:
                            inst.TemplateStructureId = value;
                            break;
                        case SubFrom inst:
                            inst.TemplateStructureId = value;
                            break;
                        case AsymmetricCrop inst:
                            inst.TemplateStructureId = value;
                            break;
                        default:
                            throw new Exception("Attempting to get target from unknown instruction type");
                    }
                }
            }
        }

        
    }
}


