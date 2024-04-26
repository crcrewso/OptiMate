using OptiMate;
using OptiMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OptiMate.ViewModels
{
    public class EclipseStructureViewModel : ObservableObject
    {
        public string EclipseId { get; set; }
        public SolidColorBrush EclipseIdColor { get; private set; }
        private bool isEmpty;

        public EclipseStructureViewModel(string eclipseId, bool isEmpty)
        {
            this.isEmpty = isEmpty;
            EclipseId = eclipseId;
            UpdateEclipseStructureColors();
        }

        public EclipseStructureViewModel() { }

        private void UpdateEclipseStructureColors()
        {
            if (isEmpty)
                EclipseIdColor = new SolidColorBrush(Colors.DarkGray);
            else
                EclipseIdColor = new SolidColorBrush(Colors.Black);
            RaisePropertyChangedEvent(nameof(EclipseIdColor));
        }
    }
}
