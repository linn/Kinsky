#!/usr/bin/env python

import os
import os.path
import shutil
from optparse import OptionParser
import subprocess

kNotificationsDir = '/local/share/oss/applications/kazoo/notifications/'

def rsync(aSrc, aDest, aDryRun):
    rsync = "rsync %s --itemize-changes --recursive --delete --delete-excluded --compress --checksum --links --perms %s %s" % ('--dry-run' if aDryRun else '', aSrc, aDest)
    subprocess.check_call(rsync, shell=True)

def main():
    parser = OptionParser(usage="publishNotifications [--dry-run]")
    parser.add_option("-d", "--dry-run",
                  action="store_true", dest="debug", default=False,
                  help="debug mode")

    options, args = parser.parse_args()
    if len(args) != 0:
        parser.print_usage()
        return False

    if not os.path.exists(kNotificationsDir): os.makedirs(kNotificationsDir)
    rsync('../Notifications', kNotificationsDir, options.debug)

if __name__ == '__main__':
    main()
