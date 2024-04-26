using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public class AvailableTargetModel
    {
        private OptiMateTemplate _template;
        public AvailableTargetModel(OptiMateTemplate template)
        {
            _template = template;
        }
        internal List<StructureMappingModel> GetAugmentedTemplateStructures(string structureId)
        {
            var augmentedList = new List<StructureMappingModel>();
            foreach (var templateStructure in _template.TemplateStructures)
            {
                augmentedList.Add(new StructureMappingModel(templateStructure.TemplateStructureId, templateStructure.EclipseStructureId));
            }
            foreach (var genStructure in _template.GeneratedStructures.Take(_template.GeneratedStructures.Select(x => x.StructureId).ToList().IndexOf(structureId)))
            {
                augmentedList.Add(new StructureMappingModel(genStructure.StructureId, genStructure.StructureId));
            }
            return augmentedList;
        }
    }
}
