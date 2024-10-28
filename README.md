# SimpleHttpServer

A very simple C# Http server to provide contrast and benefits of a framework as ASP.Net core C#

# Prerequisites

- Dotnet 8.0 or higher

# How to run

- Clone the repository

## Option 1

- Open the terminal and navigate to the project folder
- Run the command `dotnet restore`
- Run the command `dotnet build`
- Run the command `dotnet run`

## Option 2

- Open the project in Visual Studio Code and run the project in the debugger

# Available endpoints

- GET / or GET /health will return a text response
- GET /api/welcome will return a json response
- POST /api/welcome with body { "name": "your name" } will return a json response
- GET /home will return an html page
