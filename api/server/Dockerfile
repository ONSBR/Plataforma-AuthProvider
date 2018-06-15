FROM microsoft/dotnet:2.1-sdk

COPY . .

ENV PORT 8088

CMD [ "dotnet", "server/${webAppName}.dll" ]