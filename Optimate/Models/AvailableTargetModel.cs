using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace OptiMate.Models
{
    public class AvailableTargetModel
    {
        private OptiMateTemplate _template;
        private IEventAggregator _ea;
        private ObservableCollection<InstructionTargetModel> _templateStructureTargets = new ObservableCollection<InstructionTargetModel>();
        private ObservableCollection<InstructionTargetModel> _genStructureTargets = new ObservableCollection<InstructionTargetModel>();
        public AvailableTargetModel(OptiMateTemplate template, IEventAggregator ea)
        {
            _template = template;
            _ea = ea;
            foreach (var templateStructure in _template.TemplateStructures)
            {
                _templateStructureTargets.Add(new InstructionTargetModel(templateStructure.TemplateStructureId, templateStructure.EclipseStructureId, true,_ea));
            }
            foreach (var genStructure in _template.GeneratedStructures)
            {
                _genStructureTargets.Add(new InstructionTargetModel(genStructure.StructureId, genStructure.StructureId, false,_ea));
            }
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _ea.GetEvent<GeneratedStructureOrderChangedEvent>().Subscribe(GeneratedStructureOrderChanged);
            _ea.GetEvent<TemplateStructureOrderChangedEvent>().Subscribe(TemplateStructureOrderChanged);
            _ea.GetEvent<NewGeneratedStructureEvent>().Subscribe(NewGeneratedStructure);
            _ea.GetEvent<NewTemplateStructureEvent>().Subscribe(NewTemplateStructure);
        }

        private void TemplateStructureOrderChanged((int, int) orderChange)
        {
            _templateStructureTargets.Move(orderChange.Item1, orderChange.Item2);
            _ea.GetEvent<AvailableTargetModelUpdated>().Publish();
        }

        private void GeneratedStructureOrderChanged((int, int) orderChange)
        {
            _genStructureTargets.Move(orderChange.Item1, orderChange.Item2);
            _ea.GetEvent<AvailableTargetModelUpdated>().Publish();
        }

        private void NewTemplateStructure(NewTemplateStructureEventInfo info)
        {
            _templateStructureTargets.Add(new InstructionTargetModel(info.NewTemplateStructure.TemplateStructureId, info.NewTemplateStructure.EclipseStructureId, true, _ea));
            _ea.GetEvent<AvailableTargetModelUpdated>().Publish();
        }

        private void NewGeneratedStructure(NewGeneratedStructureEventInfo info)
        {
            _genStructureTargets.Add(new InstructionTargetModel(info.NewGeneratedStructure.StructureId, info.NewGeneratedStructure.StructureId, false, _ea));
            _ea.GetEvent<AvailableTargetModelUpdated>().Publish();
        }

        internal List<InstructionTargetModel> GetInstructionTargets(string structureId)
        {
            var targets = new List<InstructionTargetModel>();
            targets.AddRange(_templateStructureTargets);
            targets.AddRange(_genStructureTargets.Take(_genStructureTargets.Select(x => x.TargetStructureId).ToList().IndexOf(structureId)));
            return targets;
        }
    }
}
