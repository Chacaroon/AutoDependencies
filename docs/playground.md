# Playgroun
For test purposes, you can trigger source generation without an Incremental Generator. In this guide you will find a way to setup and trigger code generation with console application.

## Overview
There are a two projects in the [samples](/samples) folder to help you get started working with code generation:
- [AutoDependencies.ConsoleApp](/samples/AutoDependencies.ConsoleApp). This project is the entry point for triggering code generation.
- [AutoDependencies.Services](/samples/AutoDependencies.Services). This project holds services that are processed by a source generator. All generated files will also be added to this project.

## Get started
To start working with source generation, follow the next steps:
1. Run **AutoDependencies.ConsoleApp** in order to generate code required for further code generation.
2. Add the service to be processed into **AutoDependencies.Services** and follow the conventions described [here](./convention.md).
3. Run **AutoDependencies.ConsoleApp** again. Part of the newly created service will be generated in the AutoDependencies.Services/Generated folder.

Congratulations! You've generated boilerplate code for just created service. Check out [Conventions and restricts](./convention.md) guide to try different features. 
