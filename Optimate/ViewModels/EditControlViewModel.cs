using OptiMate.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace OptiMate.ViewModels
{
    public class EditControlViewModel : ObservableObject
    {
        private TemplateStructureModel _templateStructureModel;
        private IEventAggregator _ea;

        public bool UserConfirmed { get; private set; } = false;
        public int SelectedAliasIndex { get; set; }
        private string _templateStructureId = "DesignTemplateStructure";
        public string TemplateStructureId
        {
            get
            {
                return _templateStructureId;
            }
            set
            {
                _templateStructureId = value;
                _templateStructureModel.TemplateStructureId = value;
                if (_templateStructureId != _templateStructureModel.TemplateStructureId)
                {
                    AddError(nameof(TemplateStructureId), "Template Structure Id is invalid");
                }
                else
                {
                    ClearErrors(nameof(TemplateStructureId));
                }
                RaisePropertyChangedEvent(nameof(TemplateStructureIdBackgroundColor));
                RaisePropertyChangedEvent(nameof(InputsValid));
            }
        }

        public SolidColorBrush TemplateStructureIdBackgroundColor
        {
            get
            {
                if (HasError(nameof(TemplateStructureId)))
                {
                    return Brushes.DarkOrange;
                }
                else
                {
                    return Brushes.LightGoldenrodYellow;
                }
            }
        }

        private string _newAlias;
        public string NewAlias
        {
            get { return _newAlias; }
            set
            {
                ClearErrors(nameof(NewAlias));
                _newAlias = value;
                if (!_templateStructureModel.IsAliasValid(value))
                {
                    AddError(nameof(NewAlias), "Alias is invalid");
                }
                RaisePropertyChangedEvent(nameof(NewAliasTextBoxColor));
            }
        }

        public bool PerformSmallStructureChecks
        {

            get { return _templateStructureModel.PerformSmallVolumeCheck; }
            set
            {
                _templateStructureModel.PerformSmallVolumeCheck = value;
            }
        }
        public SolidColorBrush NewAliasTextBoxColor
        {
            get
            {
                if (HasError(nameof(NewAlias)))
                {
                    return Brushes.DarkOrange;
                }
                else
                {
                    return Brushes.LightGoldenrodYellow;
                }
            }
        }



        public ObservableCollection<string> Aliases { get; private set; } = new ObservableCollection<string>() { "DesignAlias1", "DesignAlias2", "DesignAlias3", "DesignAlias4" };

        public EditControlViewModel() { }
        public EditControlViewModel(string templateStructureId, TemplateStructureModel templateStructureModel, IEventAggregator ea)
        {
            _templateStructureId = templateStructureId;
            _templateStructureModel = templateStructureModel;
            Aliases.Clear();
            Aliases.AddRange(templateStructureModel.Aliases);
            _ea = ea;
        }


        public ICommand AddNewAliasCommand
        {
            get { return new DelegateCommand(AddNewAlias); }
        }

        private void AddNewAlias(object obj)
        {
            if (!_templateStructureModel.IsAliasValid(_newAlias))
            {
                return;
            }
            else
            {
                _templateStructureModel.AddNewTemplateStructureAlias(NewAlias);
                Aliases.Add(NewAlias);
            }
        }

        public ICommand RemoveAliasCommand
        {
            get { return new DelegateCommand(RemoveAlias); }
        }

        private void RemoveAlias(object obj)
        {
            string alias = obj as string;
            _templateStructureModel.RemoveTemplateStructureAlias(alias);
            Aliases.Remove(alias);
        }



        internal void ReorderAliases(int a, int b)
        {
            _templateStructureModel.ReorderTemplateStructureAliases(a, b);
            Aliases.Move(a, b);
        }


        public ICommand ConfirmCommand
        {
            get { return new DelegateCommand(Confirm); }
        }
        private void Confirm(object obj)
        {
            UserConfirmed = true;
            (obj as Popup).IsOpen = false;
        }

        public bool InputsValid
        {
            get
            {
                if (HasErrors)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
