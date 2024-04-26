using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public interface IStructureMappingModel
    {
        string TemplateStructureId { get; }
        string EclipseStructureId { get; }
    }
    public class StructureMappingModel : IStructureMappingModel
    {
        public string TemplateStructureId { get; private set; }
        public string EclipseStructureId { get; private set; }
        public StructureMappingModel(string templateStructureId, string eclipseStructureId)
        {
            TemplateStructureId = templateStructureId;
            EclipseStructureId = eclipseStructureId;
        }

    }
}
