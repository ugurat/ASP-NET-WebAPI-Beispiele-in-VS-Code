

# ASP.NET WebAPI Beispiele in VS Code

Diese Beispiele zeigen, wie ASP.NET WebAPI mithilfe von Visual Studio Code und CLI-Befehlen in einem Webprojekt verwendet wird. Die Projekte können Schritt für Schritt abgeschlossen werden, wobei der Fokus auf praktischen VS Code und Kommandozeilen-Workflows liegt.


## Einfache ASP.NET WebAPI Erstellung mit VS Code und Veröffentlichung mit Docker

Dieser Leitfaden erklärt Schritt für Schritt, wie mit Visual Studio Code und .NET 6.0 eine einfache ASP.NET WebAPI erstellt und anschließend auf Docker veröffentlicht wird. Im Projekt wird ein Person-Modell mit einigen Beispielwerten über den PersonController aufgelistet.

[ASP.NET WebAPI mit Models und Docker](/ASP.NET-WebAPI--mit-Models-Docker-VSCODE/readme.md)


## Einfache ASP.NET WebAPI-Erstellung in VS Code, ohne Interface und ohne Docker

Dieser Leitfaden erklärt Schritt für Schritt, wie mit Visual Studio Code und .NET 6.0 eine einfache ASP.NET WebAPI ohne Interface und ohne Docker erstellt werden kann.

[ASP.NET WebAPI+EF ohne Interface und ohne Docker](/ASP.NET-WebAPI-EF-ohne-Interface-ohne-Docker/readme.md)

## ASP.NET WebAPI-Erstellung mit Entity Framework, Interface und Docker

Dieser Leitfaden beschreibt Schritt für Schritt, wie mit Visual Studio Code und .NET 6.0 eine ASP.NET WebAPI erstellt und auf Docker veröffentlicht wird. Es können alle Schritte befolgt werden, um das Projekt erfolgreich abzuschließen.

[ASP.NET WebAPI+EF mit Interface mit Docker](/ASP.NET-WebAPI-EF-mit-Interface-mit-Docker/readme.md)

----


## Git Konto wechseln und commiten

```` bash
git init

touch .gitignore

git add .
git commit -m "Initial commit"

git branch -M main 

git remote add origin https://github.com/ugurat/ASP-NET-WebAPI-Beispiele-in-VS-Code.git

git push -u origin main
````

Falls Fehler:

```` bash
PASSWORT LÖSCHEN und ERNEUT ANMELDEN

Gehe zu "Windows-Anmeldeinformationen": 
Unter Windows-Anmeldeinformationen "gespeicherte Zugangsdaten für Webseiten und Anwendungen" finden.

Suche nach gespeicherten GitHub-Einträgen: 
git:https://github.com oder Ähnliches.

Eintrag löschen und erneut versuchen: 

git push -u origin main
````
  
Aktualisiert

```` bash
git add .
git commit -m "aktualisiert"
git push -u origin main
````

Überschreiben

```` bash

git init

git add .
git commit -m "Initial commit"

git branch -M main 

git remote add origin https://github.com/ugurat/ASP-NET-WebAPI-Beispiele-in-VS-Code.git


git push -u origin main --force

````

Mit dem Parameter `--force` wird Git-Repo überschrieben. 

----


## Entwickler
- **Name**: Ugur CIGDEM
- **E-Mail**: [ugurat@gmail.com](mailto:ugurat@gmail.com)

---

## Markdown-Datei (.md)

---

