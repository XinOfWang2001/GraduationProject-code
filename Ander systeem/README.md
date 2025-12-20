# DSTOOL

## Algemene projectinformatie
Deze repository is onderdeel van de Data science tool project. Het totale project bevat 2 repositories:
1. **.NET Project met Blazor standalone WebAssembly en .NET Web API services.** Deze projecten functioneren als de voornaamste frontend en Backend van dit project.
2. **Een Python webservice** dat verantwoordelijk is voor alle data science functionaliteiten in actie.
Bijvoorbeeld:
1. Het trainen en opslaan van ML-Modellen
2. Het gebruik van modellen voor forecasten van data.
3. Het ophalen en transformeren van data uit bekende Sweco databronnen.

## Benodigde software
- Docker
- NET 9.0 of Hoger
- SQL Server

## Installeren repository voor lokale ontwikkeling

### Installeren SQL Server database

Voor lokale ontwikkeling maakt de LeapDataScienceAPI gebruik van een SQL Server Docker instantie. Volg onderstaande stappen om een lokale Docker container te installeren.

#### Instructies installatie SQL Server docker instantie

1. **Download en installeer Docker**: Als Docker nog niet ge�nstalleerd is op je machine, vraag Docker aan via de Software centre van Sweco. Dit vereist toestemming van de manager van het team.

2. **Start Docker desktop op**: Dit is vereist om alle docker containers lokaal te laten draaien.

3. **Start de SQL Server Docker container**:
   Open een terminal en navigeer naar de directory waar het Docker Compose bestand zich bevindt. Voer vervolgens de volgende commando uit:
   ```sh
   docker compose up -d
   ``` 
### Het opstarten van de Backend (Zonder Docker)
Voor opstarten van de Backend, zal de LeapDataScienceAPI project aangeroepen worden.

Voer deze commando uit om de .NET API op te starten:
```sh
dotnet run --project DSAPI
``` 

### Activeren Python virtual environment.
In het geval dat de Python project via een ander IDE geopend wordt - via Visual Studio Code of PyCharm -. Dan zal eerst een virtual environment geactiveerd moeten worden.

In dit geval zal eerst een Virtual environment ge�nstalleerd moeten worden. Hiervoor is Python versie 3.13.0 gebruikt.

Maak ten eerste een virtual environment aan via deze commando:
```sh
python -m venv ./venv
```

Hierin is duidelijk dat `venv` de naam van de virtual environment is en `./venv` de folder waarin deze omgeving opgeslagen wordt.

Om deze Python omgeving te activeren, zal de commando hieronder uitgevoerd moeten worden.
```sh
venv\Scripts\Activate.ps1
```

Voor het installeren van alle packages, zal `pip install -r requirements.txt` uitgevoerd moeten worden.

```sh
pip install -r requirements.txt
```

Om de omgeving te deactiveren zal de `deactivate` commando uitgevoerd moeten worden.

```sh
deactivate
```

#### Instellen van omgevingsvariabelen.

De FastAPI omgeving vereist het gebruik van omgevingsvariabelen. 
Hierbij bestaat een env_setup bestand met alle nodige parameters, waarmee het gekopieerd kan worden naar een .env bestand.

Dit zal vervolgens via ``load_env()``` ingeladen worden binnen de applicatie.

1. Hiervoor zal eerst een .env bestand aangemaakt worden.
2. Als Powershell gebruikt wordt, voer deze commando uit

```sh
Copy-Item -Path "env_setup" -Destination "./.env
```

Deze waarden zullen ook binnen de Azure omgeving moeten bestaan.

3. Kopieer deze account url *<api_url> voor de blob storage aan ACCOUNT_URL


### Toepassen van migraties
Het toepassen van migraties:
```sh
dotnet ef migrations add <Titel__migratie> --project Infra.Data --startup-project DSAPI
```

Het updaten van de database:
```sh
dotnet ef database update --project DSAPI
```

### Het opstarten van de Frontend (Zonder Docker)

Voer deze commando uit om de .NET API op te starten:

```sh
dotnet run --project DSTool
``` 

### Het opstarten van de Data science API
Work in progress

## Het draaien van software testen
Let op! Docker moet lokaal aan het draaien zijn.

Voor het draaien van alle testen.
```sh
dotnet test
```