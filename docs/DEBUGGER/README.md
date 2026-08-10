# Debugger Setup Notes

## Summary

The VS Code C# debug workflow was failing with a workspace initialization problem even though the .NET projects themselves were building correctly. The root cause was editor-side workspace and solution configuration rather than an application code error.

## Root causes

- VS Code did not have a stable, explicit launch configuration for the API and Auth projects.
- The workspace debug/build wiring was incomplete for the relevant debug targets.
- The repository initially had a solution model that did not provide the C# language service with a reliable project graph for symbol resolution.
- In this environment, the newer XML-based solution format in [VehicleServiceBooking.slnx](../../VehicleServiceBooking.slnx) was not being handled reliably by the editor for IntelliSense and Go to Definition, even when the projects themselves were valid and buildable.

## What was actually broken

The problem was not that the application code was failing to compile. The problem was that the editor was not loading a robust solution context for the workspace.

That caused two symptoms:

1. Cross-project references were not being discovered consistently.
2. In-project symbol resolution (such as finding classes, interfaces, and services within the same project) could appear broken even though the code was present and the projects built.

In other words, the language service was not getting a trustworthy representation of the workspace graph, so IntelliSense behaved as if the references were missing.

## What was fixed

- Created a conventional solution file at [VehicleServiceBooking.sln](../../VehicleServiceBooking.sln).
- Added all existing projects to that solution so the C# language service could resolve the full project graph.
- Updated [.vscode/settings.json](../../.vscode/settings.json) to make [VehicleServiceBooking.sln](../../VehicleServiceBooking.sln) the default solution for the workspace.
- Updated [.vscode/launch.json](../../.vscode/launch.json) to use explicit .NET launch configurations for the API and Auth projects.
- Added build tasks in [.vscode/tasks.json](../../.vscode/tasks.json) for the API, Auth, and notification functions.
- Reloaded the VS Code window so the C# extension could reinitialize cleanly.

## Why this fixed the issue

The C# extension relies on the loaded solution and project graph to provide IntelliSense, navigation, and reference resolution. When that graph was incomplete or unstable, Visual Studio Code could not reliably resolve symbols even though dotnet build succeeded.

By switching the workspace to a classic .sln file with all projects explicitly included, the editor was able to load a stable solution model. That restored:

- accurate Go to Definition
- proper completion suggestions
- reliable detection of in-project and cross-project references
- consistent debugger/workspace initialization behavior

## Result

The debugger now starts correctly for the relevant projects from Run and Debug, and the editor can resolve symbols and references reliably again without requiring application-code changes.

## launch.json explained

The file [.vscode/launch.json](../../.vscode/launch.json) tells VS Code how to start debugging sessions.

### Top-level structure

- version: the schema version for the debug configuration file.
- compounds: groups of debug configurations that can be started together. These are useful for running the Auth API and Booking API in one flow.

### Compound entries

- Notification (Debug C#): starts the notification function host and then attaches the .NET debugger to the running process.
- Auth + Api (Run): starts the Auth project and the API project together.
- Notification Host+Attach + Auth + Api (Ordered): runs the notification host, attaches the debugger, then starts the Auth and API projects in order.

### Auth launch configuration

- name: the label shown in the VS Code Run and Debug panel.
- type: coreclr tells VS Code to use the .NET debugger.
- request: launch means VS Code starts the process.
- preLaunchTask: runs the build Auth task before launching so the latest binaries are present.
- program: the full path to the built Auth DLL that the debugger launches.
- cwd: the working directory for the process.
- console: internalConsole keeps output in the Debug Console.
- stopAtEntry: false means the app starts normally rather than stopping immediately at the first line.
- env: sets environment variables for the process.
  - ASPNETCORE_URLS sets the URL the Auth service listens on.
  - ASPNETCORE_ENVIRONMENT sets the environment to Development.

### API launch configuration

- name: the display name in the debug list.
- type: coreclr uses the .NET debugger.
- request: launch starts the API process.
- preLaunchTask: runs the build API task before launching.
- program: the path to the built API DLL.
- cwd: the working directory used by the app.
- console: internalConsole sends logs to the Debug Console.
- stopAtEntry: false allows the service to run normally.
- env: provides runtime variables.
  - ASPNETCORE_URLS sets the API listening URL.
  - ASPNETCORE_ENVIRONMENT selects Development configuration.

### Notification function debug entries

- VehicleServiceBooking.Notification.Functions: launches the Azure Functions host in a terminal using the func CLI.
- VehicleServiceBooking.Notification.Functions (Host Debug): same host-start behavior as the standard notification launch, used by compound debug flows.
- VehicleServiceBooking.Notification.Functions (Attach .NET): attaches the debugger to an already running .NET process selected interactively.
  - processId: ${command:pickProcess} asks VS Code to prompt for a process to attach to.
  - justMyCode: true limits stepping to your own code.
  - requireExactSource: false makes debugging more tolerant when source mapping is slightly off.

## tasks.json explained

The file [.vscode/tasks.json](../../.vscode/tasks.json) defines build tasks that VS Code can run before debugging.

### Top-level structure

- version: the task schema version.
- tasks: the list of available tasks.

### Shared task properties

- label: the name shown in VS Code for the task.
- type: shell means the task runs in a terminal shell.
- command: the executable to run, here dotnet.
- args: the arguments passed to the command.
- problemMatcher: $msCompile tells VS Code to parse compiler output and show problems in the Problems panel.
- group: build marks the task as a build task so it can be run from the Build menu or referenced by debug configurations.

### build API task

- Runs dotnet build against the API project.
- This ensures that the latest API binaries exist before debugging starts.

### build Auth task

- Runs dotnet build against the Auth project.
- This keeps the Auth service binaries up to date before launch.

### build Notification Functions task

- Runs dotnet build against the notification functions project.
- This prepares the functions host build before running it under debug.

## Why these settings matter

These files do not change the application logic. They tell VS Code and the .NET debugger how to start the services in a predictable way. That matters because the debugger needs:

1. a known executable to run,
2. the correct working directory,
3. the correct environment variables,
4. a build step that creates the latest binaries,
5. and a way to attach to the notification host when needed.
