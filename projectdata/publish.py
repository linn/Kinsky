#!/usr/bin/env python

import os
import os.path
import shutil
import zipfile
from optparse import OptionParser
import subprocess

script_path, script_name = os.path.split(__file__)
kArtifactsDir = os.path.join(script_path, '../build/artifacts/slave=%s/%s')

kFilenameFormat = 'Kinsky_{0}_{1}.{2}'
kDirFormat = '/local/share/oss/Releases/Kinsky/Davaar/'
kDirFormatDebug = '~/testpublishkinsky/' 


kArtifacts = [
    # Kinsky_4.x.x_and.apk  
    { 
        "slave":"Android-mono", 
        "prefix" : "and", 
        "suffix" : "apk", 
        "path": "Kinsky/build/Kinsky/bin/Android/AppStore/uk.co.linn.kinsky-Signed.apk", 
        "zip" : False, 
        "curl" : False 
    },   
    # Kinsky_4.x.x_osx.pkg 
    { 
        "slave":"Mac-x64", 
        "prefix" : "osx", 
        "suffix" : "pkg", 
        "path": "Kinsky/build/Kinsky/bin/Mac/Installer/Release/InstallerKinsky.pkg", 
        "zip" : False, 
        "curl" : False
    },
    # Kinsky_4.x.x_osx.dll 
    { 
        "slave":"Mac-x64", 
        "prefix" : "osx", 
        "suffix" : "dll", 
        "path": "Kinsky/build/Kinsky/bin/Mac/Installer/Release/UpdaterKinsky.dll", 
        "zip" : False, 
        "curl" : False
    },        
    # Kinsky_4.x.x_win.exe 
    { 
        "slave":"Windows-x86", 
        "prefix" : "win", 
        "suffix" : "exe", 
        "path": "Kinsky/build/Kinsky/bin/Windows/Installer/Release/InstallerKinsky.exe", 
        "zip" : False, 
        "curl" : False
    },
    # Kinsky_4.x.x_win.dll 
    { 
        "slave":"Windows-x86", 
        "prefix" : "win", 
        "suffix" : "dll", 
        "path": "Kinsky/build/Kinsky/bin/Windows/Installer/Release/UpdaterKinsky.dll", 
        "zip" : False, 
        "curl" : False
    },        
    # Kinsky_4.x.x_all.ipa 
    { 
        "slave":"iOs-armv7", 
        "prefix" : "all", 
        "suffix" : "ipa", 
        "path": "Kinsky/build/Kinsky/bin/iPhone/AppStore/InstallerKinsky.ipa" , 
        "zip" : False, 
        "curl" : False
    },
    # Kinsky_4.x.x_all.dsym.zip
    { 
        "slave":"iOs-armv7", 
        "prefix" : "all.dsym", 
        "suffix" : "zip", 
        "path": "Kinsky/build/Kinsky/bin/iPhone/AppStore/Kinsky.app.dSYM", 
        "zip" : True, 
        "curl" : True
    },
    # Kinsky_4.x.x_adhoc_all.ipa 
    { 
        "slave":"iOs-armv7", 
        "prefix" : "adhoc_all", 
        "suffix" : "ipa", 
        "path": "Kinsky/build/Kinsky/bin/iPhone/AdHoc/InstallerKinsky.ipa" , 
        "zip" : False, 
        "curl" : False
    },
    # Kinsky_4.x.x_adhoc_all.dsym.zip
    { 
        "slave":"iOs-armv7", 
        "prefix" : "adhoc_all.dsym", 
        "suffix" : "zip", 
        "path": "Kinsky/build/Kinsky/bin/iPhone/AdHoc/Kinsky.app.dSYM", 
        "zip" : True, 
        "curl" : True
    }
]

def zipdir(path, ziph):
    # ziph is zipfile handle
    for root, dirs, files in os.walk(path):
        for file in files:
            ziph.write(os.path.join(root, file))

def zip(aFilename, aDirectory):
    zipf = zipfile.ZipFile(aFilename, 'w')
    zipdir(aDirectory, zipf)
    zipf.close()

def main():
    parser = OptionParser(usage="publish [--dry-run] STATUS([Nightly|Development|Internal|Beta|Release]) VERSION(x.x.x)")
    parser.add_option("-d", "--dry-run",
                  action="store_true", dest="debug", default=False,
                  help="debug mode")

    options, args = parser.parse_args()
    if len(args) != 2:
        parser.print_usage()
        return False

    buildtype=args[0]
    version=args[1]

    # check build type
    if (buildtype not in ['Nightly', 'Development', 'Internal', 'Beta', 'Release']):
        raise ("Invalid build type '%s'" % buildtype)

    destDir = kDirFormatDebug if options.debug else kDirFormat
    if not os.path.exists(destDir): os.makedirs(destDir)

    if options.debug or buildtype in ['Beta', 'Release']:
        for artifact in kArtifacts:
            src = kArtifactsDir % (artifact['slave'],artifact['path'])
            dest = os.path.join(destDir, kFilenameFormat.format(version, artifact['prefix'], artifact['suffix']))
            if artifact['zip']:
                zip(dest, src)
            else:
                shutil.copy2(src, dest)
            if artifact['curl']:
                # upload dSYM to insights
                lcurlcmd = 'curl -F "dsym=@%s;type=application/zip" https://xaapi.xamarin.com/api/dsym?apikey=129c76d1b4043e568d19a9fea8a1f5534cdae703' % dest
                subprocess.check_call(lcurlcmd, shell=True)

if __name__ == '__main__':
    main()
