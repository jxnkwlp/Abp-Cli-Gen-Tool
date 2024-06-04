# DotNet Cli 工具

一个命令行的 Abp 代码辅助生成工具。

[![NuGet](https://img.shields.io/nuget/v/Passingwind.AbpProjectTools?style=flat-square)](https://www.nuget.org/packages/Passingwind.AbpProjectTools)

### 一键生成 Entity 的

- repository
- domain manager
- app service
- http controller (从 app service 生成)
- typescript types (基于 openapi json)
- typescript service (基于 openapi json)

#### 其他

- antd pro CRUD 页面

## 如何使用

1. 安装 `dotnet tool install --global Passingwind.AbpProjectTools`
2. 在项目根目录，打开命令行工具，输入 `abptool gen ` 开始
   > 举例：生成实体 `Product` 的 `Repository` : `abptool gen backend repository --project-name Demo --name Product`

## 更多命令和参数使用 `--help` 查看

### 后端相关命令

```shell
> abptool gen backend --help

backend
  Generate abp repository, CRUD app service, http controller code

Usage:
  AbpProjectTools [options] generate backend [command]

Options:
  --slu-dir <slu-dir> (REQUIRED)            The solution root dir. Default is current directory
  --name <name> (REQUIRED)                  The Domain entity name
  --project-name <project-name> (REQUIRED)  The project name. Default is solution name if found in solution directory
  --overwrite                               Over write file if the target file exists [default: False]
  --templates <templates>                   The template files directory
  -?, -h, --help                            Show help and usage information

Commands:
  domain-service                           生成一个DomainService文件
  repository                               生成对应的 IRepository 和 EfRepository 文件
  app-service                              生成对应的 AppService 文件
  http-controller                          生成 HttpAPI Controller 文件，基于 AppService
```

### 前端相关命令

```shell
> abptool gen fontend --help
fontend

Usage:
  AbpProjectTools [options] generate fontend [command]

Options:
  -?, -h, --help  Show help and usage information

Commands:
  ts                                        生成后端 api 对应的 typescript 的相关文件（types和service），基于 openapi 。
```
