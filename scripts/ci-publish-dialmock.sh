#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="dialmock-publish-web"
CONTAINER_NAME="dialmock-web-container"

OUTPUT_DIR="artifacts/DialMock-web"

echo "=== CI PUBLISH WEB START ==="

mkdir -p artifacts

docker build \
  --target publish-dialmock \
  -t "${IMAGE_NAME}" \
  .

docker rm -f "${CONTAINER_NAME}" 2>/dev/null || true

docker create \
  --name "${CONTAINER_NAME}" \
  "${IMAGE_NAME}"

docker cp \
  "${CONTAINER_NAME}:/artifacts/DialMock-web" \
  "${OUTPUT_DIR}"

docker rm "${CONTAINER_NAME}"

echo "=== CI PUBLISH WEB DONE ==="