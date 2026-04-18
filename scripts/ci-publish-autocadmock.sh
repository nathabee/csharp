#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="dialmock-publish-desktop"
CONTAINER_NAME="dialmock-desktop-container"

OUTPUT_DIR="artifacts/AutoCadMock-desktop-linux-x64"

echo "=== CI PUBLISH DESKTOP START ==="

mkdir -p artifacts

docker build \
  --target publish-autocadmock \
  -t "${IMAGE_NAME}" \
  .

docker rm -f "${CONTAINER_NAME}" 2>/dev/null || true

docker create \
  --name "${CONTAINER_NAME}" \
  "${IMAGE_NAME}"

docker cp \
  "${CONTAINER_NAME}:/artifacts/AutoCadMock-desktop" \
  "${OUTPUT_DIR}"

docker rm "${CONTAINER_NAME}"

echo "=== CI PUBLISH DESKTOP DONE ==="