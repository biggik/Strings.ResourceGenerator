@echo Off
Echo Publishing %1 to nuget.org
cd C:\development\Status\Strings.ResourceGenerator\Strings.ResourceGenerator
set nuget=%userprofile%\.nuget\nuget.exe
%nuget% push bin\release\%1 oy2okihw4nrb3tms6kvauzinolr45aquleleeplrwz6jty -Source https://nuget.org
