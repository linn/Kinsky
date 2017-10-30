@echo off

if not exist "fake\FAKE\tools\Fake.exe" (
    echo Installing FAKE
    nuget install FAKE -OutputDirectory fake -ExcludeVersion -NoCache -NonInteractive -Source https://nuget.org/api/v2
)

fake\FAKE\tools\Fake.exe releaseWindows.fsx %*
