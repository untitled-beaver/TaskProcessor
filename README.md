# Description

This is a simple test project which handles task creation and does "fake" processing

# Startup

1. Copy `.env.example` to `.env` and set some postgres password. For sake of simpicity we do not create special service user here
2. Create directory `./docker-data/postgres`
3. Execute `docker-compose up -d --build` (or `podman compose up -d --build` if you are sure that file permissions are correct) to spin up postgres

# Usage

Repo contains `.http` file that can easily be executed from IDE:

1. Call `POST http://127.0.0.1:8080/task` to create a new task
2. Call `GET http://127.0.0.1:8080/task/{id}` to get status of created task

