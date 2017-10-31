#r @"fake/FAKE/tools/FakeLib.dll"

namespace Linn.FakeTools
module Utils = 
    open Fake
    open System
    open System.Text
    open System.Text.RegularExpressions


    let private kTagRegexBuilder = sprintf @"%s-[v]?0*([0-9]+)\.0*([0-9]+)\.0*([0-9]+)-(release|stable|development|beta|nightly|internal|developer).*"

    let Exec command args =
        let result = Shell.Exec(command, args)
        if result <> 0 then failwithf "%s exited with error %d" command result


    let UpdatePlist plistPath shortVersion version =
        Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleShortVersionString " + shortVersion + "' " + plistPath)
        Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleVersion " + version + "' " + plistPath)

    let FormatBuildQuality (quality:string) =
        match quality.ToLowerInvariant() with
            | "release" | "stable" -> "Release"
            | "beta" -> "Beta"
            | "nightly" -> "Nightly"
            | "internal" | "development" -> "Internal"
            | _ -> "Developer"

    let FormatTitle (titleBase:string) (buildQuality:string) =
        match FormatBuildQuality buildQuality with
            | "Release" -> titleBase
            | _ -> String.Format("{0} ({1})", titleBase, FormatBuildQuality buildQuality)

    
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

    let ParseReleaseVersionFromTag tag = 
        let pattern = kTagRegexBuilder "[^-]*"
        match tag with
        | Regex(pattern) [ major; minor; build; quality ] -> sprintf @"%s.%s.%s" major minor build
        | _ -> failwith (sprintf "Failed to parse release version from tag %s" tag)

    let ParseBuildQualityFromTag tag =
        let pattern = kTagRegexBuilder "[^-]*"
        match tag with 
        | Regex(pattern) [ major; minor; build; quality ] -> FormatBuildQuality quality
        | _ -> failwith (sprintf "Failed to parse build quality from tag %s" tag)

    let ShouldPublishReleaseFromTag productName tag = 
        let pattern = kTagRegexBuilder (sprintf "(%s)" productName)
        match tag with 
        | Regex(pattern) [ product; major; minor; build; quality ] -> true
        | _ -> false