using Microsoft.AspNetCore.Mvc;
using TitanicPredictor.MachineLearningManual;

namespace TitanicPredictor.Controllers
{
    public class PredictionController : Controller
    {
        private readonly ITitanicPredictor titanicPredictor;

        public PredictionController(ITitanicPredictor titanicPredictor)
        {
            this.titanicPredictor = titanicPredictor;
        }

        [HttpPost]
        public IActionResult Index([FromBody] PredictionSubmission submission)
        {
            var subissionValue = new NewTitanicPassenger
            {
                Age = submission.Age,
                Embarked = submission.Embarked.ToString(),
                Family = submission.Family,
                Fare = submission.Fare,
                Pclass = int.Parse(submission.Cabin),
                Sex = submission.Sex,
                Title = submission.Title
            };

            var prediction = this.titanicPredictor.PredictSurvival(subissionValue);

            return Json(new { Result =  prediction});
        }
    }

    public readonly record struct PredictionSubmission
    {
        public string Cabin { get; init; }
        public string Title { get; init; }
        public string Sex { get; init; }
        public int Age { get; init; }
        public int Family { get; init; }
        public char Embarked { get; init; }
        public float Fare { get; init; } 
    }
}