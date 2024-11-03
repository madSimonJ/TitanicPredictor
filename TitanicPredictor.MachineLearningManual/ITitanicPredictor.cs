using TitanicPredictor.Entities;

namespace TitanicPredictor.MachineLearningManual
{
    public interface ITitanicPredictor
    {
        IEnumerable<TitanicPassenger> PredictSurvival(IEnumerable<UnclassifiedTitanicPassenger> passengers);
        public bool PredictSurvival(NewTitanicPassenger passenger);
    }
}