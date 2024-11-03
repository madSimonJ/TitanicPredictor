using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanicPredictor.Entities
{
    public readonly record struct RefinedPassengerData
    {
        public int PassengerId { get; init; }
        public int Pclass { get; init; }
        public string Title { get; init; }
        public string Sex { get; init; }
        public float Age { get; init; }
        public int Family { get; init; }
        public float Fare { get; init; }
        public string CabinLetter { get; init; }
        public char Embarked { get; init; }
        public bool Survived { get; init; }
    }
}
