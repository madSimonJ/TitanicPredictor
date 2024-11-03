using Microsoft.ML;
using TitanicPredictor.Entities;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Transforms;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;

namespace TitanicPredictor.MachineLearningManual
{
    public class TitanicPredictorManual : ITitanicPredictor
    {
        private class UnclassifiedTitanicPassengerMl
        {
            [ColumnName("PassengerId"), LoadColumn(0)]
            public float PassengerId { get; set; }


            [ColumnName("Pclass"), LoadColumn(1)]
            public float Pclass { get; set; }

            [ColumnName("Name"), LoadColumn(2)]
            public string Name { get; set; }

            [ColumnName("Sex"), LoadColumn(3)]
            public string Sex { get; set; }

            [ColumnName("Age"), LoadColumn(4)]
            public float Age { get; set; }

            [ColumnName("SibSp"), LoadColumn(5)]
            public float SibSp { get; set; }

            [ColumnName("Parch"), LoadColumn(6)]
            public float Parch { get; set; }

            [ColumnName("Ticket"), LoadColumn(7)]
            public string Ticket { get; set; }

            [ColumnName("Fare"), LoadColumn(8)]
            public float Fare { get; set; }

            [ColumnName("Cabin"), LoadColumn(9)]
            public string Cabin { get; set; }

            [ColumnName("Embarked"), LoadColumn(10)]
            public string Embarked { get; set; }
        }

        private class TitanicPassengerMl
        {

            [ColumnName("PassengerId"), LoadColumn(0)]
            public float PassengerId { get; set; }

            [ColumnName("Survived"), LoadColumn(1)]
            public bool Survived { get; set; }

            [ColumnName("Pclass"), LoadColumn(2)]
            public float Pclass { get; set; }

            [ColumnName("Name"), LoadColumn(3)]
            public string Name { get; set; }

            [ColumnName("Sex"), LoadColumn(4)]
            public string Sex { get; set; }

            [ColumnName("Age"), LoadColumn(5)]
            public float Age { get; set; }

            [ColumnName("SibSp"), LoadColumn(6)]
            public float SibSp { get; set; }

            [ColumnName("Parch"), LoadColumn(7)]
            public float Parch { get; set; }

            [ColumnName("Ticket"), LoadColumn(8)]
            public string Ticket { get; set; }

            [ColumnName("Fare"), LoadColumn(9)]
            public float Fare { get; set; }

            [ColumnName("Cabin"), LoadColumn(10)]
            public string Cabin { get; set; }

            [ColumnName("Embarked"), LoadColumn(11)]
            public string Embarked { get; set; }
        }

        private class TitanicPassengerPredictionMl
        {
            [ColumnName("PassengerId")]
            public float PassengerId { get; set; }

            [ColumnName("Pclass")]
            public float Pclass { get; set; }

            [ColumnName("Name")]
            public string Name { get; set; }

            [ColumnName("Age")]
            public float Age { get; set; }

            [ColumnName("SibSp")]
            public float SibSp { get; set; }

            [ColumnName("Parch")]
            public float Parch { get; set; }

            [ColumnName("Ticket")]
            public string Ticket { get; set; }

            [ColumnName("Fare")]
            public float Fare { get; set; }

            [ColumnName("Cabin")]
            public string Cabin { get; set; }

            public bool PredictedLabel { get; set; }
            public float Score { get; set; }
        }

        private class CustomTransformationNameInputRow
        {
            public string Name { get; set; }
        }

        private class CustomTransformationNameOutputRow
        {
            public string Title { get; set; }
        }

        private class CustomTransformationFamilyInputRow
        {
            public float SibSp { get; set; }
            public float Parch { get; set; }
        }

        private class CustomTransformationCategoriesOutputRow
        {
            public float Age { get; set; }
            public float Pclass { get; set; }
            public float Fare { get; set; }
            public float Family { get; set; }

        }

        private class CustomTransformationCategoriesInputRow
        {
            public float FarePerPerson { get; set; }
            public float AgeClass { get; set; }
        }

        private class CustomTransformationFamilyOutputRow
        {
            public float Family { get; set; }
            public float TravelledAlone { get; set; }
        }

        private class UnclassifiedTitanicPassengerCsvRow
        {
            public int PassengerId { get; set; }
            public int Pclass { get; set; }
            public string Name { get; set; }
            public string Sex { get; set; }
            public decimal? Age { get; set; }
            public int SibSp { get; set; }
            public int Parch { get; set; }
            public string Ticket { get; set; }
            public decimal? Fare { get; set; }
            public string Cabin { get; set; }
            public char? Embarked { get; set; }
        }

        private class TitanicPassengerCsvRow : UnclassifiedTitanicPassengerCsvRow
        {

            public bool Survived { get; set; }
        }

        private readonly MLContext ctx = new();
        private ITransformer Model;

