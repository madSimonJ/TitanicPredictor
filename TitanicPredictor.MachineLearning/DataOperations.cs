using System.Text.RegularExpressions;
using TitanicPredictor.Entities;
using TitanicPredictor_MachineLearning;

namespace TitanicPredictor.MachineLearningAuto
{
    public static class DataOperations
    {
        private static TitanicAuto_PredictAge AgePrediction = new TitanicAuto_PredictAge();

        private static Regex GetLetters = new Regex("[^a-zA-Z -]");

        public static IEnumerable<RefinedPassengerData> RefineData(IEnumerable<UnclassifiedTitanicPassenger> input) =>
            input.Select(x => new RefinedPassengerData
            {
                PassengerId = x.PassengerId,
                Pclass = x.Pclass,
                Age = x.Age,
                CabinLetter = GetLetters.Match(x.Cabin).Value.ToUpper(),
                Embarked = x.Embarked ?? default,
                Family = x.SibSp + x.Parch,
                Fare = x.Fare,
                Sex = x.Sex,
                Title = x.Name.Split(",")[1].Trim().Split(" ")[0].Replace(".", "") switch
                {
                    "Lady" => "Rare",
                    "Countess" => "Rare",
                    "Capt" => "Rare",
                    "Col" => "Rare",
                    "Don" => "Rare",
                    "Dr" => "Rare",
                    "Major" => "Rare",
                    "Rev" => "Rare",
                    "Sir" => "Rare",
                    "Jonkheer" => "Rare",
                    "Dona" => "Rare",
                    "Mlle" => "Miss",
                    "Miss" => "Miss",
                    "Ms" => "Miss",
                    "Mme" => "Mrs",
                    "Mrs" => "Mrs",
                    "Mr" => "Mr",
                    "Master" => "Master",
                    "the" => "Rare",
                    _ => throw new Exception("blkah")
                }
            });

        public static IEnumerable<RefinedPassengerData> RefineData(IEnumerable<TitanicPassenger> input) =>
            input.Select(x => new RefinedPassengerData
            {
                PassengerId = x.PassengerId,
                Pclass = x.Pclass,
                Survived = x.Survived,
                Age = x.Age,
                CabinLetter = x.Cabin.FirstOrDefault().ToString() ?? "",
                Embarked = x.Embarked ?? default,
                Family = x.SibSp + x.Parch,
                Fare = x.Fare,
                Sex = x.Sex,
                Title = x.Name.Split(",")[1].Trim().Split(" ")[0].Replace(".", "") switch
                {
                    "Lady" => "Rare",
                    "Countess" => "Rare",
                    "Capt" => "Rare",
                    "Col" => "Rare",
                    "Don" => "Rare",
                    "Dr" => "Rare",
                    "Major" => "Rare",
                    "Rev" => "Rare",
                    "Sir" => "Rare",
                    "Jonkheer" => "Rare",
                    "Dona" => "Rare",
                    "Mlle" => "Miss",
                    "Miss" => "Miss",
                    "Ms" => "Miss",
                    "Mme" => "Mrs",
                    "Mrs" => "Mrs",
                    "Mr" => "Mr",
                    "Master" => "Master",
                    "the" => "Rare",
                    _ => throw new Exception("blkah")
                }
            });

        private static float CalculateAge(RefinedPassengerData input)
        {
            return input.Age == float.NaN
                ? input.Age
                : TitanicAuto_PredictAge.Predict(new TitanicAuto_PredictAge.ModelInput
                {
                    CabinLetter = input.CabinLetter,
                    Embarked = input.Embarked.ToString(),
                    Family = input.Family,
                    Fare = (float)input.Fare,
                    Pclass = input.Pclass,
                    Sex = input.Sex,
                    Title = input.Title
                }).Score;
        }

        public static IEnumerable<RefinedPassengerData> FillInMissingData(IEnumerable<RefinedPassengerData> input)
        {
            var modeEmbarked = input.GroupBy(x => x.Embarked).OrderByDescending(x => x.Count()).First().First().Embarked;

            return input.Select(x => x with 
            { 
                Age = CalculateAge(x),
                Embarked = x.Embarked == default ? modeEmbarked : x.Embarked
            });
        }
    }
}
