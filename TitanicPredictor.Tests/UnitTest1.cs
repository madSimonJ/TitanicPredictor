using CsvHelper;
using FluentAssertions;
using TitanicPredictor.MachineLearningAuto;
using TitanicPredictor_MachineLearning;

namespace TitanicPredictor.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
             var testData = GetData.GetTestData().ToArray();

            var testinput = testData.Select(x => (
                x.PassengerId,
                Prediction: TitanicAuto01.Predict(
                    new TitanicAuto01.ModelInput
                    {
                        Age = x.Age switch { not float.NaN => (float)x.Age, _ => float.NaN },
                        Embarked = x.Embarked?.ToString() ?? "",
                        Fare = x.Fare switch { not float.NaN => (float)x.Fare, _ => float.NaN },
                        Parch = x.Parch,
                        Pclass = x.Pclass,
                        Sex = x.Sex,
                        SibSp = x.SibSp,
                        Cabin = x.Cabin,
                        Name = x.Name,
                        Ticket = x.Ticket
                    }))).ToArray();

            var answerLookup = GetData.GetAnswerLookup();
            var answerCheck = testinput.GroupBy(x => answerLookup[x.PassengerId] == x.Prediction.PredictedLabel).ToDictionary(x => x.Key, x => x.ToArray());
            var (Correct, Wrong) = (
                 answerCheck[true],
                answerCheck[false]
            );
            var score = ((decimal)Correct.Length / (decimal)testinput.Length) * 100;
            Math.Round(score, 3, MidpointRounding.ToEven).Should().Be(55.024M);
        }

        [Fact]
        public void GenerateRefinedData()
        {
            var testData = GetData.GetTrainData();
            var refinedData = DataOperations.RefineData(testData);
            GetData.WriteRefinedData(refinedData);
        }


        [Fact]
        public void GenerateAgeTrainData()
        {
            var ageTestdata = GetData.GetAgeTestData();
            var ageTestRefined = DataOperations.RefineData(ageTestdata);
            var answers = ageTestRefined.Select(x => (rawData: x, Prediction: TitanicAuto_PredictAge.Predict(new TitanicAuto_PredictAge.ModelInput
            {
                CabinLetter = x.CabinLetter,
                Embarked = x.Embarked.ToString(),
                Family = x.Family,
                Fare = (float)x.Fare,
                Pclass = x.Pclass,
                Sex = x.Sex,
                Title = x.Title
            })))
            .Select(x => (
                Prediction: x.Prediction.Score,
                Input: x.rawData
            )).ToArray();
        }

        [Fact]
        public void GenerateAgeTestData()
        {
            var testData = GetData.GetTrainData();
            var refinedData = DataOperations.RefineData(testData);
            var missingDataFilledIn = DataOperations.FillInMissingData(refinedData).ToArray();
            GetData.WriteRefinedData(missingDataFilledIn);

        }

        [Fact]
        public void GenerateMissingCabinLetterTrainData()
        {
            var testData = GetData.GetTrainData();
            var refinedData = DataOperations.RefineData(testData);
            var missingDataFilledIn = DataOperations.FillInMissingData(refinedData).ToArray();
            var trainData = missingDataFilledIn.Where(x => !string.IsNullOrWhiteSpace(x.CabinLetter) && x.CabinLetter != "\0").ToArray();
            GetData.WriteRefinedData(trainData);
        }


        [Fact]
        public void Test2()
        {
            var testData = GetData.GetTestData().ToArray();
            var refinedData = DataOperations.RefineData(testData);


            var testinput = refinedData.Select(x => (
                x.PassengerId,
                Prediction: TitanicAuto02RefinedData.Predict(
                    new TitanicAuto02RefinedData.ModelInput
                    {
                        Age = x.Age switch { not float.NaN => (float)x.Age, _ => float.NaN },
                        CabinLetter = x.CabinLetter,
                        Embarked = x.Embarked.ToString(),
                        Family = x.Family,
                        Fare = x.Fare switch { not float.NaN => (float)x.Fare, _ => float.NaN },
                        Pclass = x.Pclass,
                        Sex = x.Sex,
                        Title = x.Title
                        
                    }))).ToArray();

            var answerLookup = GetData.GetAnswerLookup();
            var answerCheck = testinput.GroupBy(x => answerLookup[x.PassengerId] == x.Prediction.PredictedLabel).ToDictionary(x => x.Key, x => x.ToArray());
            var (Correct, Wrong) = (
                answerCheck[true],
                answerCheck[false]
            );
            var score = (decimal)Correct.Length / testinput.Length * 100;
            Math.Round(score, 3, MidpointRounding.ToEven).Should().Be(77.751M);
        }

        [Fact]
        public void Test3()
        {
            var testData = GetData.GetTestData().ToArray();
            var refinedData = DataOperations.RefineData(testData);

            var missingDataFilledIn = DataOperations.FillInMissingData(refinedData).ToArray();


            var testinput = missingDataFilledIn.Select(x => (
                x.PassengerId,
                Prediction: TitanicAuto02RefinedData.Predict(
                    new TitanicAuto02RefinedData.ModelInput
                    {
                        Age = x.Age switch { not float.NaN => (float)x.Age, _ => float.NaN },
                        CabinLetter = x.CabinLetter,
                        Embarked = x.Embarked.ToString(),
                        Family = x.Family,
                        Fare = x.Fare switch { not float.NaN => (float)x.Fare, _ => float.NaN },
                        Pclass = x.Pclass,
                        Sex = x.Sex,
                        Title = x.Title

                    }))).ToArray();

            var answerLookup = GetData.GetAnswerLookup();
            var answerCheck = testinput.GroupBy(x => answerLookup[x.PassengerId] == x.Prediction.PredictedLabel).ToDictionary(x => x.Key, x => x.ToArray());
            var (Correct, Wrong) = (
                answerCheck[true],
                answerCheck[false]
            );
            var score = (decimal)Correct.Length / testinput.Length * 100;
            Math.Round(score, 3, MidpointRounding.ToEven).Should().Be(77.512M);
        }


        [Fact]
        public void Test4()
        {
            var testData = GetData.GetTestData().ToArray();
            var refinedData = DataOperations.RefineData(testData);

            var missingDataFilledIn = DataOperations.FillInMissingData(refinedData).ToArray();


            var testinput = missingDataFilledIn.Select(x => (
                x.PassengerId,
                Prediction: TitanicAuto_NoCabinNumber.Predict(
                    new TitanicAuto_NoCabinNumber.ModelInput
                    {
                        Age = x.Age switch { not float.NaN => (float)x.Age, _ => float.NaN },
                        Embarked = x.Embarked.ToString(),
                        Family = x.Family,
                        Fare = x.Fare switch { not float.NaN => (float)x.Fare, _ => float.NaN },
                        Pclass = x.Pclass,
                        Sex = x.Sex,
                        Title = x.Title

                    }))).ToArray();

            var answerLookup = GetData.GetAnswerLookup();
            var answerCheck = testinput.GroupBy(x => 
                answerLookup[x.PassengerId] == x.Prediction.PredictedLabel).ToDictionary(x => x.Key, x => x.ToArray());
            var (Correct, Wrong) = (
                answerCheck[true],
                answerCheck[false]
            );
            var score = (decimal)Correct.Length / testinput.Length * 100;
            Math.Round(score, 3, MidpointRounding.ToEven).Should().Be(77.99M);

            var body = string.Join("\r\n", testinput.Select(x => $"{x.PassengerId},{(x.Prediction.PredictedLabel ? "0" : "1")}"));
            var header = "PassengerId,Survived";
            var file = $"{header}\r\n{body}";
        }
    }
}