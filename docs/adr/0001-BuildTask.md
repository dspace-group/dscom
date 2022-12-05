# Build Task

## Story

I as a developer want to export a type library for the assembly currently build for my project. I accept that this might restrict the build to the target runtime my assembly is being built for and the architecture x64. But my advantage is that I don't have to deal with the installation of the global tool nor handling a local tool manifest. The build task shall be added automatically if I add a reference to the resulting package.

## State

Accepted

## Decision

The task will be added as a separate NuGet package. It will consist of three files:

* an assembly file containing the MsBuild task.
* a properties file containing the properties to consider for the build which can be changed by the user.
* a targets file containing the properties that cannot be overridden and the items to consume as well as the MsBuild task to build.

## Dropped Decisions

* Create a targets file that will create a local tool manifest at installation-time, which installs the tool at installation-time, and call it at build-time.
