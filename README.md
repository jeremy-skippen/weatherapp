# WeatherApp by Jeremy Skippen
This repo contains a simple web application that calls
[OpenWeatherMap.com](http://openweathermap.com) to fetch the current weather for a city.

## Running the application
The backend API can be started from the repository root like so:

    dotnet run -c Debug --project src/JeremySkippen.WeatherApp/JeremySkippen.WeatherApp.csproj

The frontend interface can be started in a separate terminal as per below:

    npm run dev

## Executing tests
The backend test suite can be run with from the repository root as per:

    dotnet test

To run the frontend tests you can run one of the following:

    # To run the tests in watch mode
    npm run test

    # To run the tests and exit
    npx vitest --run
