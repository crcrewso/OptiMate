using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiMate.Models
{
    public struct StructureGeneratedEventInfo
    {
        public IGeneratedStructureModel Structure;
        public int IndexInQueue;
        public int TotalToGenerate;
        public List<string> Warnings;
    }

    public struct StructureGeneratingEventInfo
    {
        public IGeneratedStructureModel Structure;
        public int IndexInQueue;
        public int TotalToGenerate;
        public List<string> Warnings;
    }
    public struct InstructionRemovedEventInfo
    {
        public IGeneratedStructureModel Structure;
        public IInstructionModel RemovedInstruction;
    }
    public struct InstructionAddedEventInfo
    {
        public IGeneratedStructureModel Structure;
        public IInstructionModel AddedInstruction;
    }
    public struct NewTemplateStructureEventInfo
    {
        public TemplateStructureModel NewTemplateStructure;
    }
    public struct RemovedTemplateStructureEventInfo
    {
        public string RemovedTemplateStructureId;
    }
    public struct RemovedGeneratedStructureEventInfo
    {
        public string RemovedStructureId;
    }
    public struct NewGeneratedStructureEventInfo
    {
        public GeneratedStructureModel NewGeneratedStructure;
    }

    public struct TemplateStructureIdChangedEventInfo
    {
        public string OldId;
        public string NewId;
    }

    public struct GeneratedStructureIdChangedEventInfo
    {
        public string OldId;
        public string NewId;
    }

    public class TemplateSavedEvent : PubSubEvent { }

    public class ReadyForGenStructureCleanupEvent : PubSubEvent { }
    
    public class TemplateStructureIdChangedEvent : PubSubEvent<TemplateStructureIdChangedEventInfo> { }
    public class GeneratedStructureIdChangedEvent : PubSubEvent<GeneratedStructureIdChangedEventInfo> { }

    public class GeneratedStructureCleaningUpEvent : PubSubEvent<string> { }
    public class GeneratedStructureOrderChangedEvent : PubSubEvent { }
    public class RemovedInstructionEvent : PubSubEvent<InstructionRemovedEventInfo> { }
    public class ModelInitializedEvent : PubSubEvent { }
    public class NewTemplateNameSpecified : PubSubEvent<string> { }
    public class AddedInstructionEvent : PubSubEvent<InstructionAddedEventInfo> { }
    public class StructureGeneratedEvent : PubSubEvent<StructureGeneratedEventInfo> { }
    public class StructureGeneratingEvent : PubSubEvent<StructureGeneratingEventInfo> { }
    public class NewTemplateStructureEvent : PubSubEvent<NewTemplateStructureEventInfo> { }
    public class RemovedTemplateStructureEvent : PubSubEvent<RemovedTemplateStructureEventInfo> { }
    public class RemovedGeneratedStructureEvent : PubSubEvent<RemovedGeneratedStructureEventInfo> { }
    public class NewGeneratedStructureEvent : PubSubEvent<NewGeneratedStructureEventInfo> { }
}
