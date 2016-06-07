#!/bin/sh

#> UserLog.nib
#> Plugins

RESOURCES_DIR=$2/Contents/Resources
TARGET_DIR=$2/Contents/MacOs

echo "Copying files..."
cp -vf ../../External/libmonobjc.2.dylib ${TARGET_DIR}
#cp -vf ../../../Kinsky/Resources/Kinsky2.icns ${TARGET_DIR}
cp -vf ../..//UserLog.nib ${TARGET_DIR}
echo "Done copying files"

