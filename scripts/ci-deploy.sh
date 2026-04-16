#!/usr/bin/env bash
set -euo pipefail

RUNTIME_IMAGE="${RUNTIME_IMAGE:?RUNTIME_IMAGE is required}"
CONTAINER_NAME="${CONTAINER_NAME:?CONTAINER_NAME is required}"
HOST_PORT="${HOST_PORT:?HOST_PORT is required}"
CONTAINER_PORT="${CONTAINER_PORT:?CONTAINER_PORT is required}"

echo "[ci-deploy] deploying ${RUNTIME_IMAGE} as ${CONTAINER_NAME}"
echo "[ci-deploy] binding 127.0.0.1:${HOST_PORT}:${CONTAINER_PORT}"

docker stop "${CONTAINER_NAME}" >/dev/null 2>&1 || true
docker rm "${CONTAINER_NAME}" >/dev/null 2>&1 || true

docker run -d \
  --name "${CONTAINER_NAME}" \
  --restart unless-stopped \
  -p "127.0.0.1:${HOST_PORT}:${CONTAINER_PORT}" \
  "${RUNTIME_IMAGE}"