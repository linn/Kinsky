#!/usr/bin/env python

import os
import os.path
import shutil
from optparse import OptionParser
import subprocess

script_path, script_name = os.path.split(__file__)

kNotificationsDirEng = '/local/share/oss/applications/kinsky/notifications/'
kNotificationsDirKiboko = 'kiboko.linn.co.uk:/var/www.oss/applications/kinsky/notifications'

def rsync(aSrc, aDest, aDryRun, additionalArgs=''):
    rsync = "rsync %s %s --itemize-changes --recursive --delete --delete-excluded --compress --checksum --links --perms %s %s" % ('--dry-run' if aDryRun else '', additionalArgs, aSrc, aDest)
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

    if not os.path.exists(kNotificationsDirEng): os.makedirs(kNotificationsDirEng)
    rsync(os.path.join(script_path, '../Notifications/'), kNotificationsDirEng, options.debug) # sync with eng
    rsync(kNotificationsDirEng, kNotificationsDirKiboko, options.debug) # sync eng with kiboko

if __name__ == '__main__':
    main()
