# RDP Ncentral Tool

## What is this?
This is a replacement for the n-central custom protocol handler to allow more control over the Remote desktop settings.
Settings can be set once for all RDP Sessions, rather than the native per-device customisation.

This only works with n-central appliances that use the SSH Tunnel, the failover HTTP tunnel is not implemented.

## Installation

1. Extract the zip and overwrite the files in: C:\Program Files (x86)\N-able Technologies\Custom Protocol Handler\
2. Configure your settings with RDP-Config.exe and click save.
Settings are saved per-user under AppData\Roaming\NcentralProtocolHandler
3. Use Remote Desktop in N-central.

If the original tool is not already installed, register the file association with the below script:

```
$RegPath = "HKLM:\Software\Classes\swncrc"
$IconPath = "$RegPath\DefaultIcon"
$CommandPath = "$RegPath\shell\open\command"

New-Item -Path $RegPath -Force | Out-Null
New-Item -Path $IconPath -Force | Out-Null
New-Item -Path "$RegPath\shell\open" -Force | Out-Null
New-Item -Path $CommandPath -Force | Out-Null

Set-ItemProperty -Path $RegPath -Name "(Default)" -Value "URL:swncrc Protocol Handler"
Set-ItemProperty -Path $RegPath -Name "URL Protocol" -Value ""
Set-ItemProperty -Path $IconPath -Name "(Default)" -Value "C:\Program Files (x86)\N-able Technologies\Custom Protocol Handler\Remote Application Launcher.exe"
Set-ItemProperty -Path $CommandPath -Name "(Default)" -Value 'C:\Program Files (x86)\N-able Technologies\Custom Protocol Handler\Remote Application Launcher.exe "%1"'

```