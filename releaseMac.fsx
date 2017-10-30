
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


let Exec command args =
    let result = Shell.Exec(command, args)
    if result <> 0 then failwithf "%s exited with error %d" command result


let UpdatePlist plistPath shortVersion version =
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleShortVersionString " + shortVersion + "' " + plistPath)
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleVersion " + version + "' " + plistPath)



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
    "./KinskyMac.sln"
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
        [Attribute.Title ("Linn Konfig" + BuildQuality quality)
         Attribute.Version version]

    UpdatePlist "./Kinsky/Mac/Info.plist" version version

    XmlPoke "./src/Mac/Updater/distribution.dist" "/installer-gui-script/pkg-ref/@version" version

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
                    "BuildVersion", version
                ]
         }
    build setParams "KinskyWindows.sln"
        |> DoNothing

    CreateDir "./build/artifacts"
    CopyFile ("./build/artifacts/Konfig_" + version + "_osx.pkg") "./build/Konfig/Mac/bin/Release/Konfig.pkg"
    CopyFile ("./build/artifacts/Konfig_" + version + "_osx.dll") "./build/KonfigUpdater/Mac/bin/Release/KonfigMacUpdater.dll"
)


// Dependencies
"Clean"
    ==> "RestorePackages"
    ==> "Build"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"

