#!/usr/bin/python

import os
import sys
import subprocess
from optparse import OptionParser
import glob
import shutil
import plistlib
from xml.dom.minidom import parseString


kScriptDir = os.path.dirname(os.path.realpath(__file__)) + os.sep


def GetArgs():
    parser = OptionParser(usage="buildinstaller <<configuration>> <<version>>")
    options, args = parser.parse_args()

    if (len(args) != 2):
        parser.print_usage()
        raise Exception("Incorrect args: " + ",".join(a for a in args))

    configuration = args[0]
    version = args[1]

    builddir = os.path.abspath(os.path.join(kScriptDir,"../../../build"))
    releasedir = os.path.abspath(os.path.join(builddir,"Kinsky/bin/Mac", configuration))
    installerdir = os.path.abspath(os.path.join(builddir,"Kinsky/bin/Mac/Installer", configuration))

    if (not os.path.isdir(releasedir)):
        raise Exception("directory not found: " + releasedir)

    if os.path.exists(installerdir):
        shutil.rmtree(installerdir)
    os.makedirs(installerdir)

    return (releasedir, installerdir, version)

def PackageBuild(outputDir, componentPath, appName, appNamespace, scriptsDir=None, installLocation='/Applications', pkgBuild='pkgbuild', sourceOptionName='--component', componentPlist=None):
    appIdentifier = "%s.%s" % (appNamespace, appName)
    outputPkg = os.path.join(outputDir, "%s.pkg" % appIdentifier)
    scripts = "" if not scriptsDir else "--scripts \"%s\"" % scriptsDir
    componentPlist = "" if not componentPlist else "--component-plist \"%s\"" % componentPlist
    pkgbuildCom = "\"%s\" %s \"%s\" --install-location \"%s\" %s --identifier %s %s \"%s\"" % (pkgBuild, sourceOptionName, componentPath, installLocation, scripts, appIdentifier, componentPlist, outputPkg)
    print pkgbuildCom
    subprocess.check_call(pkgbuildCom, shell=True)
    return outputPkg

def BuildUpdater(outDir, updaterName,  appName, installer, codeFilename):
    updater = os.path.join(outDir, updaterName)
    buildUpdaterCom = "dmcs /nologo /t:library /res:\"%s\" /out:\"%s\" \"%s\""  % (installer, updater, codeFilename)
    print buildUpdaterCom
    subprocess.check_call(buildUpdaterCom, shell=True)
    return updater

def ProductBuildSynthesizeDistFile(outDir, package):
    distFile = os.path.join(outDir, "%s.dist" % os.path.basename(package))
    productBuildCom = "productbuild --synthesize --package \"%s\" \"%s\"" % (package, distFile)
    print productBuildCom
    subprocess.check_call(productBuildCom, shell=True)
    return distFile

def UpdateDistributionFile(filename, version, title=None, license=None, requireRestart=False, minOsVersion='10.7'):
    # open the distribution.dist file
    f = open(filename, 'r')
    distFile = f.read()
    f.close()

    doc = parseString(distFile)
    root = doc.documentElement

    if title is not None:
        titleElem = doc.createElement('title')
        titleElem.appendChild(doc.createTextNode(title))
        root.appendChild(titleElem)

    # insert a license entry if present
    if license is not None:
        licenseElem = doc.createElement('license')
        licenseElem.setAttribute('file', license)
        root.appendChild(licenseElem)

    pkgElems = root.getElementsByTagName('pkg-ref')
    for elem in pkgElems:
        if requireRestart and elem.hasAttribute('onConclusion'):
            elem.setAttribute('onConclusion', 'RequireRestart')
        if elem.hasAttribute('version'):
            elem.setAttribute('version', version)

    if minOsVersion:
        osVersElem = doc.createElement('allowed-os-versions')
        osVerElem = doc.createElement('os-version')
        osVerElem.setAttribute('min', minOsVersion)
        osVersElem.appendChild(osVerElem)
        root.appendChild(osVersElem)

    f = open(filename, 'w')
    f.write(doc.toxml())
    f.close()


def ProductBuildCreateDistribution(buildDir, packageName, packageSearchPaths, resourcesDir, distFile, key, productBuild='productbuild'):
    if isinstance(packageSearchPaths, str):
        packageSearchPaths = [packageSearchPaths]

    packagePaths = "".join([" --package-path \"%s\"" % path for path in packageSearchPaths])

    pkgFile = os.path.join(buildDir, packageName)

    productbuildCom = "\"%s\" --distribution \"%s\"  --resources \"%s\" %s --sign \"%s\" \"%s\"" % (productBuild, distFile, resourcesDir, packagePaths, key, pkgFile)
    print productbuildCom
    subprocess.check_call(productbuildCom, shell=True)
    return pkgFile

def main():
    (releasedir, installerdir, version) = GetArgs()

    appName = "Kinsky"
    appFullName = "Kinsky"

    installerTmpDir = os.path.join(installerdir, "InstallerTemp")
    if os.path.exists(installerTmpDir):
        shutil.rmtree(installerTmpDir) 
    os.makedirs(installerTmpDir)
    pkgDir = os.path.join(installerTmpDir, "PackageRoot")
    os.makedirs(pkgDir)

    resourcesdir = os.path.join(installerTmpDir, 'Resources')
    os.makedirs(resourcesdir)
    shutil.copyfile(os.path.join(kScriptDir, "../../../license.txt"), os.path.join(resourcesdir, "License.txt"))

    scriptsDir = os.path.join(installerTmpDir, "Scripts")
    os.makedirs(scriptsDir)
    shutil.copyfile(os.path.join(kScriptDir, "postinstall"), os.path.join(scriptsDir, "postinstall"))

    
    bundleSrc = os.path.join(releasedir, "Kinsky.app")
    bundle = os.path.join(pkgDir, "Kinsky.app")
    shutil.copytree(bundleSrc, bundle, symlinks=True) 


    componentplist = os.path.join(installerdir, 'PkgInfo.plist')
    plistparts = [{
        'RootRelativeBundlePath':"%s.app" % appFullName,
        'BundleHasStrictIdentifier':False,
        'BundleIsRelocatable':True,
        'BundleIsVersionChecked':False,
        'BundleOverwriteAction':"upgrade"
    }]
    plistlib.writePlist(plistparts, componentplist)

    pkg = PackageBuild(installerdir, pkgDir, "Kinsky", "uk.co.linn", sourceOptionName="--root", componentPlist=componentplist, scriptsDir=scriptsDir)

    distFile = ProductBuildSynthesizeDistFile(installerdir, pkg)
    UpdateDistributionFile(distFile, version, title="%s" % appName, license='License.txt')
    installer = ProductBuildCreateDistribution(installerdir, "Installer%s.pkg" % appName, [installerdir], resourcesdir, distFile, "Developer ID Installer: Linn Products Ltd")

    BuildUpdater(installerdir, "Updater%s.dll" % appName, appName, installer, os.path.join(os.path.abspath(kScriptDir), 'Updater.cs'))



if __name__ == "__main__":
    main()
