using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace EQD2_DVH
{
    /// <summary>
    /// Tarjoaa staattisia metodeja EQD2-laskentaa varten.
    /// </summary>
    public static class DVHCalculator
    {
        /// <summary>
        /// Laskee EQD2-arvon yksittäiselle annospisteelle.
        /// </summary>
        public static double CalculateEQD2ForPoint(double totalDose, int numberOfFractions, double alphaBeta)
        {
            if (numberOfFractions <= 0 || (2.0 + alphaBeta) == 0)
            {
                return totalDose;
            }
            double dosePerFractionAtPoint = totalDose / numberOfFractions;
            return totalDose * (dosePerFractionAtPoint + alphaBeta) / (2.0 + alphaBeta);
        }

        /// <summary>
        /// Muuntaa kokonaisen DVH-käyrän EQD2-muotoon kuvaajaa varten.
        /// </summary>
        public static DVHPoint[] GetEQD2Curve(DVHPoint[] originalCurve, int numberOfFractions, double alphaBeta)
        {
            if (originalCurve == null) return new DVHPoint[0];

            return originalCurve.Select(p => new DVHPoint(
                new DoseValue(CalculateEQD2ForPoint(p.DoseValue.Dose, numberOfFractions, alphaBeta), DoseValue.DoseUnit.Gy),
                p.Volume,
                p.VolumeUnit
            )).ToArray();
        }

        /// <summary>
        /// Laskee biologisen keskiarvoannoksen (EQD2 D.mean) tarkemmin hyödyntäen differentiaali-DVH:ta.
        /// </summary>
        public static double CalculateMeanEQD2FromDVH(DVHPoint[] curveData, int numberOfFractions, double alphaBeta)
        {
            if (curveData == null || curveData.Length < 2) return 0.0;

            double totalVolume = curveData.First().Volume;
            if (totalVolume <= 0) return 0.0;

            double totalBioDose = 0;

            for (int i = 0; i < curveData.Length - 1; i++)
            {
                DVHPoint p1 = curveData[i];
                DVHPoint p2 = curveData[i + 1];
                double volumeSegment = p1.Volume - p2.Volume;

                if (volumeSegment > 0)
                {
                    double doseSegment = (p1.DoseValue.Dose + p2.DoseValue.Dose) / 2.0;
                    double eqd2Segment = CalculateEQD2ForPoint(doseSegment, numberOfFractions, alphaBeta);
                    totalBioDose += eqd2Segment * volumeSegment;
                }
            }
            return totalBioDose / totalVolume;
        }
    }
}