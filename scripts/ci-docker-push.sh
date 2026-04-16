#!/usr/bin/env bash
set -euo pipefail

echo "[ci-docker-push] starting"

: "${IMAGE_NAME:?IMAGE_NAME is required}"
: "${IMAGE_TAG:?IMAGE_TAG is required}"
: "${DOCKER_REGISTRY:?DOCKER_REGISTRY is required}"
: "${DOCKER_USERNAME:?DOCKER_USERNAME is required}"
: "${DOCKER_PASSWORD:?DOCKER_PASSWORD is required}"

FULL_IMAGE_TAG="${DOCKER_REGISTRY}/${IMAGE_NAME}:${IMAGE_TAG}"
FULL_IMAGE_LATEST="${DOCKER_REGISTRY}/${IMAGE_NAME}:latest"

echo "${DOCKER_PASSWORD}" | docker login "${DOCKER_REGISTRY}" -u "${DOCKER_USERNAME}" --password-stdin

docker tag "${IMAGE_NAME}:${IMAGE_TAG}" "${FULL_IMAGE_TAG}"
docker tag "${IMAGE_NAME}:latest" "${FULL_IMAGE_LATEST}"

docker push "${FULL_IMAGE_TAG}"
docker push "${FULL_IMAGE_LATEST}"

echo "[ci-docker-push] done"