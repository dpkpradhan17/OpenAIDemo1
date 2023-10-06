using Microsoft.AspNetCore.Mvc;
using OpenAIDemo.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication1.Repository;

namespace OpenAIDemo.Repository
{
    public class FileOperations
    {
        AIresponse ob = new AIresponse();
        List<ProjectInfo> ProjectFolderPath = new List<ProjectInfo>();
        public async void FileIterate(string folderPath, string fileExtension,string prompt, string key)
        {
            try
            {
                string[] files = Directory.GetFiles(folderPath,"*" + fileExtension, SearchOption.AllDirectories);
                string temp = prompt;

                foreach (string file in files)
                {
                    string fileContent = File.ReadAllText(file);
                    prompt = prompt + " "+fileContent;
                    File.WriteAllText(file,await ob.GetResponse(prompt,key));
                    prompt = temp;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<List<string>> FolderContent(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            List<string> File = new List<string>();
            File.AddRange(files);
            return File;
        }

        public async Task<List<string>> ProjectFolders(string folderPath)
        {
            string[] folders = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);

            List<string> systemGeneratedFolders = new List<string>
            {
                "bin",
                "obj",
                ".vs",
                "node_modules" // Add more folders as needed
            };

            List<string> filteredFolders = folders
        .Where(folder => !systemGeneratedFolders.Any(systemFolder =>
            folder.Contains(systemFolder, StringComparison.OrdinalIgnoreCase)))
        .ToList();

            List<string> Folder = new List<string>();
            Folder.AddRange(filteredFolders);
            return Folder;
        }

        public async Task<List<string>> FilesInFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);

            // Filter out any directories from the list of files
            List<string> fileList = files
                .Where(file => !Directory.Exists(file))
                .ToList();

            return fileList;
        }
        public async Task<string> FindTargetFramework(string ProjectPath)
        {
            string[] files = Directory.GetFiles(ProjectPath, "*.csproj", SearchOption.AllDirectories);
            string file= files[0] ;
            string fileContent = await File.ReadAllTextAsync(file);
            var match = Regex.Match(fileContent, "<TargetFramework>(.*?)</TargetFramework>");

            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value.ToString();
            }
            return null;

        }

