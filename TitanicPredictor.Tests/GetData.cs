using System.Globalization;
using CsvHelper;
using TitanicPredictor.Entities;

namespace TitanicPredictor.Tests
{
    public static class GetData
    {
        public class UnclassifiedTitanicPassengerCsvRow
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

        public static IEnumerable<UnclassifiedTitanicPassenger> GetTestData()
        {
            using var reader = new StreamReader("./test.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<UnclassifiedTitanicPassengerCsvRow>();
            var returnValue = records.Select(x => new UnclassifiedTitanicPassenger
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
                Ticket = x.Ticket
            }).ToArray();
            return returnValue;
        }

        public static IEnumerable<TitanicPassenger> GetTrainData()
        {
            using var reader = new StreamReader("./train.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<TitanicPassengerCsvRow>();
            var returnValue = records.Select(x => new TitanicPassenger
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
            return returnValue;
        }

        public static IDictionary<int, bool> GetAnswerLookup()
        {
            using var reader = new StreamReader("./answers.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            using var dr = new CsvDataReader(csv);
            var answerList = new List<Answer>();
            while (dr.Read())
            {
                var answer = new Answer
                {
                    PassengerId = dr.GetInt32(0),
                    Survived = dr.GetBoolean(1)
                };
                answerList.Add(answer);
            }

            var dict = answerList.ToDictionary(x => x.PassengerId, x => x.Survived);
            return dict;
        }

        public static void WriteRefinedData(IEnumerable<RefinedPassengerData> input) 
        {
            using var writer = new StreamWriter("refineddata.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(input);
        }

        public static IEnumerable<TitanicPassenger> GetAgeTrainingData()
        {
            var testData = GetTrainData();
            var ageTrainData = testData.Where(x => x.Age != float.NaN).ToArray();
            return ageTrainData;
        }

        public static IEnumerable<TitanicPassenger> GetAgeTestData()
        {
            var testData = GetTrainData();
            var ageTrainData = testData.Where(x => x.Age != float.NaN).ToArray();
            return ageTrainData;
        }
    }
}
