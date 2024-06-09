using OptiMate;
using OptiMate.ViewModels;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using VMS.TPS.Common.Model.API;
using OptiMate.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace OptiMate.Models
{


    public partial class MainModel
    {

        private string _publicTemplatePath;
        private string _usersTemplatePath;
        public List<string> ValidationErrors { get; private set; } = new List<string>();
        public string LogPath { get; private set; }

        public string CurrentUser
        {
            get
            {
                if (_ew != null)
                    return _ew.UserId;
                else
                    return "NotInitialized";
            }
        }
        private EsapiWorker _ew = null;
        private IEventAggregator _ea = null;
        private OptiMateTemplate _template = null;

        public string StructureSetId { get; private set; }
        public MainModel(EsapiWorker ew)
        {
            _ew = ew;
        }

        internal void SetEventAggregator(IEventAggregator ea)
        {
            _ea = ea;
        }

        public async void Initialize()
        {
            InitializePaths();
            SeriLogModel.Initialize(LogPath, _ew.UserId);
            await InitializeEclipseObjects();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _ea.GetEvent<NewTemplateNameSpecified>().Subscribe(OnNewTemplateNameSpecified);
            _ea.GetEvent<ModelInitializedEvent>().Publish();
        }


        private void InitializePaths()
        {

            var AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlSerializer Ser = new XmlSerializer(typeof(OptiMateConfiguration));
            var configFile = Path.Combine(AssemblyPath, "OptiMateConfig.xml");
            try
            {
                using (StreamReader config = new StreamReader(configFile))
                {
                    var OMConfig = (OptiMateConfiguration)Ser.Deserialize(config);
                    _publicTemplatePath = OMConfig.Paths.PublicTemplatePath;
                    _usersTemplatePath = OMConfig.Paths.UsersTemplatePath;
                    LogPath = OMConfig.Paths.LogPath;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Unable to read configuration file {0}\r\n\r\nDetails: {1}", configFile, ex.InnerException);
                MessageBox.Show(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        private async Task InitializeEclipseObjects()
        {
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, ss, ui) =>
            {
                p.BeginModifications();
                StructureSetId = ss.Id;
                // one time initialization
                _EclipseStructureDict = new Dictionary<string, bool>();
                foreach (var s in ss.Structures.OrderBy(x=>x.Id))
                {
                    _EclipseStructureDict.Add(s.Id, s.IsEmpty);
                }
            }));
        }

        internal TemplateModel LoadTemplate(TemplatePointer value)
        {
            XmlSerializer Ser = new XmlSerializer(typeof(OptiMateTemplate));
            if (value != null)
            {
                try
                {
                    using (StreamReader templateData = new StreamReader(value.TemplatePath))
                    {
                        _template = (OptiMateTemplate)Ser.Deserialize(templateData);
                    }
                }
                catch (Exception ex)
                {
                    string error = $"Unable to read template file: {value.TemplatePath}";
                    SeriLogModel.AddError(error, ex);
                    ValidationErrors.Add(error);
                    return null;
                }
                try
                {
                    if (ValidateTemplate())
                    {
                        SeriLogModel.AddLog($"Template [{value.TemplateDisplayName}] validated");
                        return new TemplateModel(_template, _ew, _ea, _EclipseStructureDict);
                    }
                    else
                    {
                        SeriLogModel.AddLog($"Template [{value.TemplateDisplayName}] invalid");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    string error = $"Unable to validate template file: {value.TemplatePath}";
                    SeriLogModel.AddError(error, ex);
                    ValidationErrors.Add(error);
                    return null;
                }
            }
            else
            {
                SeriLogModel.AddLog($"Attempt to load null TemplatePointer");
                throw new Exception("Attempt to load null TemplatePointer");
            }
        }

        private bool ValidateTemplate()
        {
            bool valid = true;
            ValidationErrors.Clear();
            foreach (var genStructure in _template.GeneratedStructures)
            {
                if (!IsValidEclipseStructureId(genStructure.StructureId))
                {
                    valid = false;
                    ValidationErrors.Add($"{genStructure.StructureId} is not a valid Eclipse struct");
                }
                // Disabling this validation as it prevents users from loading a template that they may have saved incompletely.
                //foreach (var instruction in genStructure.Instructions.Items)
                //{
                //    switch (instruction)
                //    {
                //        case Or inst:
                //            if (!IsValidReferenceStructure(genStructure.StructureId, inst.TemplateStructureId))
                //            {
                //                valid = false;
                //                ValidationErrors.Add($"Generated structure {genStructure.StructureId} references a Template Structure that could not be found: {inst.TemplateStructureId}");
                //            }
                //            break;
                //        case And inst:
                //            if (!IsValidReferenceStructure(genStructure.StructureId, inst.TemplateStructureId))
                //            {
                //                valid = false;
                //                ValidationErrors.Add($"Generated structure {genStructure.StructureId} references a Template Structure that could not be found: {inst.TemplateStructureId}");
                //            }
                //            break;
                //        case Sub inst:
                //            if (!IsValidReferenceStructure(genStructure.StructureId, inst.TemplateStructureId))
                //            {
                //                valid = false;
                //                ValidationErrors.Add($"Generated structure {genStructure.StructureId} references a Template Structure that could not be found: {inst.TemplateStructureId}");
                //            }
                //            break;
                //        case Crop inst:
                //            if (!IsValidReferenceStructure(genStructure.StructureId, inst.TemplateStructureId))
                //            {
                //                valid = false;
                //                ValidationErrors.Add($"Generated structure {genStructure.StructureId} references a Template Structure that could not be found: {inst.TemplateStructureId}");
                //            }
                //            break;
                //        case SubFrom inst:
                //            if (!IsValidReferenceStructure(genStructure.StructureId, inst.TemplateStructureId))
                //            {
                //                valid = false;
                //                ValidationErrors.Add($"Generated structure {genStructure.StructureId} references a Template Structure that could not be found: {inst.TemplateStructureId}");
                //            }
                //            break;
                //    }
                //}
            }
            if (!AreTemplateStructuresUnique())
            {
                valid = false;
                ValidationErrors.Add("Template structure names are not unique.");
            }
            return valid;
        }

        private bool AreTemplateStructuresUnique()
        {
            return _template.TemplateStructures.Select(x => x.TemplateStructureId).Distinct().Count() == _template.TemplateStructures.Count();
        }

        private bool IsValidEclipseStructureId(string eclipseStructureId)
        {
            return (eclipseStructureId.Count() > 0 && eclipseStructureId.Count() <= 16);
        }

        //private bool IsValidReferenceStructure(string genStructureId, string templateStructureId)
        //{
        //    var augmentedList = GetAugmentedTemplateStructures(genStructureId);
        //    if (augmentedList.Any(x => string.Equals(x.TemplateStructureId, templateStructureId, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        private Dictionary<string, bool> _EclipseStructureDict = null;



        internal IEnumerable<TemplatePointer> GetTemplates()
        {
            var TemplatePointers = new List<TemplatePointer>();
            try
            {
                XmlSerializer Ser = new XmlSerializer(typeof(OptiMateTemplate));

                foreach (var file in Directory.GetFiles(_publicTemplatePath, "*.xml"))
                {
                    using (StreamReader protocol = new StreamReader(file))
                    {
                        try
                        {
                            var OMProtocol = (OptiMateTemplate)Ser.Deserialize(protocol);
                            TemplatePointers.Add(new TemplatePointer() { TemplateDisplayName = OMProtocol.TemplateDisplayName, TemplatePath = file });
                        }
                        catch (Exception ex)
                        {
                            SeriLogModel.AddLog(string.Format("Unable to read protocol file: {0}\r\n\r\nDetails: {1}", file, ex.InnerException));
                            MessageBox.Show(string.Format("Unable to read protocol file {0}\r\n\r\nDetails: {1}", file, ex.InnerException));

                        }
                    }
                }
                foreach (var file in Directory.GetFiles(GetUserTemplatePath()))
                {
                    using (StreamReader protocol = new StreamReader(file))
                    {
                        try
                        {
                            var OMProtocol = (OptiMateTemplate)Ser.Deserialize(protocol);
                            TemplatePointers.Add(new TemplatePointer() { TemplateDisplayName = $"(p) {OMProtocol.TemplateDisplayName}", TemplatePath = file });
                        }
                        catch (Exception ex)
                        {
                            SeriLogModel.AddLog(string.Format("Unable to read protocol file: {0}\r\n\r\nDetails: {1}", file, ex.InnerException));
                            MessageBox.Show(string.Format("Unable to read protocol file {0}\r\n\r\nDetails: {1}", file, ex.InnerException));

                        }
                    }
                }
                return TemplatePointers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
                return null;
            }
        }

        internal string GetUserTemplatePath()
        {
            string userPath = Path.Combine(_usersTemplatePath, Helpers.MakeStringPathSafe(CurrentUser));
            if (!Directory.Exists(userPath))
            {
                Directory.CreateDirectory(userPath);
            }
            return Path.Combine(_usersTemplatePath, Helpers.MakeStringPathSafe(CurrentUser));
        }

        internal SaveToPersonalResult SaveTemplateToPersonal(string newTemplateId)
        {
            var dir = GetUserTemplatePath();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = dir;
            saveFileDialog.FileName = Helpers.MakeStringPathSafe(newTemplateId + ".xml");
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    writeTemplate(newTemplateId, saveFileDialog.FileName);
                    _ea.GetEvent<TemplateSavedEvent>().Publish();
                    return SaveToPersonalResult.Success;
                }
                catch (Exception ex)
                {
                    SeriLogModel.AddError("Failed to save user's template", ex);
                    return SaveToPersonalResult.Failure;
                }
            }
            else
            {
                return SaveToPersonalResult.Cancelled;
            }
        }

        private void OnNewTemplateNameSpecified(string templateName)
        {
            SaveTemplateToPersonal(templateName);
        }



        private bool writeTemplate(string newTemplateId, string fileName)
        {
            var originalTemplateName = _template.TemplateDisplayName;
            try
            {
                //Prep the template for saving, mostly filling in null strings with empty strings
                foreach (var gs in _template.GeneratedStructures)
                {
                    if (gs.Instructions != null)
                        foreach (var inst in gs.Instructions.Items)
                            switch (inst)
                            {
                                case Or or:
                                    or.TargetStructureId = or.TargetStructureId ?? "";
                                    break;
                                case And and:
                                    and.TargetStructureId = and.TargetStructureId ?? "";
                                    break;
                                case Sub sub:
                                    sub.TargetStructureId = sub.TargetStructureId ?? "";
                                    break;
                                case SubFrom subFrom:
                                    subFrom.TargetStructureId = subFrom.TargetStructureId ?? "";
                                    break;
                                case Crop crop:
                                    crop.TargetStructureId = crop.TargetStructureId ?? "";
                                    break;
                            }
                }
            }
            catch (Exception ex)
            {
                _template.TemplateDisplayName = originalTemplateName;
                SeriLogModel.AddError("Failed to save user's template during preparation for saving", ex);
                return false;
            }
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(OptiMateTemplate));
                using (TextWriter writer = new StreamWriter(fileName))
                {
                    _template.TemplateDisplayName = newTemplateId;
                    ser.Serialize(writer, _template);
                    _template.TemplateDisplayName = originalTemplateName;
                }
                return true;
            }
            catch (Exception ex)
            {
                _template.TemplateDisplayName = originalTemplateName;
                SeriLogModel.AddError("Failed to save user's template", ex);
                return false;
            }
        }

      
    }
}

