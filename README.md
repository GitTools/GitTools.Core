<h1>
   <img src="logo/GitTools_logo.svg" alt="Network" height="100">
   GitTools.Core
</h1>

GitTools.Core contains the building blocks and common functionality which is needed by other projects in GitTools.

These core libraries may prove useful if you are a library or application with interacts with git

![License][license]
[![Stable version][nuget-stable-badge]][nuget-stable]
![Pre-release version][nuget-pre-badge]
[![Build status][appveyor-badge]][appveyor]
[![Build Status][travis-badge]][travis]

## Features
 - Repository normalisation
    - Fixes up repositories on build servers which do not create local branches
 - Dynamic Repositories
    - Abstracts cloning/normalising a repository for use on the build server or in another automated way


## Icon
[Network][network] by Lorena Salagre from the Noun Project.


   [license]:              https://img.shields.io/github/license/gittools/gittools.core.svg
   [nuget-stable-badge]:   https://img.shields.io/nuget/v/GitTools.Core.svg?maxAge=2592000
   [nuget-stable]:         https://www.nuget.org/packages/GitTools.Core/
   [nuget-pre-badge]:      https://img.shields.io/nuget/vpre/gittools.core.svg
   [appveyor-badge]:       https://ci.appveyor.com/api/projects/status/jtc2o9tql0qqcc9w?svg=true
   [appveyor]:             https://ci.appveyor.com/project/GitTools/gittools-core
   [travis-badge]:         https://travis-ci.org/GitTools/GitTools.Core.svg?branch=master
   [travis]:               https://travis-ci.org/GitTools/GitTools.Core
   [network]:              https://thenounproject.com/term/network/60865/