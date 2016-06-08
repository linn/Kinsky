# Defines the build behaviour for continuous integration builds.

import os
import glob
import re

class Builder(OpenHomeBuilder):

    def setup(self):
        self.set_nuget_sln(self.solutionfile)


        # write release version in assembly info and manifest
        f1 = open('Kinsky/Properties/AssemblyInfo.cs', 'r')
        r = re.compile('^\[assembly: AssemblyVersion\(\"(.*?)\"\)\]', re.M)
        c = r.sub('[assembly: AssemblyVersion("' + os.environ['RELEASE_VERSION'] + '")]', f1.read())
        f1.close()
        r = re.compile('^\[assembly: AssemblyTitle\(\"(.*?)\"\)\]', re.M)
        quality = ""
        if os.environ['BUILD_QUALITY'] != 'Release':
            quality = " (" + os.environ['BUILD_QUALITY'] + ")"
        c = r.sub('[assembly: AssemblyTitle("' + "Linn Kinsky" + quality + '")]', c)
        f2 = open('Kinsky/Properties/AssemblyInfo.cs', 'w')
        f2.write(c)
        f2.close()

        if (self.platform == "Android-mono"):
            f1 = open('Kinsky/Android/Properties/AndroidManifest.xml', 'r')
            r = re.compile('android:versionCode\=\".*?\"', re.M)
            c = r.sub('android:versionCode="' + os.environ['BUILD_NUMBER'] + '"', f1.read())
            f1.close()
            r = re.compile('android:versionName\=\".*?\"', re.M)
            c = r.sub('android:versionName="' + os.environ['RELEASE_VERSION'] + '"', c)
            f2 = open('Kinsky/Android/Properties/AndroidManifest.xml', 'w')
            f2.write(c)
            f2.close()
        elif (self.platform == "Ios-armv7"):
            f1 = open('Kinsky/Ios/Info.plist', 'r')
            r = re.compile('<key>CFBundleShortVersionString<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleShortVersionString</key>\n\t<string>' + os.environ['RELEASE_VERSION'] + '</string>', f1.read())
            f1.close()
            r = re.compile('<key>CFBundleVersion<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleVersion</key>\n\t<string>' + os.environ['RELEASE_VERSION'] + '</string>', c)
            f2 = open('Kinsky/Ios/Info.plist', 'w')
            f2.write(c)
            f2.close()
        elif (self.platform == "Mac-x64"):
            f1 = open('Kinsky/Mac/Info.plist', 'r')
            r = re.compile('<key>CFBundleShortVersionString<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleShortVersionString</key>\n\t<string>' + os.environ['RELEASE_VERSION'] + '</string>', f1.read())
            f1.close()
            r = re.compile('<key>CFBundleVersion<\/key>\s*<string>.*<\/string>', re.M)
            c = r.sub('<key>CFBundleVersion</key>\n\t<string>' + os.environ['RELEASE_VERSION'] + '</string>', c)
            f2 = open('Kinsky/Mac/Info.plist', 'w')
            f2.write(c)
            f2.close()


    def configure(self):
        platformsolutions = { 
            "Windows-x86" : "KinskyWindows.sln",
            "Mac-x64" : "KinskyMac.sln",
            "Android-mono" : "KinskyAndroid.sln",
            "Ios-armv7" : "KinskyIos.sln"
        }
        self.solutionfile = platformsolutions[self.platform]


    def clean(self):
        self.msbuild(self.solutionfile, target='Clean', configuration=self.configuration)
        #todo
        #if self.configuration == "Release" and self.platform == "Android-mono":
            # only build AppStore if release build
            #self.msbuild('KazooAndroid.sln', target='Clean', configuration="AppStore")

    def build(self):
        self.msbuild(self.solutionfile, target='Build', configuration=self.configuration)
        #todo
        #if self.configuration == "Release" and self.platform == "Android-mono":
            # only build AppStore if release build
            #props = {'AndroidSigningStorePass': os.environ['ANDROID_SIGNING_STORE_PASS'],
            #         'AndroidSigningKeyPass': os.environ['ANDROID_SIGNING_KEY_PASS']}
            #self.msbuild('KazooAndroid.sln', target='Build', configuration="AppStore", properties=props)
