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
namespace Optimate
{
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class OptiMateProtocol
    {

        private byte versionField;

        private OptiMateProtocolOptiStructure[] optiStructuresField;

        private string protocolDisplayNameField;

        /// <remarks/>
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("OptiStructure", IsNullable = false)]
        public OptiMateProtocolOptiStructure[] OptiStructures
        {
            get
            {
                return this.optiStructuresField;
            }
            set
            {
                this.optiStructuresField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ProtocolDisplayName
        {
            get
            {
                return this.protocolDisplayNameField;
            }
            set
            {
                this.protocolDisplayNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OptiMateProtocolOptiStructure : ObservableObject
    {

        private OptiMateProtocolOptiStructureInstruction[] instructionField;

        private string structureIdField;

        private bool isHighResolutionField;

        private bool isNewField;

        private bool isNewFieldSpecified;

        private string typeField;

        private string baseStructureField;

        public OptiMateProtocolOptiStructure()
        {
            this.isHighResolutionField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Instruction")]
        public OptiMateProtocolOptiStructureInstruction[] Instruction
        {
            get
            {
                return this.instructionField;
            }
            set
            {
                this.instructionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StructureId
        {
            get
            {
                return this.structureIdField;
            }
            set
            {
                this.structureIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool isHighResolution
        {
            get
            {
                return this.isHighResolutionField;
            }
            set
            {
                this.isHighResolutionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isNew
        {
            get
            {
                return this.isNewField;
            }
            set
            {
                this.isNewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isNewSpecified
        {
            get
            {
                return this.isNewFieldSpecified;
            }
            set
            {
                this.isNewFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BaseStructure
        {
            get
            {
                return this.baseStructureField;
            }
            set
            {
                this.baseStructureField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OptiMateProtocolOptiStructureInstruction : ObservableObject
    {

        private OperatorType operatorField;

        private bool operatorFieldSpecified;

        private string defaultTargetField;

        private string targetField;

        private string operatorParameterField;

        private string operatorParameter2Field;

        private string operatorParameter3Field;

        private string operatorParameter4Field;

        private string operatorParameter5Field;

        private string operatorParameter6Field;

        private string operatorParameter7Field;

        private bool isNewField;

        private bool isNewFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public OperatorType Operator
        {
            get
            {
                return this.operatorField;
            }
            set
            {
                this.operatorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool OperatorSpecified
        {
            get
            {
                return this.operatorFieldSpecified;
            }
            set
            {
                this.operatorFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DefaultTarget
        {
            get
            {
                return this.defaultTargetField;
            }
            set
            {
                this.defaultTargetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Target
        {
            get
            {
                return this.targetField;
            }
            set
            {
                this.targetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter
        {
            get
            {
                return this.operatorParameterField;
            }
            set
            {
                this.operatorParameterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter2
        {
            get
            {
                return this.operatorParameter2Field;
            }
            set
            {
                this.operatorParameter2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter3
        {
            get
            {
                return this.operatorParameter3Field;
            }
            set
            {
                this.operatorParameter3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter4
        {
            get
            {
                return this.operatorParameter4Field;
            }
            set
            {
                this.operatorParameter4Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter5
        {
            get
            {
                return this.operatorParameter5Field;
            }
            set
            {
                this.operatorParameter5Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter6
        {
            get
            {
                return this.operatorParameter6Field;
            }
            set
            {
                this.operatorParameter6Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OperatorParameter7
        {
            get
            {
                return this.operatorParameter7Field;
            }
            set
            {
                this.operatorParameter7Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isNew
        {
            get
            {
                return this.isNewField;
            }
            set
            {
                this.isNewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool isNewSpecified
        {
            get
            {
                return this.isNewFieldSpecified;
            }
            set
            {
                this.isNewFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    public enum OperatorType
    {

        /// <remarks/>
        UNDEFINED,

        /// <remarks/>
        copy,

        /// <remarks/>
        margin,

        /// <remarks/>
        or,

        /// <remarks/>
        and,

        /// <remarks/>
        crop,

        /// <remarks/>
        sub,

        /// <remarks/>
        subfrom,

        /// <remarks/>
        convertDose,
    }

}
