using AbpProjectTools.Models;

namespace AbpProjectTools.Services;

public interface IBackendCodeGenerator
{
    /// <summary>
    /// Generate the repository code
    /// </summary> 
    void GenerateRepository(BackendCodeRepositoryGenerateOptions options);
    /// <summary>
    /// Generate the domain manager code
    /// </summary> 
    void GenerateDomain(BackendCodeDomainGenerateOptions options);
    void GenerateAppService(BackendCodeAppServiceGenerateOptions options);
    void GenerateHttpApi(BackendCodeHttpApiGenerateOptions options);
}
