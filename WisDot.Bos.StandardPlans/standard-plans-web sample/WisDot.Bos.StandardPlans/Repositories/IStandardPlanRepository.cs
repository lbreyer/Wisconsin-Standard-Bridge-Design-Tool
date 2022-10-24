using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.StandardPlans.Models;

namespace WisDot.Bos.StandardPlans.Repositories
{
    internal interface IStandardPlanRepository
    {
        StandardPlan GetStandardPlan(int id);
        StandardPlan GetStandardPlan(float spanLength, float substructureSkew, float clearRoadwayWidth, string barrierTypeCode, bool pavingNotch,
            float abutmentHeight, string pilingTypeCode);
        Task<StandardPlan> GetStandardPlanAsync(float spanLength, float substructureSkew, float clearRoadwayWidth, string barrierTypeCode, bool pavingNotch,
            float abutmentHeight, string pilingTypeCode);
        List<BarrierType> GetBarrierTypes();
        BarrierType GetBarrierType(string barrierTypeCode);
        List<PilingType> GetPilingTypes();
        PilingType GetPilingType(string pilingTypeCode);
        List<double> GetSpanLengths();
        List<string> GetSubstructureSkews();
        List<double> GetClearRoadwayWidths();
        List<double> GetAbutmentHeights();
        FiipsProject GetFiipsProject(string fiipsConstructionId);
        string GetTermsOfAgreement();
        string GetSampleStandardSlabPlan();
        void WriteDownloadHistory(string fileName, bool fileFound, DateTime fileDate, DateTime downloadDate, string comments, string wamsId, string fiipsConstructionId, string fiipsDesignId, string fiipsStructureId);
    }
}