        public async Task<bool> ParseFileThroughAPI(string filePath , string prompt, string outputFolderPath, string key)
        {
            string fileContent = await File.ReadAllTextAsync(filePath);
            string temp = prompt;
            string fullPrompt = prompt + "\n " + fileContent; 
            string Response = await ob.GetResponse(fullPrompt, key);
            if (Response.Contains("Error"))
            {
                return false;
            }
            else
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string outputFileName = fileNameWithoutExtension + ".txt";
                //string fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(outputFolderPath, outputFileName);
                File.WriteAllText(newFilePath, Response);
                //File.WriteAllText(filePath, Response);
                return true;
            }

        }
        public async Task<bool> ParsePOMFile(string ProjectPath, string outputFolderPath, string key)
        {
            string prompt = "Convert this .csproj code to pom.xml code using maven : \n";
            string[] files = Directory.GetFiles(ProjectPath, "*.csproj", SearchOption.AllDirectories);
            string file = files[0];
            string fileContent = await File.ReadAllTextAsync(file);
            string temp = prompt;
            string fullPrompt = prompt + "\n " + fileContent;
            string Response = await ob.GetResponse(fullPrompt, key);
            if (Response.Contains("Error"))
            {
                return false;
            }
            else
            {
                string fileNameWithoutExtension ="pom";
                string outputFileName = fileNameWithoutExtension + ".xml";
                //string fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(outputFolderPath, outputFileName);
                File.WriteAllText(newFilePath, Response);
                //File.WriteAllText(filePath, Response);
                return true;
            }

        }
        public async Task<bool> ParseFolderThroughAPIForJAVA(string folderPath, string prompt, string outputFolderPath, string key)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Folder does not exist: " + folderPath);
                return false;
            }

            if (!Directory.Exists(outputFolderPath))
            {
                Console.WriteLine("Output folder does not exist: " + outputFolderPath);
                return false;
            }

            string[] filePaths = Directory.GetFiles(folderPath);
            bool success = true;

            foreach (string filePath in filePaths)
            {
                string fileContent = await File.ReadAllTextAsync(filePath);
                string fullPrompt = prompt + "\n" + fileContent;
                string Response = await ob.GetResponse(fullPrompt, key);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string outputSubfolderName = Path.GetFileName(folderPath); // Get the parent folder name
                string outputFolderPathWithSubfolder = Path.Combine(outputFolderPath, outputSubfolderName);

                // Ensure the subfolder exists in the outputFolderPath
                Directory.CreateDirectory(outputFolderPathWithSubfolder);

                string outputFileName = fileNameWithoutExtension + ".java";
                string newFilePath = Path.Combine(outputFolderPathWithSubfolder, outputFileName);
                File.WriteAllText(newFilePath, Response);
                fullPrompt = "";
                /*
                 * if (Response.Contains("Error"))
                {
                    Console.WriteLine("Error processing file: " + filePath);
                    success = false;
                    fullPrompt = "";
                }
                else
                {
                  
                }*/
            }

            return success;
        }

        public string CreateDirectoryStructureLM(string sourceSolutionPath, string targetSolutionPath)
        {
            try
            {
                /*if (!Directory.Exists(targetSolutionPath))
                {
                    Directory.CreateDirectory(targetSolutionPath);
                }*/
                string SourceDirrectoryPath = Path.GetDirectoryName(sourceSolutionPath);
                List<ProjectInfo> projects = ParseSolutionFile(sourceSolutionPath);
                foreach (var project in projects)
                {
                    string ProjectDirectory = Path.GetDirectoryName(project.FilePath);
                    string projectPath = Path.Combine(SourceDirrectoryPath, ProjectDirectory);
                    //ProjectFolderPath.Add(projectPath);
                    ReplicateDirectoryStructure(projectPath, targetSolutionPath);
                    
                }
                Console.WriteLine("Directory structure replicated.");
                //CreateSolutionFile(targetSolutionPath, projects);
                //Console.WriteLine("Solution file created.");
                
                
                return "Directories Replicated";
            }
            catch (Exception ex)
            {
                return $"Error replicating the directory structure: {ex.Message}";
            }
        }

        public List<ProjectInfo> GetProjectPaths(string solutionPath)
        {
            string SourceDirrectoryPath = Path.GetDirectoryName(solutionPath);
            List<ProjectInfo> projects = ParseSolutionFile(solutionPath);
            foreach (var project in projects)
            {
                string ProjectDirectory = Path.GetDirectoryName(project.FilePath);
                string projectPath = Path.Combine(SourceDirrectoryPath, ProjectDirectory);

                ProjectFolderPath.Add(new ProjectInfo
                {
                    Type=project.Type,
                    Name=project.Name,
                    FilePath=projectPath
                });

            }
            return ProjectFolderPath;
        }
        public List<ProjectInfo> ParseSolutionFile(string solutionFilePath)
        {
            List<ProjectInfo> projects = new List<ProjectInfo>();

            try
            {
                string solutionFileContent = File.ReadAllText(solutionFilePath);

                // Define regular expressions to match project lines.
                Regex projectLineRegex = new Regex(@"Project\(""(.*?)""\) = ""(.*?)"", ""(.*?)""");

                // Split the solution file content by lines.
                string[] lines = solutionFileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    Match match = projectLineRegex.Match(line);
                    if (match.Success)
                    {
                        // Extract project information from the matched line.
                        string type = match.Groups[1].Value;
                        string name = match.Groups[2].Value;
                        string filePath = match.Groups[3].Value;

                        // Add the project information to the list.
                        projects.Add(new ProjectInfo
                        {
                            Type = type,
                            Name = name,
                            FilePath = filePath
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the solution file: {ex.Message}");
            }

            return projects;
        }
        public void CreateSolutionFile(string solutionFilePath, List<ProjectInfo> projects)
        {
            try
            {
                // Create a StringBuilder to build the solution file content.
                StringBuilder solutionContent = new StringBuilder();

                // Add the solution header.
                solutionContent.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
                solutionContent.AppendLine("# Visual Studio Version 17");
                solutionContent.AppendLine("VisualStudioVersion = 17.7.34031.279");
                solutionContent.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

                // Add project entries.
                foreach (var project in projects)
                {
                    // Generate a new GUID for the project identifier.
                    string projectGuid = Guid.NewGuid().ToString("B").ToUpper();

                    solutionContent.AppendLine($"Project(\"{project.Type}\") = \"{project.Name}\", \"{project.FilePath}\", \"{projectGuid}\"");
                    solutionContent.AppendLine("EndProject");

                    // Add the global section for the project with the new GUID.
                    solutionContent.AppendLine($"GlobalSection({projectGuid}.Debug|Any CPU.ActiveCfg) = Debug|Any CPU");
                    solutionContent.AppendLine("    Debug|Any CPU = Debug|Any CPU");
                    solutionContent.AppendLine("EndGlobalSection");
                    solutionContent.AppendLine($"GlobalSection({projectGuid}.Debug|Any CPU.Build.0) = Debug|Any CPU");
                    solutionContent.AppendLine("    Debug|Any CPU = Debug|Any CPU");
                    solutionContent.AppendLine("EndGlobalSection");
                    solutionContent.AppendLine($"GlobalSection({projectGuid}.Release|Any CPU.ActiveCfg) = Release|Any CPU");
                    solutionContent.AppendLine("    Release|Any CPU = Release|Any CPU");
                    solutionContent.AppendLine("EndGlobalSection");
                    solutionContent.AppendLine($"GlobalSection({projectGuid}.Release|Any CPU.Build.0) = Release|Any CPU");
                    solutionContent.AppendLine("    Release|Any CPU = Release|Any CPU");
                    solutionContent.AppendLine("EndGlobalSection");
                }

                // Add the global sections (if needed).
                solutionContent.AppendLine("Global");
                solutionContent.AppendLine("    GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                solutionContent.AppendLine("        Debug|Any CPU = Debug|Any CPU");
                solutionContent.AppendLine("        Release|Any CPU = Release|Any CPU");
                solutionContent.AppendLine("    EndGlobalSection");
                // Add more global sections as needed.
                solutionContent.AppendLine("EndGlobal");

                // Write the solution content to the file.
                File.WriteAllText(solutionFilePath, solutionContent.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating the solution file: {ex.Message}");
            }
        }
        public void ReplicateDirectoryStructure(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            string[] subDirs = Directory.GetDirectories(sourceDirName);

            foreach (string subDir in subDirs)
            {
                // Create the corresponding subdirectory in the destination.
                string destSubDir = Path.Combine(destDirName, Path.GetFileName(subDir));
                Directory.CreateDirectory(destSubDir);

                // Recursively replicate the subdirectory's structure.
                ReplicateDirectoryStructure(subDir, destSubDir);
            }
        }

        

    }
}
