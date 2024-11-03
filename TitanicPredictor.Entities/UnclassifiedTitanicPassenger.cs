namespace TitanicPredictor.Entities
{
    public class UnclassifiedTitanicPassenger
    {
        public int PassengerId { get; set; }
        public int Pclass { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public float Age { get; set; }
        public int SibSp { get; set; }
        public int Parch { get; set; }
        public string Ticket { get; set; }
        public float Fare { get; set; }
        public string Cabin { get; set; }
        public char? Embarked { get; set; }
    }
}
