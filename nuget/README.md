#### Criação do Nuget-Server

Image URL: https://github.com/idoop/docker-nuget-server

Simple Nuget Server URL: https://github.com/Daniel15/simple-nuget-server


#### docker command

docker run -d --name nuget-server -p 80:80 -e NUGET_API_KEY="112233" idoop/docker-nuget-server


#### docker-compose

version: '2'
services:
  nuget-server:
    container_name: nuget-server
    image: idoop/docker-nuget-server:latest
    network_mode: "host"
    restart: always
    environment:
      NUGET_API_KEY: "112233"
      UPLOAD_MAX_FILESIZE: "40M"
      
      ## When use host network mode, 
      ## set SERVER_PORT value if you want change server expose port.
      # SERVER_PORT: "8080"
      
      ## Set nuget server domain[:port], also you can use machine(not container) ip[:port]. 
      ## eg: "192.168.11.22:8080" or "nuet.eg.com:8080"
      SERVER_NAME: "localhost:80"
      WORKER_PROCESSES: "2"
    volumes:
      - nuget-db:/var/www/simple-nuget-server/db
      - nuget-packagefiles:/var/www/simple-nuget-server/packagefiles
      - nuget-nginx:/etc/nginx
    ulimits:
      nproc: 8096
      nofile:
        soft: 65535
        hard: 65535
volumes:
  nuget-db:
  nuget-packagefiles:
  nuget-nginx:


#### Adicionando repositório Nuget 

Edite o arquivo de configuração nuget:

%APPDATA%\NuGet\NuGet.Config

Adicione o novo repositório:
<add key="Plataforma" value="http://localhost:80/" />


#### Dotnet Pack 

Criação do pacote nuget para o projeto:

dotnet pack ..\api\lib\ONS.AuthProvider.Validator.csproj --output ..\..\nuget\nupkgs --include-symbols


#### docker push

.\nuget.exe push .\nupkgs\ONS.AuthProvider.Validator.1.0.0.nupkg -source http://127.0.0.1:80 -apikey "112233"


#### deletar pack do repositório nuget

.\nuget.exe delete "ONS.AuthProvider.Validator" "1.0.0" -s http://localhost:80 -apiKey "112233"


#### nuget install

dotnet add <projectName.csproj> package ONS.AuthProvider.Validator --source http://127.0.0.1:80/ --version 1.0.0
