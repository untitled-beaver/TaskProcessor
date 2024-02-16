# Description

This is a simple test project which handles task creation and does "fake" processing

# Startup

1. Copy `.env.example` to `.env` and set some postgres password. For sake of simpicity we do not create special service user here
2. Create directory `./docker-data/postgres`
2. Execute `docker-compose up -d` (or `podman compose up -d` if you are sure that file permissions are correct) to spin up postgres
