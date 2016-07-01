#!/usr/bin/env python

import os
import shutil

script_path, script_name = os.path.split(__file__)
kArtifactsDir = os.path.join(script_path, '../build/artifacts')

def rmdir(dirname):
    if os.path.isdir(dirname):
        shutil.rmtree(dirname)

def clean():
    rmdir(kArtifactsDir)

if __name__ == '__main__':
    clean()
