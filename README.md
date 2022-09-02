# DotNet Cli 工具

一个简单的Abp代码辅助生成工具。 

[![NuGet](https://img.shields.io/nuget/v/Passingwind.AbpProjectTools?style=flat-square)](https://www.nuget.org/packages/Passingwind.AbpProjectTools)

####  一键生成实体的 
- repository
- app service 
- http controller (从 app service 生成)
- typescript types (基于 openapi)
- typescript service (基于 openapi)

## 如何使用

1. 安装 `dotnet tool install --global Passingwind.AbpProjectTools`
2. 在项目根目录，打开命令行，输入 `abptool gen backend <command> <options>` 
>  举例：生成实体 `Product` 的 `Repository` : `abptool gen backend repository --project-name Demo --name Product --slu-dir d://work/MyProject`

## 更多命令和参数使用 `--help` 查看

### 后端相关命令
``` shell 
> abptool gen backend --help

backend

Usage:
  AbpProjectTools [options] generate backend [command]

Options:
  --slu-dir <slu-dir> (REQUIRED)            The solution root dir
  --name <name> (REQUIRED)                  The Domain entity name
  --project-name <project-name> (REQUIRED)  The project name
  --overwrite                               [default: False]
  --templates <templates>                   The template files directory
  -?, -h, --help                            Show help and usage information

Commands:
  domain-service                           生成一个DomainService文件
  repository                               生成对应的 IRepository 和 EfRepository 文件
  app-service                              生成对应的 AppService 文件
  http-controller                          生成 HttpAPI Controller 文件，基于 AppService
```

### 前端相关命令
``` shell
> abptool gen fontend --help
fontend

Usage:
  AbpProjectTools [options] generate fontend [command]

Options:
  -?, -h, --help  Show help and usage information

Commands:
  ts                                        生成后端 api 对应的 typescript 的相关文件（types和service），基于 openapi 。
``` 
