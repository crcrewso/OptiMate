using OptiMate.Logging;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public class TemplateModel
    {
        private EsapiWorker _ew;
        private OptiMateTemplate _template;
        private Dictionary<string, bool> _eclipseIds;
        private IEventAggregator _ea;

        public TemplateModel(OptiMateTemplate template, EsapiWorker ew, IEventAggregator ea, Dictionary<string, bool> eclipseIds)
        {
            _ew = ew;
            _template = template;
            _ea = ea;
            _eclipseIds = eclipseIds;
        }

        internal AvailableTargetModel AvailableTargets
        {
            get { return new AvailableTargetModel(_template); }
        }
        internal bool IsEmpty(string eclipseStructureId)
        {
            if (_eclipseIds.ContainsKey(eclipseStructureId))
            {
                return _eclipseIds[eclipseStructureId];
            }
            else
            {
                return true;
            }
        }
        public string TemplateDisplayName
        {
            get { return _template.TemplateDisplayName; }
            set { _template.TemplateDisplayName = value; }
        }
        public List<string> GetGeneratedStructureIds()
        {
            return new List<string>(_template.GeneratedStructures.Select(x => x.StructureId));
        }
        internal GeneratedStructureModel GetGeneratedStructureModel(string genStructureId)
        {
            var genStructure = _template.GeneratedStructures.FirstOrDefault(x => string.Equals(x.StructureId, genStructureId, StringComparison.OrdinalIgnoreCase));
            return new GeneratedStructureModel(_ew, _ea, genStructure, this);
        }

        private string getNewGenStructureId()
        {
            int count = 1;
            string baseId = "NewGS";
            while (!IsNewGeneratedStructureIdValid(baseId + count))
            {
                count++;
            }
            return baseId + count;
        }


        internal bool IsNewTemplateStructureIdValid(string value)
        {
            if (value.Length <= 16
                && value.Length > 0
                && !_template.GeneratedStructures.Select(x => x.StructureId).Contains(value, StringComparer.OrdinalIgnoreCase)
                && !_template.TemplateStructures.Select(x => x.TemplateStructureId).Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal bool IsNewGeneratedStructureIdValid(string value)
        {
            if (value.Length <= 16
                && value.Length > 0
                && !_template.GeneratedStructures.Select(x => x.StructureId).Contains(value, StringComparer.OrdinalIgnoreCase)
                && !_template.TemplateStructures.Select(x => x.TemplateStructureId).Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal GeneratedStructureModel AddGeneratedStructure()
        {
            var newGeneratedStructure = new GeneratedStructure()
            {
                StructureId = getNewGenStructureId(),
                Instructions = new GeneratedStructureInstructions() { Items = new Instruction[] { new Or() } }
            };
            var genStructures = _template.GeneratedStructures.ToList();
            genStructures.Add(newGeneratedStructure);
            _template.GeneratedStructures = genStructures.ToArray();
            var newGeneratedStructureModel = new GeneratedStructureModel(_ew, _ea, newGeneratedStructure, this);
            _ea.GetEvent<NewGeneratedStructureEvent>().Publish(new NewGeneratedStructureEventInfo { NewGeneratedStructure = newGeneratedStructureModel });
            return newGeneratedStructureModel;
        }

        internal void RemoveGeneratedStructure(string structureId)
        {
            var genStructures = _template.GeneratedStructures.ToList();
            var toRemove = genStructures.FirstOrDefault(x => x.StructureId == structureId);
            if (toRemove != null)
            {
                genStructures.Remove(toRemove);
                _template.GeneratedStructures = genStructures.ToArray();
                _ea.GetEvent<RemovedGeneratedStructureEvent>().Publish(new RemovedGeneratedStructureEventInfo { RemovedStructureId = structureId });
            }
        }
        private string getNewTemplateStructureId()
        {
            int count = 1;
            string baseId = "NewTS";
            while (!IsNewTemplateStructureIdValid(baseId + count))
            {
                count++;
            }
            return baseId + count;
        }
        internal TemplateStructureModel AddTemplateStructure()
        {
            var newTemplateStructure = new TemplateStructure()
            {
                TemplateStructureId = getNewTemplateStructureId(),
                Alias = new string[] { }
            };
            var templateList = _template.TemplateStructures.ToList();
            templateList.Add(newTemplateStructure);
            _template.TemplateStructures = templateList.ToArray();
            var newTemplateStructureModel = new TemplateStructureModel(newTemplateStructure, _eclipseIds, _ew);
            _ea.GetEvent<NewTemplateStructureEvent>().Publish(new NewTemplateStructureEventInfo { NewTemplateStructure = newTemplateStructureModel });
            return newTemplateStructureModel;
        }

        internal void RemoveTemplateStructure(string templateStructureId)
        {
            var templateStructures = _template.TemplateStructures.ToList();
            var removedStructure = templateStructures.FirstOrDefault(x => x.TemplateStructureId == templateStructureId);
            templateStructures.Remove(removedStructure);
            _template.TemplateStructures = templateStructures.ToArray();
            _ea.GetEvent<RemovedTemplateStructureEvent>().Publish(new RemovedTemplateStructureEventInfo { RemovedTemplateStructureId = templateStructureId });
            _ea.GetEvent<ReadyForGenStructureCleanupEvent>().Publish();
        }

     

        internal List<string> GetTemplateStructureAliases(string templateStructureId)
        {
            var ts = _template.TemplateStructures.FirstOrDefault(x => string.Equals(x.TemplateStructureId, templateStructureId, StringComparison.OrdinalIgnoreCase));
            if (ts != null)
            {
                return ts.Alias == null ? new List<string>() : ts.Alias.ToList();
            }
            else
            {
                return null;
            }
        }

        internal void RenameTemplateStructure(string templateStructureId, string value)
        {
            var ts = _template.TemplateStructures.FirstOrDefault(x => string.Equals(x.TemplateStructureId, templateStructureId, StringComparison.OrdinalIgnoreCase));
            if (ts != null && IsNewTemplateStructureIdValid(value))
            {
                var eventArgs = new TemplateStructureIdChangedEventInfo() { OldId = ts.TemplateStructureId, NewId = value };
                string oldId = ts.TemplateStructureId;
                ts.TemplateStructureId = value;
                _ea.GetEvent<TemplateStructureIdChangedEvent>().Publish(eventArgs);
            }
        }

        public List<string> GetEclipseStructureIds()
        {
            return new List<string>(_eclipseIds.Keys);
        }


        public List<string> GetAvailableTemplateTargetIds(string thisGenStructureId = "")
        {
            var availableStructures = _template.TemplateStructures.Select(x => x.TemplateStructureId).ToList();
            availableStructures.AddRange(_template.GeneratedStructures.Take(_template.GeneratedStructures.Select(x => x.StructureId).ToList().IndexOf(thisGenStructureId)).Select(x => x.StructureId));
            return availableStructures;
        }

        internal async Task<List<string>> GenerateStructures()
        {
            int index = 0;
            List<string> completionWarnings = new List<string>();
            foreach (var genStructure in _template.GeneratedStructures)
            {
                var structureModel = GetGeneratedStructureModel(genStructure.StructureId);
                _ea.GetEvent<StructureGeneratingEvent>().Publish(new StructureGeneratingEventInfo { Structure = structureModel, IndexInQueue = index, TotalToGenerate = _template.GeneratedStructures.Count() });
                await structureModel.GenerateStructure();
                completionWarnings.AddRange(structureModel.GetCompletionWarnings());
                _ea.GetEvent<StructureGeneratedEvent>().Publish(new StructureGeneratedEventInfo { Structure = structureModel, IndexInQueue = index++, TotalToGenerate = _template.GeneratedStructures.Count() });
            }
            foreach (var tempStructure in _template.GeneratedStructures.Where(x => x.IsTemporary))
            {
                var structureModel = GetGeneratedStructureModel(tempStructure.StructureId);
                _ea.GetEvent<GeneratedStructureCleaningUpEvent>().Publish(tempStructure.StructureId);
                await structureModel.RemoveEclipseStructure(tempStructure.StructureId);
            }
            return completionWarnings;
        }
        public List<string> GetTemplateStructureIds()
        {
            return new List<string>(_template.TemplateStructures.Select(x => x.TemplateStructureId));
        }


        internal TemplateStructureModel GetTemplateStructureModel(string structureId)
        {
            var tempStructure = _template.TemplateStructures.FirstOrDefault(x => string.Equals(x.TemplateStructureId, structureId, StringComparison.OrdinalIgnoreCase));
            return new TemplateStructureModel(tempStructure, _eclipseIds, _ew);
        }

        internal void ReorderTemplateStructures(int a, int b)
        {
            var TemplateStructures = new ObservableCollection<TemplateStructure>(_template.TemplateStructures);
            TemplateStructures.Move(a, b);
            _template.TemplateStructures = TemplateStructures.ToArray();
        }

        internal void ReorderGeneratedStructures(int a, int b)
        {
            var GeneratedStructures = new ObservableCollection<GeneratedStructure>(_template.GeneratedStructures);
            GeneratedStructures.Move(a, b);
            _template.GeneratedStructures = GeneratedStructures.ToArray();
            _ea.GetEvent<GeneratedStructureOrderChangedEvent>().Publish();
        }



      

       
    }
}
