#!/usr/bin/bash

pushd ../
docker build -f src/RinhaBackend.2024.Q1.Api/Dockerfile -t rinha-2024q1-crebito -t brutoledo/rinha-2024q1-crebito .
docker push brutoledo/rinha-2024q1-crebito
popd
