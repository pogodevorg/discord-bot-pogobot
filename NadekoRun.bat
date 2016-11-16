@ECHO off
@TITLE NadekoBot
CD NadekoBot\src\NadekoBot
dotnet run --configuration GlobalNadeko
ECHO NadekoBot has been succesfully stopped, press any key to close this window.
TITLE NadekoBot - Stopped
CD %~dp0
PAUSE >nul 2>&1
