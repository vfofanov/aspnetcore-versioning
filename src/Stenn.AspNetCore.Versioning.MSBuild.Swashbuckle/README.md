# MSBuild target to run Swashbuckle cli for generate open api specification

This package contains MSBuild target **SwaggerGen_GenerateOpenApi** to run [Swashbuckle cli](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Cli) for generate open api specification during building ASP.Net Core project

## Prerequisites
 - [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) Gen must be configured for ASP.NET Core project
 - [Swashbuckle cli](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Cli) must be [installed as a tool for dotnet cli](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#swashbuckleaspnetcorecli)

## Configuration

- Add ApiVersions.props file to project's root folder with next content

```XML
<Project>

    <PropertyGroup>
<!--        This property enable\disable open api generation task-->
        <RunSwaggerGen>True</RunSwaggerGen>
<!--        Directory for open api generated specification. This folder relative to project root folder.-->
        <SwaggerGenOutputDirectory>open-api</SwaggerGenOutputDirectory>
<!--        Recreate open ai specs output folder before generation-->
        <SwaggerGenOutputDirectoryRecreate>True</SwaggerGenOutputDirectoryRecreate>
<!--        Generate open api spec in json format-->
        <SwaggerGenJsonOutput>True</SwaggerGenJsonOutput>
<!--        Generate open api spec in yaml format-->
        <SwaggerGenYamlOutput>False</SwaggerGenYamlOutput>
<!--        open api specification result file prefix. Full name will be constracted like: $(SwaggerGenFileNamePrefix)$(ApiVersion)-->
        <SwaggerGenFileNamePrefix>$(MSBuildProjectName).openapi.</SwaggerGenFileNamePrefix>
    </PropertyGroup>
    
    <ItemGroup>
<!--        Add SwaggerGenOutputDirectory to project-->
        <Folder Include="$(SwaggerGenOutputDirectory)"/>
    </ItemGroup>
    
    <ItemGroup>
<!--        Api version declaration. Add one for every api version in ASP.NET core project.-->
        <ApiVersion Include="v1" Major="1" Minor="0" Depricated="false" IsDefault="true" />
    </ItemGroup>
    
</Project>
```
_I failed to found a way how to copy file during nuget package installation. If you know a way, please share it with me_

- Add ApiVersion for every api version in ASP.NET core project

  For Swashbuckle cli only Include is metter in ApiVersion item. Other item's metadata value for versioning source generator



##Result

After building the project you will find open api specs file(s) in $(ProjectDirectory)\$(SwaggerGenOutputDirectory) destination
