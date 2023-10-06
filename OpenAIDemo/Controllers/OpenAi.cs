using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OpenAIDemo.Repository;

namespace OpenAIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAi : ControllerBase
    {
        [HttpGet]
        [Route("[[FileChange]]")]
        public IActionResult FileChange([FromQuery] string folderPath,[FromQuery] string fileExtension, [FromQuery] string prompt, [FromQuery]string key)
        {

            FileOperations ob = new FileOperations();
            ob.FileIterate(folderPath, fileExtension, prompt,key);

            return Ok();
        }
        [HttpPost]
        [Route("FolderContent")]
        public async Task<IActionResult> ProjectContent([FromQuery] string folderPath)
        {
            FileOperations ob = new FileOperations();
            List<string> result = await ob.FolderContent(folderPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost]
        [Route("ProjectFolders")]
        public async Task<IActionResult> ProjectFolders([FromQuery] string folderPath)
        {
            FileOperations ob = new FileOperations();
            List<string> result = await ob.ProjectFolders(folderPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost]
        [Route("FilesInFolder")]
        public async Task<IActionResult> FilesInFolder([FromQuery] string folderPath)
        {
            FileOperations ob = new FileOperations();
            List<string> result = await ob.FilesInFolder(folderPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpGet]
        [Route("FindTargetFramework")]
        public async Task<IActionResult> FindTargetFramework([FromQuery] string ProjectPath)
        {
            FileOperations ob = new FileOperations();
            string result = await ob.FindTargetFramework(ProjectPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }
        [HttpPost]
        [Route("ParseFileThroughAPI")]
        public async Task<IActionResult> ParseFileThroughAPI([FromQuery] string filePath, [FromQuery] string prompt, [FromQuery] string outputFolderPath, [FromQuery]string key)
        {
            FileOperations ob = new FileOperations();
            bool result = await ob.ParseFileThroughAPI(filePath, prompt, outputFolderPath, key);

            if (result == true)
            {
                return Ok("Parsing success");
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost]
        [Route("CreateDirectoryStructure")]
        public IActionResult CreateDirectoryStructure([FromQuery]string solutionPath, string outputFoldderPath)
        {
            FileOperations ob = new FileOperations();
            var result = ob.CreateDirectoryStructureLM(solutionPath,outputFoldderPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost]
        [Route("GetProjectPaths")]
        public IActionResult GetProjectPaths(string SolutionPath)
        {
            FileOperations ob = new FileOperations();
            var result = ob.GetProjectPaths(SolutionPath);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }
        [HttpPost]
        [Route("ParseFolderThroughAPIJava")]
        public async Task<IActionResult> ParseFolderThroughAPIJava([FromQuery] string folderPath, [FromQuery] string prompt, [FromQuery] string outputFolderPath, [FromQuery] string key)
        {
            FileOperations ob = new FileOperations();
            bool result = await ob.ParseFolderThroughAPIForJAVA(folderPath, prompt, outputFolderPath, key);

            if (result == true)
            {
                return Ok("Parsing success");
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost]
        [Route("ParsePOM")]
        public async Task<IActionResult> ParsePOM([FromQuery] string ProjectPath, string outputFolderPath, string key)
        {
            FileOperations ob = new FileOperations();
            bool result = await ob.ParsePOMFile(ProjectPath,outputFolderPath, key);

            if (result == true)
            {
                return Ok("Parsing success");
            }
            else
            {
                return BadRequest("Something went wrong!");
            }
        }

    }
}
