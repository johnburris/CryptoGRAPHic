# CryptoGRAPHic
![cryptographic](https://github.com/user-attachments/assets/4679ef88-daec-4ed8-916a-a8ac07cfe8ae)
## Overview
**CryptoGRAPHic** is a .NET web app written in C# and JS that utilizes ML.NET to forecast the price of Bitcoin based on historical price data retrieved from CoinGecko’s public API.

NOTE: This project is NOT meant to be an accurate prediction of Bitcoin’s price, rather it is a demonstration of ML.NET’s ```SsaForecastingEstimator``` class. It is not recommended to make investments based on this software.

**THIS IS NOT FINANCIAL ADVICE AND I AM NOT A FINANCIAL ADVISOR.**
## Quickstart
```
git clone https://github.com/johnburris/CryptoGRAPHic.git
cd CryptoGRAPHic/
dotnet restore
dotnet run
```
*Depending on your system, you may need to install additional dependencies required by ML.NET*
## Technical
When the application is launched, it makes an API call to CoinGecko retrieving daily price data for the past 365 days. It then uses ML.NET’s ```SsaForecastingEstimator``` class to predict the price for the next 7 days based on the previous data. All price data is stored in 2 JSON files (```price_actual.json``` and ```price_predicted.json```). Then, it uses Chart.js to graph the price of the previous 21 days (Actual Price) and the next 7 days (Predicted Price). All timestamps are at 0000UTC. CoinGecko updates their daily price at 0030UTC, so it is recommended to setup a Cron job/Scheduled Task every day at 0045UTC to restart the application, thus updating the actual and predicted price data. By deafult, the application listens on all IP addresses and binds to port 5000 (Configurable in ```appsettings.json``` and ```Program.cs``` respectively). If you plan on making your instance publicly accessible, it is STRONGLY recommended to run this software behind a reverse proxy server.
