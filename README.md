[![POGODEV](https://github.com/pogodevorg/assets/blob/master/public/img/logo-github.png?raw=true)](https://pogodev.org)

# discord-bot-pogobot
[![Build Status](https://travis-ci.org/pogodevorg/discord-bot-pogobot.svg?branch=1.0)](https://travis-ci.org/pogodevorg/discord-bot-pogobot) [![Code Climate](https://codeclimate.com/github/pogodevorg/discord-bot-pogobot/badges/gpa.svg)](https://codeclimate.com/github/pogodevorg/discord-bot-pogobot) [![Issue Count](https://codeclimate.com/github/pogodevorg/discord-bot-pogobot/badges/issue_count.svg)](https://codeclimate.com/github/pogodevorg/discord-bot-pogobot) [![license](https://img.shields.io/github/license/pogodevorg/discord-bot-pogobot.svg?maxAge=2592000?style=flat-square)](https://github.com/pogodevorg/discord-bot-pogobot/blob/master/LICENSE)

## Table of Contents
* [What is it?](#what-is-it)
* [Installation](#installation)
* [Documentation](#documentation)
* [Contributing](#contributing)
  * [Core Maintainers](#core-maintainers)
* [Licensing](#licensing)
  * [Third Party Licenses](#third-party-licenses)
* [Credits](#credits)

## What is it?
`discord-bot-pogobot` is an open source repo for managing POGODev Discord Server. It is not recommended to use this to host other Discord servers.

## Installation
### Linux
1. `git clone https://github.com/pogodevorg/discord-bot-pogobot.git`
2. `git submodule update --init --recursive`
3. `Install` [.NET Core](https://www.microsoft.com/net/core#linuxubuntu)
4. `dotnet restore`
5. `Configure credentials.json`
6. `dotnet build -c Release`
7. `dotnet run -c Release`

## Documentation
1. [Commands List](http://nadekobot.readthedocs.io/en/1.0/Commands%20List/)
2. [NadekoBot](http://nadekobot.readthedocs.io/en/1.0/)
3. [Discord.NET](http://rtd.discord.foxbot.me/en/legacy/)

## Licensing
[GNU GPL](https://github.com/pogodevorg/discord-bot-pogobot/blob/master/LICENSE) v3 or later.

### Third Party Licenses
    None

## Contributing
Currently, you can contribute to this project by:
* Submitting a detailed [issue](https://github.com/pogodevorg/discord-bot-pogobot/issues/new).
* [Forking the project](https://github.com/pogodevorg/discord-bot-pogobot/fork), and sending a pull request back to for review.

### Core Maintainer
* [![Build Status](https://github.com/fkndean.png?size=36) - fkndean](https://github.com/fkndean)

## Credits

Credits to [Kwoth/NadekoBot](https://github.com/Kwoth/NadekoBot/tree/1.0) for base.

Modified to work with the PogoDev Discord Server ([https://discord.pogodev.org](https://discord.pogodev.org)).
