#!/bin/sh

#> UserLog.nib
#> Plugins

TARGET_DIR=$1/Kinsky.app/Contents/Resources
TARGET_DIR2=$1/Kinsky.app/Contents/MacOs

echo "Copying files..."
cp -vf ../../../External/libmonobjc.2.dylib ${TARGET_DIR}
cp -vf ../../../External/libmonobjc.2.dylib ${TARGET_DIR2}
cp -vf ../../../Kinsky/Resources/Kinsky2.icns ${TARGET_DIR}
cp -vf $1/UserLog.nib ${TARGET_DIR}
echo "Done copying files"

echo "Copying resources..."
mkdir -v ${TARGET_DIR}/Icons
mkdir -v ${TARGET_DIR}/Images
cp -vf ../../../Layouts/Kinsky/Desktop2/Icons/* ${TARGET_DIR}/Icons
cp -vf ../../../Layouts/Kinsky/Desktop2/Images/* ${TARGET_DIR}/Images
echo "Done copying resources"

echo "Copying plugins..."
cp -vfR $1/Plugins ${TARGET_DIR}
echo "Done copying plugins"

