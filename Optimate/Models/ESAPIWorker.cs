using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OptiMate
{
    public class EsapiWorker 
    {
        private readonly StructureSet _ss = null;
        private readonly PlanSetup _pl = null;
        private readonly Patient _p = null;
        private readonly Dispatcher _dispatcher = null;
        private readonly ActiveStructureCodeDictionaries _codeDict = null;
        public string UserId { get; private set; }
        
        public EsapiWorker(Patient p, StructureSet ss, string userId, ActiveStructureCodeDictionaries dict)
        {
            _p = p;
            UserId = userId;
            _ss = ss;
            _codeDict = dict;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public EsapiWorker(Patient p, PlanSetup pl, StructureSet ss, string userId, ActiveStructureCodeDictionaries dict)
        {
            _p = p;
            _pl = pl;
            UserId = userId;
            _ss = ss;
            _codeDict = dict;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public delegate void D(Patient p, StructureSet s);
        public async Task<bool> AsyncRunStructureContext(Action<Patient, StructureSet, Dispatcher> a)
        {
            await _dispatcher.BeginInvoke(a, _p, _ss,  _dispatcher);
            return true;
        }

        public async Task<bool> AsyncRunStructureCodeContext(Action<Patient, StructureSet, ActiveStructureCodeDictionaries, Dispatcher> a)
        {
            await _dispatcher.BeginInvoke(a, _p, _ss, _codeDict, _dispatcher);
            return true;
        }

        public async Task<bool> AsyncRunPlanContext(Action<Patient, PlanSetup, StructureSet, Dispatcher> a)
        {
            await _dispatcher.BeginInvoke(a, _p, _pl, _ss, _dispatcher);
            return true;
        }
    }
}
