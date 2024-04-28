using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public interface IInstructionTargetModel
    {
        string TargetStructureId { get; }
        string EclipseStructureId { get; }
    }
    public class InstructionTargetModel : ObservableObject, IInstructionTargetModel
    {
        private IEventAggregator _ea;
        public string TargetStructureId { get; private set; }
        public string EclipseStructureId { get; private set; }
        private bool _isTemplateStructure; 
        public InstructionTargetModel(string targetStructureId, string eclipseStructureId, bool isTemplateStructure=true, IEventAggregator ea = null)
        {
            _ea = ea;
            _isTemplateStructure = isTemplateStructure;
            TargetStructureId = targetStructureId;
            EclipseStructureId = eclipseStructureId;
            if (ea != null)
            {
                _ea.GetEvent<TemplateStructureIdChangedEvent>().Subscribe(OnTemplateStructureIdChanged);
                _ea.GetEvent<GeneratedStructureIdChangedEvent>().Subscribe(OnGeneratedStructureIdChanged);
            }
        }

        private void OnGeneratedStructureIdChanged(GeneratedStructureIdChangedEventInfo info)
        {
            if (TargetStructureId == info.OldId && !_isTemplateStructure)
            {
                TargetStructureId = info.NewId;
                EclipseStructureId = info.NewId;
            }
        }

        private void OnTemplateStructureIdChanged(TemplateStructureIdChangedEventInfo info)
        {
            if (TargetStructureId == info.OldId && _isTemplateStructure)
            {
               TargetStructureId = info.NewId;
            }

        }
    }
}
