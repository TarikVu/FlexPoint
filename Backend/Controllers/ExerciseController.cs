using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{

    // API controller to fetch exercises from ExerciseDB
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseDbApi _exerciseDbApi;

        public ExerciseController(ExerciseDbApi exerciseDbApi)
        {
            _exerciseDbApi = exerciseDbApi;
        }

        [HttpGet("GetExercises")]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            var exercises = await _exerciseDbApi.GetExercisesAsync();
            return Ok(exercises);
        }
    }
}
