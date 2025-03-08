# RDP Ncentral Tool

## Installation

Copy the Files into: C:\Program Files (x86)\N-able Technologies\Custom Protocol Handler\
It will overwrite the original tool.

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