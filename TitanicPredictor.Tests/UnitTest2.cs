using FluentAssertions;
using TitanicPredictor.MachineLearningManual;

namespace TitanicPredictor.Tests
{
    public class UnitTest2
    {
        [Fact]
        public void UnitTest01()
        {
            var predictor = new TitanicPredictorManual();
            var trainData = GetData.GetTrainData();
            predictor.TrainModel(trainData);
            var testData = GetData.GetTestData();
            var predictions = predictor.PredictSurvival(testData);

            var answerLookup = GetData.GetAnswerLookup();
            var answerCheck = predictions.GroupBy(x => answerLookup[x.PassengerId] == x.Survived).ToDictionary(x => x.Key, x => x.ToArray());
            var (Correct, Wrong) = (
                answerCheck[true],
                answerCheck[false]
            );
            var score = (decimal)Correct.Length / testData.Count() * 100;
            Math.Round(score, 3, MidpointRounding.ToEven).Should().Be(78.947M);


            //var body = string.Join("\r\n", predictions.Select(x => $"{x.PassengerId},{(x.Survived ? "1" : "0")}"));
            //var header = "PassengerId,Survived";
            //var file = $"{header}\r\n{body}";
        }
    }
}
