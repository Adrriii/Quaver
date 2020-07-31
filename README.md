# Quaver Offline

## About 

**Quaver** is a community-driven and open-source competitive vertical scrolling rhythm game with two game modes and online leaderboards. It also features an in-game editor and a flexible multiplayer mode.

It is also being officially released on [Steam](https://store.steampowered.com/app/980610/Quaver/) for Windows, Mac, and Linux—making it one of the most accessible community-driven rhythm games to date.

This is the Offline version, which includes only limited features but doesn't rely on steam running in the background !

## Status

This project will likely not be heavily maintained, you might want to use the official client for that. This project is here for the sake of existing.

## Features

Those are the features still present in this version :

* **Two Game Modes** - Play the game with 4 keys or challenge yourself with 7. Each game mode has separate global and country leaderboards to compete on.

* **Map Editor** - Create your own maps to any of your favorite songs. Upload and share them with the world, and submit them for official ranking.

* **Advanced Skinning System** - Completely customize your gameplay experience with the ability to create skins. Export and share your skins with friends or upload them to the Steam workshop.

* **Replays** - Go back in time by watching your previous scores. Watch replays from other players around the world, or export your own and share them with your rivals.

* **Over 10+ Game Modifiers** - Switch up the way you play by activating in-game modifiers. Customize the speed of the song, get rid of all the long notes, or even randomize the entire map

* **Play Maps From Other Games** - Coming from a different game and miss all of your favorite maps? Quaver supports .osz, .sm, and .mc/.mcz files out of the box - with support for more games to be added in the future!

## Building & Running

Getting started with **Quaver** development is extremely easy.

* Install the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* Clone the Quaver repository and its submodules `git clone --single-branch --branch offline-custom --recurse-submodules https://github.com/Adrriii/Quaver`
* Build & run Quaver with `dotnet run --project Quaver`

## Contributing 

The best place to begin contributing to Quaver is through our [Discord server](https://discord.gg/nJa8VFr), where all of our developers, community, and testers are located.

Any contributions can be made through opening a pull request. When contributing however, please keep in mind that there should only be one branch/pull request per feature. This means if your branch is titled `my-new-feature`, you shouldn't have any unrelated commits, and the same system is applied to pull requests. Please make sure to keep your pull requests short and concise.

If you are wanting to develop a feature with the goal of having it being in the Steam release, open up an issue first, so we can discuss if it is in the scope of what we are trying to accomplish.

When contributing, please remember to follow our [code style](https://github.com/Quaver/Quaver/blob/master/CODESTYLE.md), so the codebase is consistent across the board. If you have any issues with our approaches to structuring/styling our code, feel free to bring this up.

## LICENSE

The Quaver game client is split up into submodules which are subject to their own individual licensing. Please see each submodule to view their respective license(s).

The code in this repository is released and licensed under the [Mozilla Public License 2.0](https://github.com/Quaver/Quaver/blob/develop/LICENSE). Please see the [LICENSE](https://github.com/Quaver/Quaver/blob/develop/LICENSE) file for more information. In short, if you are making any modifications to this software, you **must** disclose the source code of the modified version of the file(s), and include the original copyright notice.

Please be aware that all game assets are released and covered by a separate license. This should be noted when using this software to create derivatives for commercial purposes. Please see the [Quaver.Resources](https://github.com/Quaver/Quaver.Resources) repository for further information regarding licensing.
