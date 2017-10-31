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
    | _ -> Utils.ShouldPublishReleaseFromTag "Kinsky" buildTag || Utils.ShouldPublishReleaseFromTag "KinskyMac" buildTag

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
    "./KinskyMac.sln"
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

    Utils.UpdatePlist "./Kinsky/Mac/Info.plist" buildVersion buildVersion

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
    build setParams "KinskyMac.sln"
        |> DoNothing


    Utils.Exec "python" (sprintf "%s %s %s" "./Kinsky/Mac/Installer/buildinstaller.py" "Release" buildVersion)

    CreateDir "./build/artifacts"
    CopyFile ("./build/artifacts/Kinsky_" + buildVersion + "_osx.pkg") "./build/Kinsky/bin/Mac/Installer/Release/InstallerKinsky.pkg"
    CopyFile ("./build/artifacts/Kinsky_" + buildVersion + "_osx.dll") "./build/Kinsky/bin/Mac/Installer/Release/UpdaterKinsky.dll"
)


// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "Build"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"

