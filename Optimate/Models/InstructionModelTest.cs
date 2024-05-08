using OptiMate.Models;
using OptiMate.ViewModels;
using System;
using System.Collections.Generic;

namespace OptiMate.Models
{
    public class InstructionModelTest : IInstructionModel
    {
        public ushort? AntMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string DefaultInstructionTargetId => throw new NotImplementedException();

        public ushort? DoseLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort? InfMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string InstructionTargetId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsAntMarginValid => throw new NotImplementedException();

        public bool IsInfMarginValid => throw new NotImplementedException();

        public bool isInstructionTargetIdValid => throw new NotImplementedException();

        public bool IsIsoMarginValid => throw new NotImplementedException();

        public bool IsLeftMarginValid => throw new NotImplementedException();

        public ushort? IsoMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsPostMarginValid => throw new NotImplementedException();

        public bool IsRightMarginValid => throw new NotImplementedException();

        public bool IsSupMarginValid => throw new NotImplementedException();

        public ushort? LeftMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MarginTypes? MarginType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public OperatorTypes Operator => throw new NotImplementedException();

        public ushort? PostMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort? RightMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort? SupMargin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort? IsoCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsIsoCropOffsetValid => throw new NotImplementedException();

        public ushort? LeftCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsLeftCropOffsetValid => throw new NotImplementedException();

        public ushort? RightCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsRightCropOffsetValid => throw new NotImplementedException();

        public ushort? SupCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsSupCropOffsetValid => throw new NotImplementedException();

        public ushort? InfCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsInfCropOffsetValid => throw new NotImplementedException();

        public ushort? AntCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsAntCropOffsetValid => throw new NotImplementedException();

        public ushort? PostCropOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsPostCropOffsetValid => throw new NotImplementedException();

        public bool? InternalCrop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsDoseLevelValid => throw new NotImplementedException();

        public Guid InstructionId => throw new NotImplementedException();

        public double? SupBound { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? InfBound { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsSupBoundValid => throw new NotImplementedException();

        public bool IsInfBoundValid => throw new NotImplementedException();

        public short? HUValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsHUValueValid => throw new NotImplementedException();

        public InstructionTargetModel InstructionTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? IsDoseLevelAbsolute { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        string IInstructionModel.DefaultInstructionTargetId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool? IInstructionModel.InternalCrop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        OperatorTypes IInstructionModel.Operator => throw new NotImplementedException();

        public List<InstructionTargetModel> GetAvailableTargets()
        {
            throw new NotImplementedException();
        }

        public bool InstructionHasTarget()
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public InstructionModel ReplaceInstruction(OperatorTypes value)
        {
            throw new NotImplementedException();
        }

      
    }

}
