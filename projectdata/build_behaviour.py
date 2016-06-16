# Defines the build behaviour for continuous integration builds.

import os
import glob
import re
import subprocess

class Builder(OpenHomeBuilder):



    def findoption(self, aKey):
        kAdditionalOptions = {
            "RELEASE_VERSION" : "0.0.0",
            "BUILD_NUMBER" : 1,
            "BUILD_QUALITY" : "Development",
            "ANDROID_SIGNING_STORE_PASS" : "",
            "ANDROID_SIGNING_KEY_PASS" : ""
        }
        
        if aKey in os.environ:
            return os.environ[aKey]
        elif aKey in kAdditionalOptions:
            return kAdditionalOptions[aKey]
        return None

    def setup(self):
        platformsolutions = { 
            "Windows-x86" : "KinskyWindows.sln",
            "Mac-x64" : "KinskyMac.sln",
            "Android-mono" : "KinskyAndroid.sln",
            "iOs-armv7" : "KinskyIos.sln",
            "iOs-x86" : "KinskyIos.sln"
        }
        self.solutionfile = platformsolutions[self.platform]
        
        self.set_nuget_sln(self.solutionfile)

        self.releaseversion = self.findoption("RELEASE_VERSION")
        self.buildnumber = self.findoption("BUILD_NUMBER")
        self.buildquality = self.findoption("BUILD_QUALITY")
        self.androidpasses = {
            'AndroidSigningStorePass': self.findoption("ANDROID_SIGNING_STORE_PASS"),
            'AndroidSigningKeyPass': self.findoption("ANDROID_SIGNING_KEY_PASS")
        }

        # write release version in assembly info and manifest
        f1 = open('Kinsky/Properties/AssemblyInfo.cs', 'r')
        r = re.compile('^\[assembly: AssemblyVersion\(\"(.*?)\"\)\]', re.M)
        c = r.sub('[assembly: AssemblyVersion("' + self.releaseversion + '")]', f1.read())
        f1.close()

        r = re.compile('^\[assembly: AssemblyInformationalVersion\(\"(.*?)\"\)\]', re.M)        
        c = r.sub('[assembly: AssemblyInformationalVersion("' + self.releaseversion + '")]', c)

        r = re.compile('^\[assembly: AssemblyTitle\(\"(.*?)\"\)\]', re.M)
        quality = ""
        if self.buildquality != 'Release':
            quality = " (" + self.buildquality + ")"
        c = r.sub('[assembly: AssemblyTitle("' + "Linn Kinsky" + quality + '")]', c)

        f2 = open('Kinsky/Properties/AssemblyInfo.cs', 'w')
        f2.write(c)
        f2.close()

        if (self.platform == "Android-mono"):
            f1 = open('Kinsky/Android/Properties/AndroidManifest.xml', 'r')
            r = re.compile('android:versionCode\=\".*?\"', re.M)
            c = r.sub('android:versionCode="' + self.buildnumber + '"', f1.read())
            f1.close()
            r = re.compile('android:versionName\=\".*?\"', re.M)
            c = r.sub('android:versionName="' + self.releaseversion + '"', c)
            f2 = open('Kinsky/Android/Properties/AndroidManifest.xml', 'w')
            f2.write(c)
            f2.close()
        elif (self.platform == "Ios-armv7"):
            f1 = open('Kinsky/Ios/Info.plist', 'r')
            r = re.compile('<key>CFBundleShortVersionString<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleShortVersionString</key>\n\t<string>' + self.releaseversion + '</string>', f1.read())
            f1.close()
            r = re.compile('<key>CFBundleVersion<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleVersion</key>\n\t<string>' + self.releaseversion + '</string>', c)
            f2 = open('Kinsky/Ios/Info.plist', 'w')
            f2.write(c)
            f2.close()
        elif (self.platform == "Mac-x64"):
            f1 = open('Kinsky/Mac/Info.plist', 'r')
            r = re.compile('<key>CFBundleShortVersionString<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleShortVersionString</key>\n\t<string>' + self.releaseversion + '</string>', f1.read())
            f1.close()
            r = re.compile('<key>CFBundleVersion<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleVersion</key>\n\t<string>' + self.releaseversion + '</string>', c)
            f2 = open('Kinsky/Mac/Info.plist', 'w')
            f2.write(c)
            f2.close()

        # unlock ios and mac keychains on build slaves for non interactive code signing
        if (self.platform == "Ios-armv7" or self.platform == "Mac-x64"):
            cmdLine='~/unlock-keychain.py'
            subprocess.check_call(cmdLine, shell=True)

    def clean(self):
        if self.platform == 'iOs-armv7' or self.platform == 'iOs-x86':
            platformTarget = "iPhoneSimulator" if self.platform == 'iOs-x86' else "iPhone"
            self.msbuild(self.solutionfile, target='Clean', configuration=self.configuration, platform=platformTarget)
            if self.configuration == "Release" and platformTarget == "iPhone":
                # only build AdHoc/AppStore if release build on armv7
                self.msbuild(self.solutionfile, target='Clean', configuration="AdHoc", platform=platformTarget)
                self.msbuild(self.solutionfile, target='Clean', configuration="AppStore", platform=platformTarget)    
        else:
            self.msbuild(self.solutionfile, target='Clean', configuration=self.configuration)
            if self.configuration == "Release" and self.platform == "Android-mono":
                # only build AppStore if release build
                self.msbuild(self.solutionfile, target='Clean', configuration="AppStore")
            if self.configuration == "Release" and self.platform == "iOs-armv7":
                self.msbuild(self.solutionfile, target='Build', configuration=self.configuration)

    def build(self):
        if self.platform == 'iOs-armv7' or self.platform == 'iOs-x86':
            platformTarget = "iPhoneSimulator" if self.platform == 'iOs-x86' else "iPhone"
            self.msbuild(self.solutionfile, target='Build', configuration=self.configuration, platform=platformTarget)
            if self.configuration == "Release" and platformTarget == "iPhone":
                # only build AdHoc/AppStore if release build on armv7
                self.msbuild(self.solutionfile, target='Build', configuration="AdHoc", platform=platformTarget)
                self.msbuild(self.solutionfile, target='Build', configuration="AppStore", platform=platformTarget)
        else:
            self.msbuild(self.solutionfile, target='Build', configuration=self.configuration, properties={'ReleaseVersion':self.releaseversion})
            if self.configuration == "Release" and self.platform == "Android-mono":
                # only build AppStore if release build
                self.msbuild(self.solutionfile, target='Build', configuration="AppStore", properties=self.androidpasses)

            if self.configuration == "Release" and self.platform == "Mac-x64":
                makeinstallercmd = "python buildinstaller.py %s %s" % (self.configuration, self.releaseversion)
                print makeinstallercmd
                subprocess.check_call(makeinstallercmd, shell=True, cwd="Kinsky/Mac/Installer/")