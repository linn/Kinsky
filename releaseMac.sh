#!/bin/sh

if [ ! -f "./fake/FAKE/tools/Fake.exe" ]; then
  echo Installing FAKE
  nuget install FAKE -OutputDirectory fake -ExcludeVersion -NoCache -NonInteractive -Version 4.52.0
fi

mono ./fake/FAKE/tools/Fake.exe releaseMac.fsx "$@"

