#!/usr/bin/env bash
set -euo pipefail

echo "[ci-build] starting"

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT_DIR}"

IMAGE_NAME="${IMAGE_NAME:-dialmock}"
IMAGE_TAG="${IMAGE_TAG:-latest}"

echo "[ci-build] building test stage to verify restore/build/test environment"
docker build --target test -t "${IMAGE_NAME}:ci-test-${IMAGE_TAG}" .

echo "[ci-build] building runtime image"
docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" -t "${IMAGE_NAME}:latest" .

echo "[ci-build] done"