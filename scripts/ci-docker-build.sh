#!/usr/bin/env bash
set -euo pipefail

RUNTIME_IMAGE="${RUNTIME_IMAGE:?RUNTIME_IMAGE is required}"

echo "[ci-docker-build] building runtime image: ${RUNTIME_IMAGE}"

docker build \
  --no-cache \
  --target runtime \
  -t "${RUNTIME_IMAGE}" \
  -t dialmock:latest \
  .