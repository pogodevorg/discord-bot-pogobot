#!/usr/bin/env bash

#exit if any command fails
set -e
artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi

git submodule update --init --recursive

dotnet restore
dotnet build ./src/NadekoBot -c Release

revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision) 

dotnet pack ./src/NadekoBot -c Release -o ./artifacts --version-suffix=$revision  