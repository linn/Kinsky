// include Fake
#r @"fake/FAKE/tools/FakeLib.dll"
#load @"./Utils.fsx"

open Fake
open Fake.AssemblyInfoFile
open Linn.FakeTools


// get command line args

let nugetPath = getBuildParam "nugetpath"
let version = getBuildParam "version"
let quality = getBuildParam "quality"
let buildTag = getBuildParam "tag"

let isReleaseTag = 
    match buildTag with
    | "" -> false
    | _ -> Utils.ShouldPublishReleaseFromTag "Kinsky" buildTag || Utils.ShouldPublishReleaseFromTag "KinskyWindows" buildTag

let buildVersion = 
    match isReleaseTag with
    | false -> version
    | _ -> Utils.ParseReleaseVersionFromTag buildTag

let buildQuality =
    match isReleaseTag with
    | false -> quality
    | _ -> Utils.ParseBuildQualityFromTag buildTag


// Default target
Target "Default" (fun _ ->
    trace "Default build"
)

// Clean
Target "Clean" (fun _ ->
    CleanDirs [ "./packages"; "./build" ]
)

// Restore packages
Target "RestorePackages" (fun _ ->
    "./KinskyWindows.sln"
    |> RestoreMSSolutionPackages(fun p ->
    {
        p with
            ToolPath = nugetPath
            OutputPath = "packages"
    })
)

// Build solution
Target "Build" (fun _ ->

    UpdateAttributes "./Kinsky/Properties/AssemblyInfo.cs"
        [Attribute.Title (Utils.FormatTitle "Linn Kinsky" buildQuality)
         Attribute.Version buildVersion]

    let setParams defaults =
        { defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Optimize", "True"
                    "DebugSymbols", "True"
                    "Configuration", "Release"
                    "Platform", "Any CPU"
                    "BuildVersion", buildVersion
                ]
         }
    build setParams "KinskyWindows.sln"
        |> DoNothing



    CreateDir "./build/artifacts"
    CopyFile ("./build/artifacts/Kinsky_" + buildVersion + "_win.exe") "./build/Kinsky/bin/Windows/Installer/Release/InstallerKinsky.exe"
    CopyFile ("./build/artifacts/Kinsky_" + buildVersion + "_win.dll") "./build/Kinsky/bin/Windows/Installer/Release/UpdaterKinsky.dll"
)

// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "Build"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"

