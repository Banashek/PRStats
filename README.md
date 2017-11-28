# PRStats

## Introduction

PRStats is a takehome interview project written for the following prompt:
* Write some code that will retrieve all pull requests for the lodash organization using the Github web API and store the results in memory. When we pair, we will use this collection of pull requests to to answer questions like "how many pull requests were merged week over week across the organization?"

The code contains library functionality (with accompanying tests) along with sample usage in the console executable.

While I would normally take a purely functional approach to something like this, I decided to use an OOP approach for the main PullRequester class to show how my OOP approach differs from my Functional approach (which can be seen in the Utils class).

## Pre-Requisites
* [Install dotnet core](https://www.microsoft.com/net/learn/get-started/macos)
* Set the `GH_TOKEN` environment variable in your shell
    * `export GH_TOKEN="my$up3r$3cr3t70k3n"` on unix-likes
* Run the project with `dotnet run`

## Running the Sample
* Cd into the `PRStats/` directory
* `dotnet run`

## How to run the tests
* Cd into the `PRStats.Tests/` directory
* `dotnet test`

## TODO
- Logging
- Handling of end of year date overlaps