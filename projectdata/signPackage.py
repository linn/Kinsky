#!/usr/bin/python

import subprocess
import json
import fnmatch
import sys
import os
import shutil

# helper functions

def find_files(path, pattern):
  matches = []
  for root, dirnames, filenames in os.walk(path):
    for filename in fnmatch.filter(filenames, pattern):
      matches.append(os.path.join(root, filename))
  return matches

def read_json(filename):
  with open(filename) as json_data:
    return json.load(json_data)

def read_file(filename):
  with file(filename) as f:
    return f.read()

target_dir=sys.argv[1]
unsigned_apk=os.path.join(target_dir, 'uk.co.linn.kinsky.apk')
signed_apk=os.path.join(target_dir, 'uk.co.linn.kinsky-LinnSigned.apk')
android_home=os.environ['ANDROID_HOME']
keys_settings_file=os.environ['ANDROID_KEYSTORES']



# find the zipalign command, raising an error if one isn't found
zipalign=find_files(android_home, 'zipalign*')
if len(zipalign) == 0:
  sys.exit("no zipalign command found")
#take the first command that was found
zipalign=zipalign[0]

# read the settings file
signing_settings=read_json(keys_settings_file)

# make the destination directory tree
if os.path.exists(install_dir):
    shutil.rmtree(install_dir)
os.makedirs(install_dir)

# sign the jar
cmd = ['jarsigner',
  '-verbose',
  '-sigalg', 'MD5withRSA',
  '-digestalg', 'SHA1',
  '-keystore', signing_settings['keystore'],
  '-storepass', signing_settings['storepass'],
    '-keypass', signing_settings['keypass'],
    unsigned_apk,
    signing_settings['keyalias']
]
cmd = ' '.join(cmd)
subprocess.check_call(cmd, shell=True)

# zipalign it
cmd = [zipalign,
  '-f',
  '-v',
  '4',
  unsigned_apk,
  signed_apk
]
cmd = ' '.join(cmd)
subprocess.check_call(cmd, shell=True)
