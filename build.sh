#!/usr/bin/env bash
git submodule update --init --recursive
dotnet restore
cd src/NadekoBot/

dotnet build