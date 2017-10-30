
// include Fake
#r @"fake/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile


// get command line args

let nugetPath = getBuildParam "nugetpath"
let version = getBuildParam "version"
let quality = getBuildParam "quality"


// helper functions that can be moved somewhere common later

let BuildQuality quality =
    if (quality = "Release" || quality = "") then
        ""
    else
        " (" + quality + ")"


// Default target
Target "Default" (fun _ ->
    trace "Default build"
)

// Clean
Target "Clean" (fun _ ->
    CleanDirs [ "./dependencies"; "./build" ]
)

// Restore packages
Target "RestorePackages" (fun _ ->
    "./KinskyWindows.sln"
    |> RestoreMSSolutionPackages(fun p ->
    {
        p with
            ToolPath = nugetPath
            OutputPath = "dependencies/nuget"
    })
)

// Build solution
Target "Build" (fun _ ->

    UpdateAttributes "./Kinsky/Properties/AssemblyInfo.cs"
        [Attribute.Title ("Linn Kinsky" + BuildQuality quality)
         Attribute.Version version
         Attribute.FileVersion version]

    //RegexReplaceInFileWithEncoding "VERSION \".*\"" ("VERSION \"" + version + "\"") System.Text.Encoding.ASCII "./src/Windows/App/Installer/NsiTemplate.txt"

    let setParams defaults =
        { defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Optimize", "True"
                    "DebugSymbols", "True"
                    "Configuration", "Release"
                    "Platform", "x86"
                    "ReleaseVersion", version
                ]
         }
    build setParams "KinskyWindows.sln"
        |> DoNothing



    CreateDir "./build/artifacts"
    CopyFile ("./build/artifacts/Kinsky_" + version + "_win.exe") "./build/Kinsky/bin/Windows/Installer/InstallerKinsky.exe"
    CopyFile ("./build/artifacts/Kinsky_" + version + "_win.dll") "./build/Kinsky/bin/Windows/Installer/UpdaterKinsky.dll"
)

// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "Build"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"

