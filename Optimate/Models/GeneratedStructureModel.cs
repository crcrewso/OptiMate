using OptiMate;
using OptiMate.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using OptiMate.Logging;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using System.Windows.Media;
using OptiMate.Converters;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace OptiMate.Models
{
    public interface IGeneratedStructureModel
    {
        bool ClearFirst { get; set; }
        bool isStructureIdValid { get; }
        System.Windows.Media.Color StructureColor { get; set; }
        string StructureId { get; set; }

        InstructionModel AddInstruction(OperatorTypes selectedOperator, int index);
        List<IInstructionModel> GetInstructions();
        System.Windows.Media.Color GetStructureColor();

        bool OverwriteColor { get; set; }
        CleanupOptions CleanupOption { get; set; }

        int GetInstructionNumber(IInstructionModel instruction);
        InstructionModel ReplaceInstruction(IInstructionModel instruction, OperatorTypes selectedOperator);

        bool GenStructureWillBeEmpty();

    }


    public class GeneratedStructureModel : IGeneratedStructureModel
    {
        private System.Windows.Media.Color _defaultColor = Colors.Magenta;
        private EsapiWorker _ew;
        public IEventAggregator _ea;
        private TemplateModel _templateModel;
        private GeneratedStructure _genStructure;
        private Structure generatedEclipseStructure;
        private List<string> _warnings = new List<string>();
        private string TempStructureName = @"TEMP_OptiMate";
        private Dictionary<IInstructionModel, Instruction> InstructionLookup = new Dictionary<IInstructionModel, Instruction>();
        private List<IInstructionModel> InstructionOrder = new List<IInstructionModel>();


        public GeneratedStructureModel(EsapiWorker ew, IEventAggregator ea, GeneratedStructure genStructure, TemplateModel model)
        {
            this._ew = ew;
            this._ea = ea;
            this._templateModel = model;
            this._genStructure = genStructure;
            _structureColor = GetColorFromString(_genStructure.StructureColor);
            foreach (var instruction in _genStructure.Instructions.Items)
            {
                var newModel = new InstructionModel(instruction, this, model.AvailableTargets, _ea);
                InstructionLookup.Add(newModel, instruction);
                InstructionOrder.Add(newModel);
            }
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _ea.GetEvent<ReadyForGenStructureCleanupEvent>().Subscribe(OnReadyForGenStructureCleanup);
        }

        public async Task<bool> IsGenStructureEmpty()
        {
            bool isEmpty = false;
            bool Success = await _ew.AsyncRunStructureContext((p, S, _ui) =>
            {
                generatedEclipseStructure = S.Structures.FirstOrDefault(x => string.Equals(x.Id, _genStructure.StructureId, StringComparison.OrdinalIgnoreCase));
                if (generatedEclipseStructure == null || generatedEclipseStructure.IsEmpty)
                {
                    isEmpty = true;
                }
            });
            return isEmpty;
        }

        private void OnReadyForGenStructureCleanup()
        {
            if (GenStructureWillBeEmpty())
            {
                _templateModel.RemoveGeneratedStructure(StructureId);
            }
        }

        public bool isStructureIdValid { get; private set; }

        public string StructureId
        {
            get
            {
                return _genStructure.StructureId;
            }
            set
            {
                if (_templateModel.IsNewGeneratedStructureIdValid(value))
                    isStructureIdValid = true;
                else
                    isStructureIdValid = false;
                string oldStructureId = _genStructure.StructureId;
                _genStructure.StructureId = value;
                _ea.GetEvent<GeneratedStructureIdChangedEvent>().Publish(new GeneratedStructureIdChangedEventInfo() { OldId = oldStructureId, NewId = value });
            }
        }

        public string DicomType
        {
            get
            {
                return _genStructure.DicomType;
            }
            set
            {
                if (Helpers.ValidateDicomType.isValid(value))
                    _genStructure.DicomType = value;
                else
                    SeriLogModel.AddWarning($"Attempting to create structure with invalid Dicom Type: {value}.  Using default...");
            }
        }



        internal void RemoveInstruction(IInstructionModel instructionModel)
        {
            //SeriLogModel.AddLog($"Attempting to remove Instruction {instructionModel.InstructionId} from {StructureId}");
            if (InstructionLookup.ContainsKey(instructionModel))
            {
                var instruction = InstructionLookup[instructionModel];
                InstructionLookup.Remove(instructionModel);
                InstructionOrder.Remove(instructionModel);
                var instructionItems = _genStructure.Instructions.Items.ToList();
                instructionItems.Remove(instruction);
                _genStructure.Instructions.Items = instructionItems.ToArray();
                _ea.GetEvent<RemovedInstructionEvent>().Publish(new InstructionRemovedEventInfo { Structure = this, RemovedInstruction = instructionModel });
            }
            else
            {
                SeriLogModel.AddError($"Instruction [Type: {instructionModel.Operator} Target: {instructionModel.DefaultInstructionTargetId}] not found in instruction lookup");
                throw new Exception($"Instruction [Type: {instructionModel.Operator} Target: {instructionModel.DefaultInstructionTargetId}] not found in instruction lookup");
            }
        }
        public InstructionModel ReplaceInstruction(IInstructionModel instructionModel, OperatorTypes selectedOperator)
        {
            var instruction = InstructionLookup[instructionModel];
            InstructionLookup.Remove(instructionModel);
            InstructionOrder.Remove(instructionModel);
            var instructionitems = _genStructure.Instructions.Items.ToList();
            var index = instructionitems.IndexOf(instruction);
            instructionitems.Remove(instruction);
            var newinstruction = CreateInstruction(selectedOperator);
            instructionitems.Insert(index, newinstruction);
            _genStructure.Instructions.Items = instructionitems.ToArray();
            var newInstructionModel = new InstructionModel(newinstruction, this, _templateModel.AvailableTargets, _ea);
            InstructionLookup.Add(newInstructionModel, newinstruction);
            InstructionOrder.Insert(index, newInstructionModel);
            return newInstructionModel;
        }

        private Instruction CreateInstruction(OperatorTypes selectedOperator)
        {
            switch (selectedOperator)
            {
                case OperatorTypes.and:
                    return new And();
                case OperatorTypes.or:
                    return new Or();
                case OperatorTypes.asymmetricMargin:
                    return new AsymmetricMargin() { MarginType = MarginTypes.Outer };
                case OperatorTypes.sub:
                    return new Sub();
                case OperatorTypes.crop:
                    return new Crop();
                case OperatorTypes.margin:
                    return new Margin() { MarginType = MarginTypes.Outer };
                case OperatorTypes.convertDose:
                    return new ConvertDose() { DoseLevel = 0 };
                case OperatorTypes.subfrom:
                    return new SubFrom();
                case OperatorTypes.convertResolution:
                    return new ConvertResolution();
                case OperatorTypes.asymmetricCrop:
                    return new AsymmetricCrop();
                case OperatorTypes.partition:
                    return new Partition();
                case OperatorTypes.setHU:
                    return new SetHU();
                default:
                    SeriLogModel.AddLog("Adding new default instruction (Or)...");
                    return new Or();
            }
        }

        public int GetInstructionNumber(IInstructionModel instructionModel)
        {
            var index = InstructionLookup.Keys.ToList().IndexOf(instructionModel);
            return index;
        }

        private System.Windows.Media.Color _structureColor;

        public System.Windows.Media.Color StructureColor
        {
            get
            {
                return _structureColor;
            }
            set
            {
                _structureColor = value;
                _genStructure.StructureColor = $"{value.R:000},{value.G:000},{value.B:000}";
            }
        }

        public bool ClearFirst
        {
            get
            {
                return _genStructure.ClearFirst;
            }
            set
            {
                _genStructure.ClearFirst = value;
            }
        }


        public bool OverwriteColor
        {
            get { return _genStructure.OverwriteColor; }
            set { _genStructure.OverwriteColor = value; }
        }

        public CleanupOptions CleanupOption
        {
            get
            {
                return _genStructure.Cleanup;
            }
            set
            {
                _genStructure.Cleanup = value;
            }
        }

        public async Task RemoveEclipseStructure(string structureId)
        {
            try
            {
                await _ew.AsyncRunStructureContext((p, ss, ui) =>
                {
                    var structureToRemove = ss.Structures.First(x => x.Id == structureId);
                    if (structureToRemove != null)
                    {
                        ss.RemoveStructure(structureToRemove);
                        SeriLogModel.AddLog($"Structure {structureId} was deleted based on its cleanup option...");
                    }
                });
            }
            catch (Exception ex)
            {
                SeriLogModel.AddError($"Error deleting structure {structureId}...", ex);
            }

        }



        private Structure GetTempTargetStructure(StructureSet S, string TargetId)
        {
            // returns temporary structure with the same resolution and segment volume as the target so it can be modified without changing the target
            if (string.IsNullOrEmpty(TargetId))
                return null;
            var TargetStructure = S.Structures.FirstOrDefault(x => x.Id.ToUpper() == TargetId.ToUpper());
            if (TargetStructure == null)
            {
                string warning = string.Format("Opti structure ({0}) creation operation instruction references target {1} which was not found", _genStructure.StructureId, TargetId);
                _warnings.Add(warning);
                SeriLogModel.AddLog(warning);
                return null;
            }
            else
            {
                Structure Temp = S.Structures.FirstOrDefault(x => x.Id.ToUpper() == TempStructureName.ToUpper());
                if (Temp != null)
                    S.RemoveStructure(Temp);
                Temp = S.AddStructure("CONTROL", TempStructureName);
                if (TargetStructure.IsHighResolution)
                {
                    Temp.ConvertToHighResolution();
                }
                Temp.SegmentVolume = TargetStructure.SegmentVolume;
                if (generatedEclipseStructure.IsHighResolution && !Temp.IsHighResolution)
                {
                    // if output structure is indicated to be in high resolution and temp structure has not already been converted, convert temp to high resolution
                    Temp.ConvertToHighResolution();
                }
                return Temp;
            }
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(Margin marginInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Pending;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                double IsotropicMargin = 0;
                if (string.IsNullOrEmpty(marginInstruction.IsotropicMargin))
                {
                    marginInstruction.IsotropicMargin = "0";
                }
                if (double.TryParse(marginInstruction.IsotropicMargin, out IsotropicMargin))
                {
                    if (IsotropicMargin > 0 && IsotropicMargin < 50)
                    {
                        if (marginInstruction.MarginType == MarginTypes.Outer)
                            generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Margin(IsotropicMargin);
                        else
                            generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Margin(-IsotropicMargin);
                        SeriLogModel.AddLog($"Applied isotropic margin of {IsotropicMargin}mm to {_genStructure.StructureId}");
                        completionStatus = InstructionCompletionStatus.Completed;
                    }
                    else
                    {
                        completionStatus = InstructionCompletionStatus.Failed;
                        SeriLogModel.AddWarning($"Isotropic margin for {_genStructure.StructureId} is invalid (value = {IsotropicMargin}), skipping structure...");
                    }
                }
                else
                {
                    completionStatus = InstructionCompletionStatus.Failed;
                    SeriLogModel.AddWarning($"Isotropic margin for {_genStructure.StructureId} is invalid (value = {IsotropicMargin}), skipping structure...");
                }
            }));
            return completionStatus;
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(AsymmetricMargin marginInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                double leftmargin = 0;
                double rightmargin = 0;
                double antmargin = 0;
                double postmargin = 0;
                double infmargin = 0;
                double supmargin = 0;
                if (!double.TryParse(marginInstruction.RightMargin, out rightmargin))
                {
                    string warning = "Right margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                }
                else
                {
                    SeriLogModel.AddLog($"Right margin for {_genStructure.StructureId} set to {rightmargin}");
                }
                if (!double.TryParse(marginInstruction.LeftMargin, out leftmargin))
                {
                    string warning = "Left margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                else
                {
                    SeriLogModel.AddLog($"Left margin for {_genStructure.StructureId} set to {leftmargin}");
                }
                if (!double.TryParse(marginInstruction.AntMargin, out antmargin))
                {
                    string warning = "Anterior margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                else
                {
                    SeriLogModel.AddLog($"Anterior margin for {_genStructure.StructureId} set to {antmargin}");
                }
                if (!double.TryParse(marginInstruction.PostMargin, out postmargin))
                {
                    string warning = "Posterior margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                else
                {
                    SeriLogModel.AddLog($"Posterior margin for {_genStructure.StructureId} set to {postmargin}");
                }
                if (!double.TryParse(marginInstruction.InfMargin, out infmargin))
                {
                    string warning = "Inferior margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                else
                {
                    SeriLogModel.AddLog($"Inferior margin for {_genStructure.StructureId} set to {infmargin}");
                }
                if (!double.TryParse(marginInstruction.SupMargin, out supmargin))
                {
                    string warning = "Superior margin for " + _genStructure.StructureId + " is invalid, using default (0) margin...";
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                else
                {
                    SeriLogModel.AddLog($"Superior margin for {_genStructure.StructureId} set to {supmargin}");
                }

                var AAM = Helpers.OrientationInvariantMargins.getAxisAlignedMargins(S.Image.ImagingOrientation, marginInstruction.MarginType, rightmargin, antmargin, infmargin, leftmargin, postmargin, supmargin);
                generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.AsymmetricMargin(AAM);
            }));
            return completionStatus;
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(AsymmetricCrop cropInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Pending;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, cropInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of ASYMMETRIC CROP operation [{cropInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null/empty, skipping instruction...";
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    return;
                }
                else if (EclipseTarget.IsEmpty)
                {
                    var warning = $"Target of ASYMMETRIC CROP operation [{cropInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, skipping instruction...";
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    return;
                }
                CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                //Determine crop offset parameters
                double leftOffset = 0;
                double rightOffset = 0;
                double antOffset = 0;
                double postOffset = 0;
                double infOffset = 0;
                double supOffset = 0;
                var logMsg = $"Applying asymmetric crop for {_genStructure.StructureId} from {TargetStructure.EclipseStructureId}";
                // Anisotropic crop margins
                logMsg = $"Using anisotropic crop margins for {_genStructure.StructureId}...";
                SeriLogModel.AddLog(logMsg);
                if (!double.TryParse(cropInstruction.RightOffset, out rightOffset))
                {
                    var warning = $"Right crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (!double.TryParse(cropInstruction.LeftOffset, out leftOffset))
                {
                    var warning = $"Left crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (!double.TryParse(cropInstruction.AntOffset, out antOffset))
                {
                    var warning = $"Anterior crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (!double.TryParse(cropInstruction.PostOffset, out postOffset))
                {
                    var warning = $"Posterior crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (!double.TryParse(cropInstruction.InfOffset, out infOffset))
                {
                    var warning = $"Inferior crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (!double.TryParse(cropInstruction.SupOffset, out supOffset))
                {
                    var warning = $"Superior crop margin for {_genStructure.StructureId} is invalid, using default (0) offset...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    SeriLogModel.AddWarning(warning);
                    _warnings.Add(warning);
                }
                if (cropInstruction.InternalCrop)
                {
                    MarginTypes cropType = MarginTypes.Inner;
                    var AAM = Helpers.OrientationInvariantMargins.getAxisAlignedMargins(S.Image.ImagingOrientation, cropType, rightOffset, antOffset, infOffset, leftOffset, postOffset, supOffset);
                    SeriLogModel.AddLog($"Using internal crop for {_genStructure.StructureId}");
                    generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.And(EclipseTarget.SegmentVolume.AsymmetricMargin(AAM));
                }
                else
                {
                    MarginTypes cropType = MarginTypes.Outer;
                    var AAM = Helpers.OrientationInvariantMargins.getAxisAlignedMargins(S.Image.ImagingOrientation, cropType, rightOffset, antOffset, infOffset, leftOffset, postOffset, supOffset);
                    SeriLogModel.AddLog($"Using external crop for {_genStructure.StructureId}");
                    generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Sub(EclipseTarget.SegmentVolume.AsymmetricMargin(AAM));
                }

            }));
            return completionStatus;
        }
        private async Task<InstructionCompletionStatus> ApplyInstruction(Crop cropInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, cropInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of CROP operation [{cropInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null/empty, skipping instruction...";
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    return;
                }
                else
                {
                    if (EclipseTarget.IsEmpty)
                    {
                        var warning = $"Target of CROP operation [{cropInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, skipping instruction...";
                        _warnings.Add(warning);
                        completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                        return;
                    }
                    //Determine crop offset parameters
                    CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                    double isotropicOffset = 0;
                    var logMsg = $"Applying crop for {_genStructure.StructureId} from {TargetStructure.EclipseStructureId}";
                    SeriLogModel.AddLog(logMsg);
                    if (string.IsNullOrEmpty(cropInstruction.IsotropicOffset))
                    {
                        cropInstruction.IsotropicOffset = "0";
                        var msg = $"Isotropic crop margin for {_genStructure.StructureId} is not-specified, using zero offset...";
                        SeriLogModel.AddLog(msg);
                    }
                    if (double.TryParse(cropInstruction.IsotropicOffset, out isotropicOffset))
                    {
                        logMsg = $"Using isotropic crop margins for {_genStructure.StructureId}...";
                        SeriLogModel.AddLog(logMsg);
                        if (isotropicOffset > -50 && isotropicOffset < 50)
                        {
                            if (cropInstruction.InternalCrop)
                            {
                                SeriLogModel.AddLog($"Applying internal isotropic crop for {_genStructure.StructureId}...");
                                generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.And(EclipseTarget.SegmentVolume.Margin(-isotropicOffset));
                            }
                            else
                            {
                                SeriLogModel.AddLog($"Applying external isotropic crop for {_genStructure.StructureId}...");
                                generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Sub(EclipseTarget.SegmentVolume.Margin(isotropicOffset));
                            }
                        }
                        else
                        {
                            string errorMessage = $"Isotropic crop margin for {_genStructure.StructureId} was outside the valid Eclipse range, aborting structure...";
                            _warnings.Add(errorMessage);
                            SeriLogModel.AddError(errorMessage);
                            completionStatus = InstructionCompletionStatus.Failed;
                        }
                    }
                    else
                    {
                        string errorMessage = $"Isotropic crop margin for {_genStructure.StructureId} was specified but invalid, aborting structure...";
                        _warnings.Add(errorMessage);
                        SeriLogModel.AddError(errorMessage);
                        completionStatus = InstructionCompletionStatus.Failed;
                    }
                    logMsg = $"Applied crop to {_genStructure.StructureId}...";
                    SeriLogModel.AddLog(logMsg);
                    S.RemoveStructure(EclipseTarget);
                }
            }));
            return completionStatus;
        }


        private async Task<InstructionCompletionStatus> ApplyInstruction(ConvertDose convertDoseInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunPlanContext((p, pl, S, ui) =>
            {
                if (pl == null)
                {
                    completionStatus = InstructionCompletionStatus.Failed;
                    var warning = $"Convert dose instruction failed as no plan was found";
                    _warnings.Add(warning);
                    SeriLogModel.AddWarning(warning);
                }
                else if (pl.Dose == null)
                {
                    completionStatus = InstructionCompletionStatus.Failed;
                    var warning = $"Convert dose instruction failed as no dose distribution was found";
                    _warnings.Add(warning);
                    SeriLogModel.AddWarning(warning);
                }
                else
                {
                    try
                    {
                        if (convertDoseInstruction.isDoseLevelAbsolute)
                        {
                            pl.DoseValuePresentation = VMS.TPS.Common.Model.Types.DoseValuePresentation.Absolute;
                            generatedEclipseStructure.ConvertDoseLevelToStructure(pl.Dose, new VMS.TPS.Common.Model.Types.DoseValue(convertDoseInstruction.DoseLevel, VMS.TPS.Common.Model.Types.DoseValue.DoseUnit.cGy));
                            SeriLogModel.AddLog($"{_genStructure.StructureId} has been converted to the {convertDoseInstruction.DoseLevel} cGy isodose level...");
                        }
                        else
                        {
                            pl.DoseValuePresentation = VMS.TPS.Common.Model.Types.DoseValuePresentation.Relative;
                            generatedEclipseStructure.ConvertDoseLevelToStructure(pl.Dose, new VMS.TPS.Common.Model.Types.DoseValue(convertDoseInstruction.DoseLevel, VMS.TPS.Common.Model.Types.DoseValue.DoseUnit.Percent));
                            SeriLogModel.AddLog($"{_genStructure.StructureId} has been converted to the {convertDoseInstruction.DoseLevel} % isodose level...");
                        }
                    }
                    catch (Exception e)
                    {
                        completionStatus = InstructionCompletionStatus.Failed;
                        var warning = $"Convert dose instruction failed with exception: {e.Message}";
                        _warnings.Add(warning);
                        SeriLogModel.AddWarning(warning);
                    }
                }
            }));
            return completionStatus;
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(Partition partitionInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                double infBound;
                double supBound;
                if (!double.TryParse(partitionInstruction.InferiorBound, out infBound))
                {
                    SeriLogModel.AddLog($"Partition instruction for {_genStructure.StructureId} has invalid inferior bound, using no limit...");
                    infBound = double.NegativeInfinity;
                }
                if (!double.TryParse(partitionInstruction.SuperiorBound, out supBound))
                {

                    SeriLogModel.AddLog($"Partition instruction for {_genStructure.StructureId} has invalid superior bound, using no limit...");
                    supBound = double.PositiveInfinity;
                }
                try
                {
                    var dir = S.Image.ZDirection.z;
                    int maxSlice = S.Image.ZSize - 1;
                    (int startSlice, int endSlice) = GetSlice(infBound, supBound, dir, S);
                    for (int sliceNum = 0; sliceNum <= startSlice; sliceNum++)
                    {
                        generatedEclipseStructure.ClearAllContoursOnImagePlane(sliceNum);
                    }
                    for (int sliceNum = endSlice; sliceNum <= maxSlice; sliceNum++)
                    {
                        generatedEclipseStructure.ClearAllContoursOnImagePlane(sliceNum);
                    }
                    SeriLogModel.AddLog($"Partition instruction for {_genStructure.StructureId} completed!");
                }
                catch (Exception ex)
                {
                    SeriLogModel.AddError($"Error during partition instruction of {_genStructure.StructureId}", ex);
                }
            }));
            return completionStatus;
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(SetHU setHUinstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                try
                {
                    SeriLogModel.AddLog($"Setting HU for {_genStructure.StructureId}...");
                    if (generatedEclipseStructure.CanSetAssignedHU(out string error))
                    {
                        generatedEclipseStructure.SetAssignedHU(setHUinstruction.HU);
                        SeriLogModel.AddLog($"HU for {_genStructure.StructureId} set to {setHUinstruction.HU}");
                    }
                    else
                    {
                        string warning = $"Could not assign HU for {_genStructure.StructureId} due to: {error}";
                        completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                        SeriLogModel.AddLog(warning);
                        _warnings.Add(warning);
                    }

                }
                catch (Exception ex)
                {
                    SeriLogModel.AddError($"Error during partition instruction of {_genStructure.StructureId}", ex);
                    completionStatus = InstructionCompletionStatus.Failed;
                }
            }));
            return completionStatus;
        }

        private (int, int) GetSlice(double zInf, double zSup, double dir, StructureSet S)
        {
            // z is in mm
            double epsilon = 1E-2;
            var imageRes = S.Image.ZRes;
            int startSlice;
            int endSlice;
            if (dir > 0)
            {
                startSlice = (int)Math.Floor((zInf * 10 - epsilon - (S.Image.Origin.z - S.Image.UserOrigin.z) ) / imageRes);
                endSlice = (int)Math.Ceiling((zSup * 10 + epsilon - (S.Image.Origin.z - S.Image.UserOrigin.z) ) / imageRes);
            }
            else
            {
                startSlice = (int)Math.Floor((zInf * 10 - epsilon - (S.Image.Origin.z - S.Image.UserOrigin.z) ) / imageRes);
                endSlice = (int)Math.Ceiling((zSup * 10 + epsilon - (S.Image.Origin.z - S.Image.UserOrigin.z) ) / imageRes);
            }
            startSlice = Math.Max(0, startSlice);
            endSlice = Math.Min(S.Image.ZSize - 1, endSlice);
            return (startSlice, endSlice);
        }
        private void CheckForHRConversion(string eclipseStructureId, Structure eclipseTarget)
        {
            if (eclipseTarget.IsHighResolution && !generatedEclipseStructure.IsHighResolution)
            {
                // log if this structure wasn't designated as needing to be high resolution
                var warning = $"Generated structure {_genStructure.StructureId} was not high-resolution, but must be converted because one of its inputs ({eclipseStructureId}) is.";
                _warnings.Add(warning);
                generatedEclipseStructure.ConvertToHighResolution();
            }
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(And andInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, andInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of AND operation [{andInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null, clearing generated structure...";
                    _warnings.Add(warning);
                    SeriLogModel.AddWarning(warning);
                    generatedEclipseStructure = ClearStructure(S, generatedEclipseStructure);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                }
                else if (EclipseTarget.IsEmpty)
                {
                    var warning = $"Target of AND operation [{andInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, clearing generated structure...";
                    _warnings.Add(warning);
                    SeriLogModel.AddWarning(warning);
                    generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.And(EclipseTarget.SegmentVolume);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                }
                else
                {
                    SeriLogModel.AddWarning($"Performing AND between {andInstruction.TargetStructureId} and {_genStructure.StructureId}...");
                    CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                    generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.And(EclipseTarget.SegmentVolume);
                }
                S.RemoveStructure(EclipseTarget);
            }));
            return completionStatus;
        }
        private async Task<InstructionCompletionStatus> ApplyInstruction(Sub subInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, subInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of SUB operation [{subInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null/empty, skipping instruction...";
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    return;
                }
                else
                {
                    if (EclipseTarget.IsEmpty)
                    {
                        var warning = $"Target of SUB operation [{subInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, skipping instruction...";
                        _warnings.Add(warning);
                        completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                        return;
                    }
                    else
                    {
                        SeriLogModel.AddWarning($"Performing SUB of {subInstruction.TargetStructureId} from {_genStructure.StructureId}...");
                        CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                        generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Sub(EclipseTarget.SegmentVolume);
                    }
                    S.RemoveStructure(EclipseTarget);
                }
            }));
            return completionStatus;
        }
        private async Task<InstructionCompletionStatus> ApplyInstruction(SubFrom subFromInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, subFromInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of SUBFROM operation [{subFromInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null/empty, aborting structure creation...";
                    _warnings.Add(warning);
                    completionStatus = InstructionCompletionStatus.Failed;
                    return;
                }
                else
                {
                    if (EclipseTarget.IsEmpty)
                    {
                        var warning = $"Target of SUBFROM operation [{subFromInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, aborting structure creation...";
                        completionStatus = InstructionCompletionStatus.Failed;
                        _warnings.Add(warning);
                    }
                    else
                    {
                        SeriLogModel.AddWarning($"Performing SUBFROM of {_genStructure.StructureId} from {subFromInstruction.TargetStructureId}...");
                        CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                        generatedEclipseStructure.SegmentVolume = EclipseTarget.SegmentVolume.Sub(generatedEclipseStructure.SegmentVolume);
                    }
                    S.RemoveStructure(EclipseTarget);
                }
            }));
            return completionStatus;
        }

        private Structure ClearStructure(StructureSet ss, Structure generatedEclipseStructure)
        {
            var structureId = generatedEclipseStructure.Id;
            var dicomType = generatedEclipseStructure.DicomType;
            var color = generatedEclipseStructure.Color;
            if (generatedEclipseStructure.DicomType == "")
            {
                SeriLogModel.AddWarning($"Clearing structure {generatedEclipseStructure.Id} by segment as dicom type is null...");
                generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Sub(generatedEclipseStructure.SegmentVolume.Margin(5));
                return generatedEclipseStructure; // workaround as structures with type "" cannot be removed
            }
            else
            {
                ss.RemoveStructure(generatedEclipseStructure);
                var newStructure = ss.AddStructure(dicomType, structureId);
                newStructure.Color = color;
                return newStructure;
            }
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(Or orInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                InstructionTargetModel TargetStructure = _templateModel.AvailableTargets.GetInstructionTargets(_genStructure.StructureId).FirstOrDefault(x => string.Equals(x.TargetStructureId, orInstruction.TargetStructureId, StringComparison.OrdinalIgnoreCase));
                Structure EclipseTarget = GetTempTargetStructure(S, TargetStructure.EclipseStructureId);
                if (EclipseTarget == null)
                {
                    var warning = $"Target of OR operation [{orInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is null/empty, skipping instruction...";
                    completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                    _warnings.Add(warning);
                    return;
                }
                else
                {
                    if (EclipseTarget.IsEmpty)
                    {
                        var warning = $"Target of OR operation [{orInstruction.TargetStructureId}] for structure {_genStructure.StructureId} is empty, skipping instruction...";
                        completionStatus = InstructionCompletionStatus.CompletedWithWarning;
                        _warnings.Add(warning);
                        return;
                    }
                    else
                    {
                        SeriLogModel.AddLog($"Performing OR between {orInstruction.TargetStructureId} and {_genStructure.StructureId}...");
                        CheckForHRConversion(TargetStructure.EclipseStructureId, EclipseTarget);
                        generatedEclipseStructure.SegmentVolume = generatedEclipseStructure.SegmentVolume.Or(EclipseTarget.SegmentVolume);
                    }
                    S.RemoveStructure(EclipseTarget);
                }
            }));
            return completionStatus;
        }

        private async Task<InstructionCompletionStatus> ApplyInstruction(ConvertResolution resolutionInstruction)
        {
            InstructionCompletionStatus completionStatus = InstructionCompletionStatus.Completed;
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                if (generatedEclipseStructure != null && !generatedEclipseStructure.IsHighResolution)
                {
                    SeriLogModel.AddWarning($"Converting {_genStructure.StructureId} to high resolution...");
                    generatedEclipseStructure.ConvertToHighResolution();
                }
            }));
            return completionStatus;
        }
        internal async Task GenerateStructure()
        {
            try
            {
                _warnings.Clear();
                bool isNewStructure = false;
                bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
                {
                    generatedEclipseStructure = S.Structures.FirstOrDefault(x => string.Equals(x.Id, _genStructure.StructureId, StringComparison.OrdinalIgnoreCase));
                    if (generatedEclipseStructure == null)
                    {
                        DICOMTypes DT = 0;
                        isNewStructure = true;
                        Enum.TryParse(_genStructure.DicomType.ToUpper(), out DT);
                        bool validNewStructure = S.CanAddStructure(DT.ToString(), _genStructure.StructureId);
                        if (validNewStructure)
                        {
                            generatedEclipseStructure = S.AddStructure(DT.ToString(), _genStructure.StructureId);
                        }
                        else
                        {
                            _warnings.Add($"Unable to create structure {_genStructure.StructureId}...");
                            throw new Exception($"Unable to create structure {_genStructure.StructureId}...");
                        }
                    }
                    else
                    {
                        isNewStructure = false;
                        if (_genStructure.ClearFirst)
                        {
                            SeriLogModel.AddWarning($"Structure {_genStructure.StructureId} already exists and clear-first is true, clearing...");
                            generatedEclipseStructure = ClearStructure(S, generatedEclipseStructure);
                        }
                        else
                        {
                            SeriLogModel.AddWarning($"Structure {_genStructure.StructureId} already exists and clear-first is false, not clearing...");
                        }
                    }
                    SetEclipseStructureColor(isNewStructure);
                    SetStructureCode();

                }));
                InstructionCompletionStatus status = InstructionCompletionStatus.Pending;
                foreach (var inst in _genStructure.Instructions.Items)
                {
                    switch (inst)
                    {
                        case ConvertResolution resolutionInstruction:
                            if (resolutionInstruction != null)
                                status = await ApplyInstruction(resolutionInstruction);
                            break;
                        case Margin marginInstruction:
                            if (marginInstruction != null)
                                status = await ApplyInstruction(marginInstruction);
                            break;
                        case AsymmetricMargin asymmMarginInstruction:
                            if (asymmMarginInstruction != null)
                                status = await ApplyInstruction(asymmMarginInstruction);
                            break;
                        case AsymmetricCrop asymmCropInstruction:
                            if (asymmCropInstruction != null)
                                status = await ApplyInstruction(asymmCropInstruction);
                            break;
                        case Crop cropInstruction:
                            if (cropInstruction != null)
                                status = await ApplyInstruction(cropInstruction);
                            break;
                        case And andInstruction:
                            if (andInstruction != null)
                                status = await ApplyInstruction(andInstruction);
                            break;
                        case Or orInstruction:
                            if (orInstruction != null)
                                status = await ApplyInstruction(orInstruction);
                            break;
                        case Sub subInstruction:
                            if (subInstruction != null)
                                status = await ApplyInstruction(subInstruction);
                            break;
                        case SubFrom subFromInstruction:
                            if (subFromInstruction != null)
                                status = await ApplyInstruction(subFromInstruction);
                            break;
                        case ConvertDose convertDoseInstruction:
                            if (convertDoseInstruction != null)
                                status = await ApplyInstruction(convertDoseInstruction);
                            break;
                        case Partition partitionInstruction:
                            if (partitionInstruction != null)
                                status = await ApplyInstruction(partitionInstruction);
                            break;
                        case SetHU setHUInstruction:
                            if (setHUInstruction != null)
                                status = await ApplyInstruction(setHUInstruction);
                            break;
                        default:
                            throw new Exception($"Unknown instruction type {inst.GetType().Name}...");
                    }
                    if (status == InstructionCompletionStatus.Failed)
                    {
                        break;
                    }
                }
                if (status == InstructionCompletionStatus.Failed)
                {
                    Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
                    {
                        SeriLogUI.AddLog($"Clearing and aborting structure {_genStructure.StructureId} due to failed instruction...");
                        generatedEclipseStructure = ClearStructure(S, generatedEclipseStructure);
                    }));

                }
            }
            catch (Exception ex)
            {
                SeriLogModel.AddError($"Exce[topm reached generating structure {_genStructure.StructureId}", ex);
                throw new Exception($"Exception reached in GenerateStructure, please contact your OptiMate admnistrator.");
            }
        }

        private async void SetStructureCode()
        {
            await Task.Run(() => _ew.AsyncRunStructureCodeContext((p, S, dict, ui) =>
            {
                if (_genStructure.StructureCode != null)
                {
                    // search all active dictionaries for the value by string
                    bool codeSet = false;
                    StructureCode structureCode = null;
                    structureCode = dict.VmsStructCode.Values.FirstOrDefault(x => x.DisplayName.ContainsCaseInsensitive(_genStructure.StructureCode));
                    if (structureCode != null)
                    {
                        generatedEclipseStructure.StructureCode = structureCode;
                        codeSet = true;
                        SeriLogModel.AddLog($"{_genStructure.StructureId} has been assigned the structure code {structureCode.DisplayName}");
                    }
                    else 
                    {
                        structureCode = dict.Fma.Values.FirstOrDefault(x => x.DisplayName.ContainsCaseInsensitive(_genStructure.StructureCode));
                        if (structureCode != null)
                        {
                            generatedEclipseStructure.StructureCode = structureCode;
                            codeSet = true;
                            SeriLogModel.AddLog($"{_genStructure.StructureId} has been assigned the structure code {structureCode.DisplayName}");
                        }
                    }
                    if (!codeSet)
                    {
                        var warning = $"Structure code {_genStructure.StructureCode} for {_genStructure.StructureId} was not found in the code dictionary";
                        _warnings.Add(warning);
                        SeriLogModel.AddWarning(warning);
                    }
                }
            }));
        }

        public System.Windows.Media.Color GetStructureColor()
        {
            return GetColorFromString(_genStructure.StructureColor);
        }
        private async void SetEclipseStructureColor(bool isNewStructure)
        {
            await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                try
                {
                    if (isNewStructure || OverwriteColor)
                    {
                        generatedEclipseStructure.Color = StructureColor;
                        SeriLogModel.AddLog($"Setting color for structure {generatedEclipseStructure.Id} to {StructureColor}...");
                    }
                    else
                    {
                        SeriLogModel.AddLog($"Not setting color for structure {generatedEclipseStructure.Id} as it already exists and overwrite-color is false...");
                    }

                }
                catch (Exception ex)
                {
                    SeriLogModel.AddError($"Exception reached in SetStructureColor for {generatedEclipseStructure.Id}", ex);
                    throw new Exception($"Exception reached in SetStructureColor for {generatedEclipseStructure.Id}, please contact your OptiMate admnistrator.");
                }
            }));

        }

        private System.Windows.Media.Color GetColorFromString(string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return _defaultColor;
            }
            var colArray = colorString.Split(',');
            if (colArray.Length == 3)
            {
                byte r = Convert.ToByte(colArray[0]);
                byte g = Convert.ToByte(colArray[1]);
                byte b = Convert.ToByte(colArray[2]);
                var structureColor = System.Windows.Media.Color.FromRgb(r, g, b);
                SeriLogModel.AddLog($"Setting color for structure {_genStructure.StructureId} to R:{r},G:{g},B:{b}...");
                return structureColor;
            }
            else
            {
                SeriLogModel.AddLog($"Input color {colorString} is invalid, using default color...");
                return _defaultColor;
            }
        }
        private async Task ConvertToHighResolution()
        {
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                if (!generatedEclipseStructure.IsHighResolution)
                {
                    generatedEclipseStructure.ConvertToHighResolution();
                }
            }));
        }

        internal IEnumerable<string> GetCompletionWarnings()
        {
            return _warnings;
        }

        public List<IInstructionModel> GetInstructions()
        {
            return InstructionOrder;
        }
        public InstructionModel AddInstruction(OperatorTypes selectedOperator, int index)
        {
            var newInstruction = CreateInstruction(selectedOperator);
            var instructionItems = _genStructure.Instructions.Items.ToList();
            instructionItems.Insert(index, newInstruction);
            _genStructure.Instructions.Items = instructionItems.ToArray();
            var newInstructionModel = new InstructionModel(newInstruction, this, _templateModel.AvailableTargets, _ea);
            InstructionLookup.Add(newInstructionModel, newInstruction);
            InstructionOrder.Add(newInstructionModel);
            return newInstructionModel;
        }

        public bool GenStructureWillBeEmpty()
        {
            if (_genStructure.Instructions.Items.Count() == 0)
                return true;
            else
            {
                if (_genStructure.Instructions.Items.Any(x => (x is Or) || (x is ConvertDose)))
                    return false;
                else
                    return true;
            }
        }



    }

}