        public TitanicPredictorManual()
        {
            using var reader = new StreamReader("./train.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<TitanicPassengerCsvRow>();
            var trainingData = records.Select(x => new TitanicPassenger
            {
                Age = x.Age switch { _ when x.Age is not null && x.Age > 0 => (float)x.Age, _ => float.NaN },
                Cabin = x.Cabin,
                Embarked = x.Embarked,
                Fare = x.Fare switch { _ when x.Fare is not null && x.Fare > 0 => (float)x.Fare, _ => float.NaN },
                Name = x.Name,
                Parch = x.Parch,
                PassengerId = x.PassengerId,
                Pclass = x.Pclass,
                Sex = x.Sex,
                SibSp = x.SibSp,
                Ticket = x.Ticket,
                Survived = x.Survived
            }).ToArray();

            this.TrainModel(trainingData);
        }

        public void TrainModel(IEnumerable<TitanicPassenger> passengers)
        {
            var td = passengers.Select(x => new TitanicPassengerMl
            {
                Age = x.Age,
                Cabin = x.Cabin,
                Embarked = x.Embarked?.ToString() ?? "",
                Fare = x.Fare,
                Name = x.Name,
                Parch = x.Parch,
                PassengerId = x.PassengerId,
                Pclass = x.Pclass,
                Sex = x.Sex,
                SibSp = x.SibSp,
                Survived = x.Survived,
                Ticket = x.Ticket
            });

            var trainingData = ctx.Data.LoadFromEnumerable(td);
            var pipeline = ctx
                .Transforms.CustomMapping((CustomTransformationNameInputRow x, CustomTransformationNameOutputRow y) =>
                {
                    y.Title = x.Name.Split(",")[1].Trim().Split(" ")[0].Replace(".", "") switch
                    {
                        "Col" => "Military",
                        "Dr" => "Dr",
                        "Major" => "Military",
                        "Rev" => "Rev",
                        "Mlle" => "Miss",
                        "Miss" => "Miss",
                        "Ms" => "Miss",
                        "Mme" => "Mrs",
                        "Mrs" => "Mrs",
                        "Mr" => "Mr",
                        "Master" => "Master",
                        _ => "Rare"
                    };
                }, contractName: null)
                .Append(ctx.Transforms.CustomMapping((CustomTransformationFamilyInputRow x, CustomTransformationFamilyOutputRow y) =>
                {
                    y.Family = x.SibSp + x.Parch;
                    y.TravelledAlone = x.SibSp == 0 && x.Parch == 0 ? 1 : 0;
                }, contractName: null))
                .Append(ctx.Transforms.Categorical.OneHotEncoding(
                [
                    new InputOutputColumnPair("Sex"),
                    new InputOutputColumnPair("Embarked"),
                    new InputOutputColumnPair("Title")
                ]))
                .Append(ctx.Transforms.NormalizeMinMax([
                    new InputOutputColumnPair("Fare"),
                    new InputOutputColumnPair("Age")
                    ]))
                .Append(ctx.Transforms.ReplaceMissingValues([new InputOutputColumnPair("Embarked")], MissingValueReplacingEstimator.ReplacementMode.Mean))
                .Append(ctx.Transforms.Concatenate("Features", ["Age", "Pclass", "Family", "Embarked", "Sex", "Fare", "Title"]))
                .Append(ctx.BinaryClassification.Trainers.FastForest("Survived"));
            this.Model = pipeline.Fit(trainingData);
        }

        public bool PredictSurvival(NewTitanicPassenger passenger)
        {
            var td  = new[] { new UnclassifiedTitanicPassengerMl
            {
                Age = passenger.Age,
                Cabin = "",
                Embarked = passenger.Embarked,
                Fare = passenger.Fare,
                Name = $"blah, {passenger.Title} blah",
                Parch = passenger.Family,
                Pclass = passenger.Pclass,
                Sex = passenger.Sex,
                SibSp = 0,
                Ticket = ""
            }};
            var data = this.ctx.Data.LoadFromEnumerable(td);
            var prediction = this.Model.Transform(data);
            var p = prediction.Preview();
            var output = this.ctx.Data.CreateEnumerable<TitanicPassengerPredictionMl>(prediction, reuseRowObject: false).ToArray();
            var predictionData = output.Select(x => new TitanicPassenger
            {
                Age = x.Age,
                Cabin = x.Cabin,
                Fare = x.Fare,
                Name = x.Name,
                Parch = (int)x.Parch,
                PassengerId = (int)x.PassengerId,
                Pclass = (int)x.Pclass,
                SibSp = (int)x.SibSp,
                Ticket = x.Ticket,
                Survived = x.PredictedLabel
            });
            var returnValue = predictionData.First().Survived;
            return returnValue;
        }

        public IEnumerable<TitanicPassenger> PredictSurvival(IEnumerable<UnclassifiedTitanicPassenger> passengers)
        {
            var td = passengers.Select(x => new UnclassifiedTitanicPassengerMl
            {
                Age = x.Age,
                Cabin = x.Cabin,
                Embarked = x.Embarked?.ToString() ?? "",
                Fare = x.Fare,
                Name = x.Name,
                Parch = x.Parch,
                PassengerId = x.PassengerId,
                Pclass = x.Pclass,
                Sex = x.Sex,
                SibSp = x.SibSp,
                Ticket = x.Ticket
            });
            var data = this.ctx.Data.LoadFromEnumerable(td);
            var prediction = this.Model.Transform(data);
            var p = prediction.Preview();
            var output = this.ctx.Data.CreateEnumerable<TitanicPassengerPredictionMl>(prediction, reuseRowObject: false).ToArray();
            var returnValue = output.Select(x => new TitanicPassenger
            {
                Age = x.Age,
                Cabin = x.Cabin,
                Fare = x.Fare,
                Name = x.Name,
                Parch = (int)x.Parch,
                PassengerId = (int)x.PassengerId,
                Pclass = (int)x.Pclass,
                SibSp = (int)x.SibSp,
                Ticket = x.Ticket,
                Survived = x.PredictedLabel
            });
            return returnValue;

        }

    }
}
