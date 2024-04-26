using OptiMate.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.Types;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace OptiMate.Models
{
    public struct MinAreaResults
    {
        public bool Success;
        public bool HasSmallAreas;
        public List<(double, VVector)> SmallAreas;
    }

    public struct SmallVolumeResults
    {
        public bool Success;
        public bool HasSmallVolumes;
        public List<VVector> SmallVolumesLocation;
    }
    public struct DistinctMeshResults
    {
        public bool Success;
        public int NumMeshes;
        public List<double> MeshVolumes;
        public List<VVector> MeshCentroids;
    }

    public interface ITemplateStructureModel
    {
        string EclipseStructureId { get; set; }
        string TemplateStructureId { get; set; }

        List<string> Aliases { get; set; }

        bool isAnAlias(string alias);
        string SetEclipseStructureByAlias();
        bool PerformSmallVolumeCheck { get; set; }
        Task<SmallVolumeResults> CheckSmallVolumes();
    }

    public class TemplateStructureModelTest : ITemplateStructureModel
    {
        public string TemplateStructureId { get; set; } = "DesignTemplateStructure";
        public string EclipseStructureId { get; set; }

        public List<string> Aliases { get; set; } = new List<string> { "DesignEclipseStructure" };
        public bool PerformSmallVolumeCheck { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool isAnAlias(string alias)
        {
            throw new NotImplementedException();
        }

        public string SetEclipseStructureByAlias()
        {
            throw new NotImplementedException();
        }

        Task<SmallVolumeResults> ITemplateStructureModel.CheckSmallVolumes()
        {
            throw new NotImplementedException();
        }
    }

    public class TemplateStructureModel : ITemplateStructureModel
    {

        private TemplateStructure _templateStructure;
        private Dictionary<string, bool> _eclipseIds;
        private EsapiWorker _ew;
        public List<string> Aliases { get; set; }
        public string TemplateStructureId
        {
            get { return _templateStructure.TemplateStructureId; }
            set { _templateStructure.TemplateStructureId = value; }
        }
        public string EclipseStructureId
        {
            get { return _templateStructure.EclipseStructureId; }
            set { _templateStructure.EclipseStructureId = value; }
        }
        public bool PerformSmallVolumeCheck
        {
            get
            {
                return _templateStructure.PerformSmallVoxelCheck;
            }
            set
            {
                _templateStructure.PerformSmallVoxelCheck = value;
            }
        }

        public TemplateStructureModel(TemplateStructure templateStructure, Dictionary<string, bool> eclipseIds, EsapiWorker ew)
        {
            _templateStructure = templateStructure;
            _ew = ew;
            _eclipseIds = eclipseIds;
            if (_templateStructure.Alias != null)
            {
                Aliases = _templateStructure.Alias.ToList();
            }
            else
            {
                Aliases = new List<string>();
            }
            SetEclipseStructureByAlias();
        }

        public string SetEclipseStructureByAlias()
        {
            foreach (var alias in Aliases)
            {
                if (_eclipseIds.Keys.Select(x => x.CompactForm()).Contains(alias.CompactForm(), StringComparer.OrdinalIgnoreCase))
                {
                    var eclipseId = _eclipseIds.Keys.FirstOrDefault(x => string.Equals(x.CompactForm(), alias.CompactForm(), StringComparison.OrdinalIgnoreCase));
                    EclipseStructureId = eclipseId;
                    SeriLogModel.AddLog($"Eclipse structure {EclipseStructureId} was found for template structure {TemplateStructureId}.");
                    return eclipseId;
                }
            }
            SeriLogModel.AddLog($"No mapping was found for template structure {TemplateStructureId}.");
            return null;
        }

        internal void RemoveTemplateStructureAlias(string alias)
        {
            var aliases = _templateStructure.Alias.ToList();
            aliases.Remove(alias);
            _templateStructure.Alias = aliases.ToArray();
        }

        public bool isAnAlias(string alias)
        {
            if (Aliases.Select(x => x.CompactForm()).Contains(alias.CompactForm(), StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal bool IsAliasValid(string value)
        {
            if (_templateStructure != null)
            {
                if (_templateStructure.Alias != null && _templateStructure.Alias.Contains(value, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }

        internal void AddNewTemplateStructureAlias(string newAlias)
        {
            if (_templateStructure.Alias == null)
                _templateStructure.Alias = new string[] { newAlias };
            else if (!_templateStructure.Alias.Contains(newAlias, StringComparer.OrdinalIgnoreCase))
                _templateStructure.Alias = _templateStructure.Alias.Concat(new string[] { newAlias }).ToArray();

        }

        internal void ReorderTemplateStructureAliases(int a, int b)
        {
            var aliases = new ObservableCollection<string>(_templateStructure.Alias);
            aliases.Move(a, b);
            _templateStructure.Alias = aliases.ToArray();
        }

        private async Task<DistinctMeshResults> GetNumDistinctMeshes()
        {
            int? NumParts = null;
            var results = new DistinctMeshResults() { Success = false };
            if (string.IsNullOrEmpty(EclipseStructureId))
            {
                return results;
            }
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                var mappedEclipseStructure = S.Structures.FirstOrDefault(x => x.Id == EclipseStructureId);
                if (mappedEclipseStructure != null)
                {
                    if (!mappedEclipseStructure.IsEmpty)
                    {
                        var result = Helpers.MeshHelper.Volumes(mappedEclipseStructure.MeshGeometry); // first of tuple is volume, second of tuple is centroid
                        result.RemoveAll(x => x.Item1 < 1E-5);
                        var partVolumes = result.Select(x => x.Item1).ToList();
                        var partVolumes_Centroids = result.Select(x => x.Item2).ToList();
                        NumParts = partVolumes.Count;
                        results = new DistinctMeshResults() { Success = true, NumMeshes = NumParts.Value, MeshVolumes = partVolumes, MeshCentroids = partVolumes_Centroids };
                    }
                }
            }));
            return results;
        }

        private async Task<int?> GetVMSMinParts()
        {
            int? NumParts = null;
            if (string.IsNullOrEmpty(EclipseStructureId))
            {
                return null;
            }
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                var mappedEclipseStructure = S.Structures.FirstOrDefault(x => x.Id == EclipseStructureId);
                if (mappedEclipseStructure != null)
                {
                    NumParts = mappedEclipseStructure.GetNumberOfSeparateParts();
                }
            }));
            return NumParts;
        }

        private async Task<MinAreaResults> GetMinArea()
        {
            MinAreaResults MinArea = new MinAreaResults() { Success = false };
            if (string.IsNullOrEmpty(EclipseStructureId))
            {
                return MinArea;
            }
            bool Done = await Task.Run(() => _ew.AsyncRunStructureContext((p, S, ui) =>
            {
                var mappedEclipseStructure = S.Structures.FirstOrDefault(x => x.Id == EclipseStructureId);
                List<(double, VVector)> smallAreas = new List<(double, VVector)>();
                if (mappedEclipseStructure != null)
                {
                    for (int z = 1; z < S.Image.ZSize - 1; z++)
                    {
                        var contours = mappedEclipseStructure.GetContoursOnImagePlane(z);
                        if (contours.Count() == 0)
                            continue;
                        VVector Centroid = new VVector();
                        foreach (VVector[] C in contours)
                        {
                            var area = GetArea(C);
                            if (Math.Abs(C.Average(x => x.z) + 2.5) < 0.01)
                            {
                                string debugme = "hi";
                            }
                            if (area < 40)
                            {
                                //Estimate normal
                                var centroid = new VVector(C.Average(x => x.x), C.Average(x => x.y), C.Average(x => x.z));
                                var normal = new VVector(C.First().x - centroid.x, C.First().y - centroid.y, C.First().z - centroid.z);
                                var Testpoint = C.First() + normal.GetUnitLengthScaledVector() * 0.5;
                                if (!mappedEclipseStructure.IsPointInsideSegment(Testpoint))
                                {
                                    Centroid.x = (C.Average(x => x.x)) / 10; // convert to cm
                                    Centroid.y = (C.Average(x => x.y)) / 10;
                                    Centroid.z = (C.Average(x => x.z)) / 10;
                                    smallAreas.Add((area, Centroid));
                                }
                            }
                        }
                    }
                }
                if (smallAreas.Count() > 0)
                {
                    MinArea = new MinAreaResults() { Success = true, HasSmallAreas = true, SmallAreas = smallAreas.OrderBy(x => x.Item1).ToList() };
                }
                else
                {
                    MinArea = new MinAreaResults() { Success = true, HasSmallAreas = false };
                }
            }));
            return MinArea;
        }

        private double GetArea(VVector[] V)
        {
            double Area = 0;
            int c = 0;
            for (c = 0; c < V.Count() - 2; c++)
            {
                Area = Area + Math.Abs(V[c].x * V[c + 1].y - V[c].y * V[c + 1].x);
            }
            Area = Area + Math.Abs(V[c + 1].x * V[0].y - V[c + 1].y * V[0].x);
            return Math.Abs(Area) / 2;

        }

        public async Task<SmallVolumeResults> CheckSmallVolumes()
        {
            var NumDetectedParts = await GetNumDistinctMeshes();
            var VarianReportedMinParts = await GetVMSMinParts();
            var minAreaResult = await GetMinArea();
            if (NumDetectedParts.Success == false)
            {
                return new SmallVolumeResults()
                {
                    Success = false,
                };
            }
            if (VarianReportedMinParts > NumDetectedParts.NumMeshes && minAreaResult.Success)
            {
                return new SmallVolumeResults()
                {
                    Success = true,
                    HasSmallVolumes = true,
                    SmallVolumesLocation = minAreaResult.SmallAreas.Select(x => x.Item2).ToList()
                };
            }
            else if (minAreaResult.HasSmallAreas)
            {
                return new SmallVolumeResults()
                {
                    Success = true,
                    HasSmallVolumes = true,
                    SmallVolumesLocation = minAreaResult.SmallAreas.Select(x => x.Item2).ToList()
                };
            }
            return new SmallVolumeResults()
            {
                Success = false,
            };
        }
    }

}
