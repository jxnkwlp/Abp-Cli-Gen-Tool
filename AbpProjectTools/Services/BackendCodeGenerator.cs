using AbpProjectTools.Models;
using Humanizer;
using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpProjectTools.Services;

public class BackendCodeGenerator : IBackendCodeGenerator
{
    private readonly BackendCodeGenerateGlobalOptions _globalOptions;
    private readonly TemplateService _templateService = new TemplateService();
    private readonly TypeService _typeService;

    public BackendCodeGenerator(BackendCodeGenerateGlobalOptions globalOptions)
    {
        _globalOptions = globalOptions;
        _typeService = new TypeService(globalOptions.SluDir);
    }

    protected void InitArgs()
    {
        if (string.IsNullOrWhiteSpace(_globalOptions.SluName))
        {
            var sluFile = Directory.EnumerateFiles(_globalOptions.SluDir, "*.sln").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(sluFile))
                throw new InvalidOperationException($"The slu name is empty and not found slu file in path: {_globalOptions.SluDir}");

            _globalOptions.SluName = Path.GetFileNameWithoutExtension(sluFile).Pascalize();
        }
    }

    protected void Check()
    {
        if (!Directory.Exists(_globalOptions.SluDir))
        {
            throw new DirectoryNotFoundException("The directory does not exist: " + _globalOptions.SluDir);
        }
    }


    /// <summary>
    ///  Print the command line arguments
    /// </summary>
    protected void PrintCommandArgs()
    {
        Console.WriteLine($"🚩 Solution directory: {_globalOptions.SluDir.Pastel(Color.GreenYellow)}");
        Console.WriteLine($"🍕 Solution name: {_globalOptions.SluName.Pastel(Color.GreenYellow)}");
    }

    protected bool TryGetDomainProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.Domain";
        if (!string.IsNullOrWhiteSpace(_globalOptions.DomainProjectName))
            projectName = _globalOptions.DomainProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 domain projects found. Please use '--domain-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }

    protected bool TryGetEfProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.EntityFrameworkCore";
        if (!string.IsNullOrWhiteSpace(_globalOptions.EfProjectName))
            projectName = _globalOptions.EfProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 ef projects found. Please use '--ef-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }

    protected bool TryGetMongoDbProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.MongoDB";
        if (!string.IsNullOrWhiteSpace(_globalOptions.MongodbProjectName))
            projectName = _globalOptions.MongodbProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 mongo db projects found. Please use '--mongodb-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }

    protected bool TryGetAppContractsProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.Application.Contracts";
        if (!string.IsNullOrWhiteSpace(_globalOptions.AppContractProjectName))
            projectName = _globalOptions.AppContractProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 mongo db projects found. Please use '--app-contract-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }

    protected bool TryGetAppServiceProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.Application";
        if (!string.IsNullOrWhiteSpace(_globalOptions.AppServiceProjectName))
            projectName = _globalOptions.AppServiceProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 mongo db projects found. Please use '--app-service-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }

    protected bool TryGetHttpApiProject(out string projectDir)
    {
        projectDir = null;
        var projectName = "*.HttpApi";
        if (!string.IsNullOrWhiteSpace(_globalOptions.HttpApiProjectName))
            projectName = _globalOptions.HttpApiProjectName;

        var list = Directory.EnumerateDirectories(_globalOptions.SluDir, projectName, SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        if (list.Count() > 1)
        {
            var projectNameList = list.Select(x => Path.GetFileName(x)).ToList();

            throw new InvalidProgramException($"More than 1 http api projects found. Please use '--http-api-project-name' to specify the project name." +
                $"\r\n\r\n{string.Join("\n", projectNameList)}");
        }

        projectDir = list.First();

        return true;
    }


    protected bool TryGetProjectDllFile(string dir, out string dllFile)
    {
        dllFile = null;

        var fileName = Path.GetFileName(dir);

        var list = Directory.EnumerateFiles(dir, fileName + ".dll", SearchOption.AllDirectories);

        if (!list.Any())
        {
            return false;
        }

        dllFile = list.First();

        return true;
    }

    protected bool TryFindFile(string dir, string fileName, out string path)
    {
        path = null;
        var filePath = Directory.EnumerateFiles(dir, "" + fileName + ".cs", SearchOption.AllDirectories).FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            path = filePath;
            return true;
        }

        return false;
    }

    protected string GetBaseDir(string projectDir, string fileName)
    {
        string fileDir = "";
        if (TryFindFile(projectDir, fileName, out string fullPath))
        {
            var baseDir = Path.GetDirectoryName(fullPath);
            if (baseDir != projectDir)
                fileDir = Path.GetDirectoryName(fullPath).Substring(projectDir.Length + 1);
        }
        return fileDir;
    }

    private static void WriteFileContent(string filePath, string content, bool overwrite = false)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        //#if DEBUG
        //        Console.WriteLine($"⬇️⬇️ File '{filePath}' Content Preview ... ".Pastel(Color.Aqua));
        //        Console.WriteLine(content.Pastel(Color.Aqua));
        //        Console.WriteLine();
        //#endif

        if (File.Exists(filePath) && overwrite == false)
        {
            Console.WriteLine($"➡️ The file '{filePath}' exists.".Pastel(Color.Yellow));
        }
        else
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
            Console.WriteLine($"⬇️ Write file '{filePath}' successful.".Pastel(Color.Green));
        }
    }

    public void GenerateDomain(BackendCodeDomainGenerateOptions generateOptions)
    {
        Console.WriteLine("\n🔛 Generating domain service code...\n".Pastel(Color.BurlyWood));

        Check();
        InitArgs();
        PrintCommandArgs();

        if (generateOptions.EntityName?.Any() != true)
        {
            throw new InvalidOperationException("The entity names are empty. Please add the entity names in command.");
        }

        GenerateDomainCore(generateOptions);
    }

    public void GenerateRepository(BackendCodeRepositoryGenerateOptions generateOptions)
    {
        Console.WriteLine("\n🔛 Generating repository code...\n".Pastel(Color.BurlyWood));

        Check();
        InitArgs();
        PrintCommandArgs();

        if (generateOptions.EntityName?.Any() != true)
        {
            throw new InvalidOperationException("The entity names are empty. Please add the entity names in command.");
        }

        GenerateRepoCore(generateOptions);
    }

    protected void GenerateDomainCore(BackendCodeDomainGenerateOptions generateOptions)
    {
        if (!TryGetDomainProject(out var domainProject))
        {
            throw new InvalidOperationException("The domain project not found");
        }

        if (!TryGetProjectDllFile(domainProject, out var projectDllFile))
        {
            throw new InvalidOperationException("The domain project DLL file not found. Please build it first.");
        }

        Console.WriteLine($"🕶️ Use DLL: {projectDllFile.Pastel(Color.LightGreen)}");

        foreach (var entityName in generateOptions.EntityName)
        {
            GenerateDomainCore(generateOptions, entityName, domainProject, projectDllFile);
        }
    }

    protected void GenerateDomainCore(BackendCodeDomainGenerateOptions generateOptions, string entityName, string projectDir, string dllFile)
    {
        var domainTypeInfo = _typeService.GetDomainInfo(dllFile, entityName);

        var renderData = new BackendCodeDomainGenerateModel()
        {
            Type = domainTypeInfo,
        };

        var fileContent = _templateService.Render("DomainService", renderData);

        string fileDir = GetBaseDir(projectDir, entityName);

        var filePath = Path.Combine(projectDir, fileDir, $"{domainTypeInfo.Name}Manager.cs");

        WriteFileContent(filePath, fileContent, _globalOptions.Force);
    }


    protected void GenerateRepoCore(BackendCodeRepositoryGenerateOptions generateOptions)
    {
        if (!TryGetDomainProject(out var domainProject))
        {
            throw new InvalidOperationException("The domain project not found");
        }

        if (!TryGetProjectDllFile(domainProject, out var projectDllFile))
        {
            throw new InvalidOperationException("The domain project DLL file not found. Please build it first.");
        }

        // EF
        string efProject = null;
        ClassTypeInfoModel efDbContextType = null;
        var hasEfProject = TryGetEfProject(out efProject);
        if (generateOptions.Ef && !hasEfProject)
        {
            Console.WriteLine("The EF project not exists.".Pastel(Color.Yellow));
        }

        if (hasEfProject && generateOptions.Ef)
        {
            if (!TryGetProjectDllFile(efProject, out var dllFile))
            {
                throw new InvalidOperationException("The ef project DLL file not found. Please build it first.");
            }

            efDbContextType = _typeService.GetRepoDbContext(dllFile);
        }

        // MongoDB 
        ClassTypeInfoModel mongoDbContextType = null;
        var hasMongodbProject = TryGetMongoDbProject(out var mongodbProject);

        if (generateOptions.Mongodb && !hasMongodbProject)
        {
            Console.WriteLine("The MongoDB project not exists.".Pastel(Color.Yellow));
        }

        if (hasMongodbProject && generateOptions.Mongodb)
        {

            if (!TryGetProjectDllFile(mongodbProject, out var dllFile))
            {
                throw new InvalidOperationException("The mongo project DLL file not found. Please build it first.");
            }

            mongoDbContextType = _typeService.GetRepoDbContext(dllFile);
        }

        Console.WriteLine($"🕶️ Use DLL: {projectDllFile.Pastel(Color.LightGreen)}");

        foreach (var entityName in generateOptions.EntityName)
        {
            var domainTypeInfo = _typeService.GetDomainInfo(projectDllFile, entityName);

            // Interface
            if (generateOptions.Interface)
            {
                GenerateDomainRepo(generateOptions, domainProject, domainTypeInfo);
            }

            // EF
            if (hasEfProject)
            {
                GenerateEfRepo(generateOptions, efProject, domainTypeInfo, efDbContextType);
            }

            // MongoDB
            if (hasMongodbProject)
            {
                GenerateMongoDBRepo(generateOptions, mongodbProject, domainTypeInfo, mongoDbContextType);
            }
        }
    }

    protected void GenerateDomainRepo(BackendCodeRepositoryGenerateOptions generateOptions, string projectDir, ClassTypeInfoModel domainTypeInfo)
    {
        var renderData = new BackendCodeDomainGenerateModel()
        {
            SluName = generateOptions.SluName,
            Type = domainTypeInfo,
            ReadonlyRepo = generateOptions.ReadonlyRepo,
        };

        var fileContent = _templateService.Render("DomainRepository", renderData);

        string fileDir = GetBaseDir(projectDir, domainTypeInfo.Name);

        var filePath = Path.Combine(projectDir, fileDir, $"I{domainTypeInfo.Name}Repository.cs");

        WriteFileContent(filePath, fileContent, _globalOptions.Force);
    }

    protected void GenerateEfRepo(BackendCodeRepositoryGenerateOptions generateOptions, string projectDir, ClassTypeInfoModel domainTypeInfo, ClassTypeInfoModel efTypeInfo)
    {
        var renderData = new BackendCodeRepoGenerateModel()
        {
            SluName = generateOptions.SluName,
            Type = domainTypeInfo,
            ReadonlyRepo = generateOptions.ReadonlyRepo,
            DbContextType = efTypeInfo,
        };

        var fileContent = _templateService.Render("EfCoreRepository", renderData);

        var filePath = Path.Combine(projectDir, "EntityFrameworkCore", "Repositories", $"{domainTypeInfo.Name}Repository.cs");

        WriteFileContent(filePath, fileContent, _globalOptions.Force);
    }

    protected void GenerateMongoDBRepo(BackendCodeRepositoryGenerateOptions generateOptions, string projectDir, ClassTypeInfoModel domainTypeInfo, ClassTypeInfoModel mongoDBTypeInfo)
    {
        var renderData = new BackendCodeRepoGenerateModel()
        {
            SluName = generateOptions.SluName,
            Type = domainTypeInfo,
            ReadonlyRepo = generateOptions.ReadonlyRepo,
            DbContextType = mongoDBTypeInfo,
        };

        var fileContent = _templateService.Render("MongoDBRepository", renderData);

        var filePath = Path.Combine(projectDir, "MongoDB", "Repositories", $"{domainTypeInfo.Name}Repository.cs");

        WriteFileContent(filePath, fileContent, _globalOptions.Force);
    }


    public void GenerateAppService(BackendCodeAppServiceGenerateOptions options)
    {
        Console.WriteLine("\n🔛 Generating app service code...\n".Pastel(Color.BurlyWood));

        Check();
        InitArgs();
        PrintCommandArgs();

        if (options.EntityName?.Any() != true)
        {
            throw new InvalidOperationException("The entity names are empty. Please add the entity names in command.");
        }

        GenerateAppServiceCore(options);
    }

    public void GenerateAppServiceCore(BackendCodeAppServiceGenerateOptions options)
    {
        if (!TryGetDomainProject(out var domainProject))
        {
            throw new InvalidOperationException("The domain project not found");
        }

        if (!TryGetProjectDllFile(domainProject, out var projectDllFile))
        {
            throw new InvalidOperationException("The domain project DLL file not found. Please build it first.");
        }

        if (!TryGetAppContractsProject(out var appContractsProject))
        {
            throw new InvalidOperationException("The app contract project not found");
        }

        if (!TryGetAppServiceProject(out var appServiceProject))
        {
            throw new InvalidOperationException("The app service project not found");
        }

        Console.WriteLine($"🕶️ Use DLL: {projectDllFile.Pastel(Color.LightGreen)}");

        foreach (var entityName in options.EntityName)
        {
            GenerateAppServiceCore(
                options,
                entityName: entityName,
                domainProject: domainProject,
                domainDllFile: projectDllFile,
                appContractsProject: appContractsProject,
                appServiceProject: appServiceProject);
        }
    }

    protected void GenerateAppServiceCore(BackendCodeAppServiceGenerateOptions options, string entityName, string domainProject, string domainDllFile, string appContractsProject, string appServiceProject)
    {
        var domainTypeInfo = _typeService.GetDomainInfo(domainDllFile, entityName);

        var renderData = new BackendCodeAppServiceGenerateModel()
        {
            Type = domainTypeInfo,
            SluName = options.SluName,
            Empty = options.Empty,
            Crud = options.Crud,

            ListRequestTypeName = $"{domainTypeInfo.Name}ListRequestDto",
            ListResultTypeName = options.SplitResult ? $"{domainTypeInfo.Name}ListResultDto" : $"{domainTypeInfo.Name}Dto",
            CreateTypeName = options.SplitCreateUpdate ? $"{domainTypeInfo.Name}CreateDto" : $"{domainTypeInfo.Name}CreateOrUpdateDto",
            UpdateTypeName = options.SplitCreateUpdate ? $"{domainTypeInfo.Name}UpdateDto" : $"{domainTypeInfo.Name}CreateOrUpdateDto",
        };

        string fileDir = options.BaseDir;
        if (string.IsNullOrWhiteSpace(fileDir))
            fileDir = GetBaseDir(domainProject, entityName);

        //  App Contracts
        var filePath = Path.Combine(appContractsProject, fileDir, $"I{domainTypeInfo.Name}AppService.cs");
        var fileContent = _templateService.Render("AppContract", renderData);
        WriteFileContent(filePath, fileContent, _globalOptions.Force);

        //  App Service
        filePath = Path.Combine(appServiceProject, fileDir, $"{domainTypeInfo.Name}AppService.cs");
        fileContent = _templateService.Render("AppService", renderData);
        WriteFileContent(filePath, fileContent, _globalOptions.Force);

        if (options.Empty)
        {
            return;
        }

        // DTO 
        void GenerateDtoFile(string typeName, BackendCodeAppServiceDtoGenerateModel model)
        {
            filePath = Path.Combine(appContractsProject, fileDir, $"{typeName}.cs");
            fileContent = _templateService.Render("AppServiceDto", model);
            WriteFileContent(filePath, fileContent, _globalOptions.Force);
        }

        foreach (var refClassType in domainTypeInfo.References)
        {
            GenerateDtoFile(refClassType.Name + "Dto", new BackendCodeAppServiceDtoGenerateModel
            {
                SluName = options.SluName,
                Type = refClassType,
                IsEntityType = true,
                OutputName = refClassType.Name + "Dto",
            });
        }

        if (options.SplitCreateUpdate)
        {
            GenerateDtoFile($"{domainTypeInfo.Name}UpdateDto", new BackendCodeAppServiceDtoGenerateModel
            {
                SluName = options.SluName,
                Type = domainTypeInfo,
                IsCreateOrUpdateType = true,
                OutputName = $"{domainTypeInfo.Name}UpdateDto",
            });

            GenerateDtoFile($"{domainTypeInfo.Name}CreateDto", new BackendCodeAppServiceDtoGenerateModel
            {
                SluName = options.SluName,
                Type = domainTypeInfo,
                IsCreateOrUpdateType = true,
                OutputName = $"{domainTypeInfo.Name}CreateDto"
            });
        }
        else
        {
            GenerateDtoFile($"{domainTypeInfo.Name}CreateOrUpdateDto", new BackendCodeAppServiceDtoGenerateModel
            {
                SluName = options.SluName,
                Type = domainTypeInfo,
                IsCreateOrUpdateType = true,
                OutputName = $"{domainTypeInfo.Name}CreateOrUpdateDto"
            });
        }

        GenerateDtoFile($"{domainTypeInfo.Name}Dto", new BackendCodeAppServiceDtoGenerateModel
        {
            SluName = options.SluName,
            Type = domainTypeInfo,
            IsListResultType = true,
            OutputName = $"{domainTypeInfo.Name}Dto"
        });

        GenerateDtoFile($"{domainTypeInfo.Name}ListRequestDto", new BackendCodeAppServiceDtoGenerateModel
        {
            SluName = options.SluName,
            Type = domainTypeInfo,
            IsListRequestType = true,
            OutputName = $"{domainTypeInfo.Name}ListRequestDto"
        });

        if (options.SplitResult)
        {
            GenerateDtoFile($"{domainTypeInfo.Name}ListResultDto", new BackendCodeAppServiceDtoGenerateModel
            {
                SluName = options.SluName,
                Type = domainTypeInfo,
                IsListResultType = true,
                OutputName = $"{domainTypeInfo.Name}ListResultDto"
            });
        }
    }

    public void GenerateHttpApi(BackendCodeHttpApiGenerateOptions options)
    {
        Console.WriteLine("\n🔛 Generating http api code...\n".Pastel(Color.BurlyWood));

        Check();
        InitArgs();
        PrintCommandArgs();

        if (options.Name?.Any() != true)
        {
            throw new InvalidOperationException("The app service names are empty. Please add it in command.");
        }

        GenerateHttpApiCore(options);
    }

    public void GenerateHttpApiCore(BackendCodeHttpApiGenerateOptions options)
    {
        if (!TryGetAppContractsProject(out var appContractsProject))
        {
            throw new InvalidOperationException("The app contract project not found");
        }

        if (!TryGetProjectDllFile(appContractsProject, out var projectDllFile))
        {
            throw new InvalidOperationException("The app contract project Dll not found");
        }

        if (!TryGetHttpApiProject(out var httpApiProject))
        {
            throw new InvalidOperationException("The http api project not found");
        }

        if (!TryGetProjectDllFile(httpApiProject, out var httApiDllFile))
        {
            throw new InvalidOperationException("The http api project Dll not found");
        }

        Console.WriteLine($"🕶️ Use DLL: {projectDllFile.Pastel(Color.LightGreen)}");

        foreach (var entityName in options.Name)
        {
            GenerateHttpApiCore(
                options,
                name: entityName,
                appProjectDir: appContractsProject,
                projectDll: projectDllFile,
                httpApiProject: httpApiProject,
                httApiDllFile);
        }
    }

    private void GenerateHttpApiCore(BackendCodeHttpApiGenerateOptions options, string name, string appProjectDir, string projectDll, string httpApiProject, string httpApiProjectDll)
    {
        var typeInfo = _typeService.GetAppContract(projectDll, name);
        var baseControllerType = _typeService.GetHttpControllerBaseType(httpApiProjectDll);

        var renderData = new BackendCodeHttpApiGenerateModel()
        {
            Type = typeInfo,
            Name = typeInfo.Name.Replace("AppService", null).TrimStart('I'),
            SluName = options.SluName,
            BaseControllerName = baseControllerType == null ? "ControllerBase" : baseControllerType.Name,
            Routes = GenerateRoutes(typeInfo.Methods, options.Post),
        };

        string fileDir = options.BaseDir;
        if (string.IsNullOrWhiteSpace(fileDir))
            fileDir = GetBaseDir(appProjectDir, typeInfo.Name);

        var filePath = Path.Combine(httpApiProject, fileDir, $"{renderData.Name}Controller.cs");
        var fileContent = _templateService.Render("HttpApiController", renderData);
        WriteFileContent(filePath, fileContent, _globalOptions.Force);
    }

    private static Dictionary<string, BackendCodeHttpApiRouteGenerateModel> GenerateRoutes(IEnumerable<MethodTypeInfoModel> methods, bool usePostMethod)
    {
        var result = new Dictionary<string, BackendCodeHttpApiRouteGenerateModel>();

        foreach (var item in methods)
        {
            string path = item.Name;
            string method = "HttpPost";

            if (item.Name.StartsWith("Create") || item.Name.StartsWith("Add") || item.Name.StartsWith("New"))
            {
                method = "HttpPost";
                path = item.Name.Replace("Create", null).Replace("Add", null).Replace("New", null);
                if (usePostMethod)
                {
                    path = "create";
                }
            }
            else if (item.Name.StartsWith("Update"))
            {
                path = item.Name.Replace("Update", null);
                if (!usePostMethod)
                {
                    method = "HttpPut";
                }
                else
                {
                    path = "update";
                }
            }
            else if (item.Name.StartsWith("Delete") || item.Name.StartsWith("Remove"))
            {
                path = item.Name.Replace("Delete", null).Replace("Remove", null);
                if (!usePostMethod)
                {
                    method = "HttpDelete";
                }
                else
                {
                    path = "delete";
                }
            }
            else if (item.Name.StartsWith("Get") || item.Name.StartsWith("Load"))
            {
                method = "HttpGet";
                path = item
                    .Name
                    .Replace("GetListWith", null)
                    .Replace("GetList", null)
                    .Replace("List", null)
                    .Replace("GetWith", null)
                    .Replace("Get", null)
                    .Replace("Load", null);
            }
            else
            {
                path = RenderHelperFunctions.ToPluralize(RenderHelperFunctions.ToSlugString(item.Name.Replace("Async", null)));
            }

            if (path != null)
            {
                path = path
                    .Replace("With", null)
                    .Replace("Async", null);
                if (!usePostMethod)
                    path = RenderHelperFunctions.ToPluralize(path);
                path = RenderHelperFunctions.ToSlugString(path);
            }

            if (item.Members.Any())
            {
                List<string> nps = new List<string>();
                string tmp = string.Empty;

                foreach (var member in item.Members)
                {
                    if (member.IsClass)
                        continue;

                    if (path.Contains(member.Name))
                    {
                        path += "/{" + member.Name.Camelize() + "}";
                    }
                    else
                    {
                        path = "{" + member.Name.Camelize() + "}/" + path;
                    }
                }

                //if (item.Members.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)))
                //{
                //    nps.Add("{id}");
                //}

                //if (!string.IsNullOrEmpty(path))
                //    nps.Add(path);

                //foreach (var p in item.Members.Where(x => !x.IsClass && !x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)))
                //{
                //    nps.Add("{" + RenderHelperFunctions.CamelCase(p.Name) + "}");
                //}

                //path = string.Join("/", nps);
            }

            path = path.TrimEnd('/');

            if (string.IsNullOrEmpty(path))
                path = null;

            result[item.Name] = new BackendCodeHttpApiRouteGenerateModel
            {
                Path = path,
                Method = method,
            };
        }

        return result;
    }
}
