using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly ExerciseDbApi _exerciseDbApi;

        public ExercisesController( )
        {
            _exerciseDbApi = new ExerciseDbApi(new HttpClient());
        }

        [HttpGet("GetExercises")]
        public async Task<List<Exercise>> GetExercises(String muscle)
        {
            return await _exerciseDbApi.GetExercisesAsync(muscle);
        }
    }
}
