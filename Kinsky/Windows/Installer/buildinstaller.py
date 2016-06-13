#!/usr/bin/python

from optparse import OptionParser
import os
import subprocess


kSignTool = "signtool sign /debug /v /a /sm /t http://timestamp.verisign.com/scripts/timestamp.dll"

kScriptDir = os.path.dirname(os.path.realpath(__file__))



def MakeInstaller(releasedir, version, installerdir, product):

    installerexe = "Installer%s.exe" % (product)
    d = {
         'product'   : product,
         'version'   : version,
         'outfile'   : os.path.join(installerdir, installerexe),
         'launch'    : "%s.exe" % (product),
         'launchDir' : "Linn\\%s" % (product),
         'icon'  : os.path.normpath(os.path.join(kScriptDir, 'icon.ico')),
         'header'  : os.path.normpath(os.path.join(kScriptDir, 'header.bmp')),
         'finish'  : os.path.normpath(os.path.join(kScriptDir, 'finish.bmp')),
         'license'  : os.path.normpath(os.path.join(kScriptDir, '../../../license.txt')),
        }

    
    templateFile = open(os.path.normpath(os.path.join(kScriptDir, 'NsiTemplate.txt')), 'rb')
    template = templateFile.read()
    templateFile.close()


    installFiles = []
    uninstallFiles = []
    for item in os.listdir(releasedir):
        installFiles.append("\nSetOutPath \"$INSTDIR\\\"\n")
        uninstallFiles.append("\nSetOutPath \"$INSTDIR\\\"\n")
        if os.path.isfile(os.path.join(releasedir, item)):
            installFiles.append("File %s" % os.path.join(releasedir, item))
            uninstallFiles.append("Delete \"$INSTDIR\\%s\"" % item)            
        else:
            installFiles.append("File /r %s" % os.path.join(releasedir, item))
            uninstallFiles.append("RMDir /r \"$INSTDIR\\%s\"" % item)            

    uninstallFiles += "\nRMDir \"$INSTDIR\.\""
    
    d['install'] = "".join(installFiles)    
    d['delete'] = "".join(uninstallFiles)

    nsifile = os.path.join(installerdir, "Installer.nsi")
    file = open(nsifile, 'wb')
    file.write(template % d)
    file.close()

    cmd = "makensis -V2 Installer.nsi"
    print cmd    
    subprocess.check_call(cmd, shell=True, cwd=installerdir)

    return os.path.join(installerdir, installerexe)


def SignExe(exefile):
    cmd = "%s %s" % (kSignTool, exefile)
    print cmd    
    subprocess.check_call(cmd, shell=True)

def GetArgs():
    parser = OptionParser(usage="buildinstaller <<configuration>> <<version>>")
    options, args = parser.parse_args()
    
    if (len(args) != 2):
        parser.print_usage()
        raise Exception("Incorrect args: " + ",".join(a for a in args))

    configuration = args[0]
    version = args[1]

    builddir = os.path.abspath(os.path.join(kScriptDir,"../../../build"))
    releasedir = os.path.abspath(os.path.join(builddir,"Kinsky/bin/Windows", configuration))
    installerdir = os.path.abspath(os.path.join(builddir,"Kinsky/bin/Windows/Installer", configuration))

    if (not os.path.isdir(releasedir)):
        raise Exception("directory not found: " + releasedir)

    if (not os.path.exists(installerdir)):
        os.makedirs(installerdir)

    return (releasedir, installerdir, version)

def BuildUpdater(installerdir, installerexe):
    cmd = "csc /nologo /t:library /res:\"%s\" /out:\"%s\" \"%s\""  % (installerexe, "UpdaterKinsky.dll", os.path.join(kScriptDir, "Updater.cs"))
    print cmd
    subprocess.check_call(cmd, shell=True, cwd=installerdir)

def main():
    (releasedir, installerdir, version) = GetArgs()

    SignExe(os.path.join(releasedir, "Kinsky.exe"))
    installerexe = MakeInstaller(releasedir, version, installerdir, "Kinsky")
    SignExe(installerexe)
    BuildUpdater(installerdir, installerexe)



if __name__ == "__main__":
    main()
