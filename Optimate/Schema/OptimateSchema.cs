﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace OptiMate {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="OptiMate")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="OptiMate", IsNullable=false)]
    public partial class OptiMateTemplate {
        
        private byte versionField;
        
        private TemplateStructure[] templateStructuresField;
        
        private GeneratedStructure[] generatedStructuresField;
        
        private string templateDisplayNameField;
        
        /// <remarks/>
        public byte version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public TemplateStructure[] TemplateStructures {
            get {
                return this.templateStructuresField;
            }
            set {
                this.templateStructuresField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public GeneratedStructure[] GeneratedStructures {
            get {
                return this.generatedStructuresField;
            }
            set {
                this.generatedStructuresField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateDisplayName {
            get {
                return this.templateDisplayNameField;
            }
            set {
                this.templateDisplayNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class TemplateStructure {
        
        private string[] aliasField;
        
        private string templateStructureIdField;
        
        private string eclipseStructureIdField;
        
        public TemplateStructure() {
            this.eclipseStructureIdField = "";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Alias")]
        public string[] Alias {
            get {
                return this.aliasField;
            }
            set {
                this.aliasField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EclipseStructureId {
            get {
                return this.eclipseStructureIdField;
            }
            set {
                this.eclipseStructureIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Margin))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AsymmetricCrop))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AsymmetricMargin))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Crop))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ConvertDose))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SubFrom))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Sub))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(And))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Or))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ConvertResolution))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class Instruction {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class Margin : Instruction {
        
        private MarginTypes marginTypeField;
        
        private string isotropicMarginField;
        
        public Margin() {
            this.marginTypeField = MarginTypes.Outer;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(MarginTypes.Outer)]
        public MarginTypes MarginType {
            get {
                return this.marginTypeField;
            }
            set {
                this.marginTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsotropicMargin {
            get {
                return this.isotropicMarginField;
            }
            set {
                this.isotropicMarginField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public enum MarginTypes {
        
        /// <remarks/>
        Outer,
        
        /// <remarks/>
        Inner,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class AsymmetricCrop : Instruction {
        
        private string templateStructureIdField;
        
        private bool internalCropField;
        
        private string antOffsetField;
        
        private string postOffsetField;
        
        private string supOffsetField;
        
        private string infOffsetField;
        
        private string leftOffsetField;
        
        private string rightOffsetField;
        
        public AsymmetricCrop() {
            this.internalCropField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool InternalCrop {
            get {
                return this.internalCropField;
            }
            set {
                this.internalCropField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AntOffset {
            get {
                return this.antOffsetField;
            }
            set {
                this.antOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PostOffset {
            get {
                return this.postOffsetField;
            }
            set {
                this.postOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SupOffset {
            get {
                return this.supOffsetField;
            }
            set {
                this.supOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string InfOffset {
            get {
                return this.infOffsetField;
            }
            set {
                this.infOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LeftOffset {
            get {
                return this.leftOffsetField;
            }
            set {
                this.leftOffsetField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RightOffset {
            get {
                return this.rightOffsetField;
            }
            set {
                this.rightOffsetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class AsymmetricMargin : Instruction {
        
        private MarginTypes marginTypeField;
        
        private string antMarginField;
        
        private string postMarginField;
        
        private string supMarginField;
        
        private string infMarginField;
        
        private string leftMarginField;
        
        private string rightMarginField;
        
        public AsymmetricMargin() {
            this.marginTypeField = MarginTypes.Outer;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(MarginTypes.Outer)]
        public MarginTypes MarginType {
            get {
                return this.marginTypeField;
            }
            set {
                this.marginTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AntMargin {
            get {
                return this.antMarginField;
            }
            set {
                this.antMarginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PostMargin {
            get {
                return this.postMarginField;
            }
            set {
                this.postMarginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SupMargin {
            get {
                return this.supMarginField;
            }
            set {
                this.supMarginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string InfMargin {
            get {
                return this.infMarginField;
            }
            set {
                this.infMarginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LeftMargin {
            get {
                return this.leftMarginField;
            }
            set {
                this.leftMarginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RightMargin {
            get {
                return this.rightMarginField;
            }
            set {
                this.rightMarginField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class Crop : Instruction {
        
        private string templateStructureIdField;
        
        private bool internalCropField;
        
        private string isotropicOffsetField;
        
        public Crop() {
            this.internalCropField = false;
            this.isotropicOffsetField = "";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool InternalCrop {
            get {
                return this.internalCropField;
            }
            set {
                this.internalCropField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string IsotropicOffset {
            get {
                return this.isotropicOffsetField;
            }
            set {
                this.isotropicOffsetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class ConvertDose : Instruction {
        
        private ushort doseLevelField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort DoseLevel {
            get {
                return this.doseLevelField;
            }
            set {
                this.doseLevelField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class SubFrom : Instruction {
        
        private string templateStructureIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class Sub : Instruction {
        
        private string templateStructureIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class And : Instruction {
        
        private string templateStructureIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class Or : Instruction {
        
        private string templateStructureIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TemplateStructureId {
            get {
                return this.templateStructureIdField;
            }
            set {
                this.templateStructureIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class ConvertResolution : Instruction {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GeneratedStructure))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public abstract partial class ObservableObject {
        
        private bool suppressNotificationField;
        
        private bool suppressNotificationFieldSpecified;
        
        /// <remarks/>
        public bool SuppressNotification {
            get {
                return this.suppressNotificationField;
            }
            set {
                this.suppressNotificationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SuppressNotificationSpecified {
            get {
                return this.suppressNotificationFieldSpecified;
            }
            set {
                this.suppressNotificationFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="OptiMate")]
    public partial class GeneratedStructure : ObservableObject {
        
        private GeneratedStructureInstructions instructionsField;
        
        private string structureIdField;
        
        private string dicomTypeField;
        
        public GeneratedStructure() {
            this.dicomTypeField = "CONTROL";
        }
        
        /// <remarks/>
        public GeneratedStructureInstructions Instructions {
            get {
                return this.instructionsField;
            }
            set {
                this.instructionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StructureId {
            get {
                return this.structureIdField;
            }
            set {
                this.structureIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("CONTROL")]
        public string DicomType {
            get {
                return this.dicomTypeField;
            }
            set {
                this.dicomTypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="OptiMate")]
    public partial class GeneratedStructureInstructions {
        
        private Instruction[] itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("And", typeof(And))]
        [System.Xml.Serialization.XmlElementAttribute("AsymmetricCrop", typeof(AsymmetricCrop))]
        [System.Xml.Serialization.XmlElementAttribute("AsymmetricMargin", typeof(AsymmetricMargin))]
        [System.Xml.Serialization.XmlElementAttribute("ConvertResolution", typeof(ConvertResolution))]
        [System.Xml.Serialization.XmlElementAttribute("Crop", typeof(Crop))]
        [System.Xml.Serialization.XmlElementAttribute("Margin", typeof(Margin))]
        [System.Xml.Serialization.XmlElementAttribute("Or", typeof(Or))]
        [System.Xml.Serialization.XmlElementAttribute("Sub", typeof(Sub))]
        [System.Xml.Serialization.XmlElementAttribute("SubFrom", typeof(SubFrom))]
        public Instruction[] Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="OptiMate")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="OptiMate", IsNullable=false)]
    public partial class OptiMateConfiguration {
        
        private OptiMateConfigurationPaths pathsField;
        
        /// <remarks/>
        public OptiMateConfigurationPaths Paths {
            get {
                return this.pathsField;
            }
            set {
                this.pathsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="OptiMate")]
    public partial class OptiMateConfigurationPaths {
        
        private string publicTemplatePathField;
        
        private string usersTemplatePathField;
        
        private string logPathField;
        
        /// <remarks/>
        public string PublicTemplatePath {
            get {
                return this.publicTemplatePathField;
            }
            set {
                this.publicTemplatePathField = value;
            }
        }
        
        /// <remarks/>
        public string UsersTemplatePath {
            get {
                return this.usersTemplatePathField;
            }
            set {
                this.usersTemplatePathField = value;
            }
        }
        
        /// <remarks/>
        public string LogPath {
            get {
                return this.logPathField;
            }
            set {
                this.logPathField = value;
            }
        }
    }
}