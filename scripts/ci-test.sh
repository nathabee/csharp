#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="dialmock-ci-test"
CONTAINER_NAME="dialmock-test-container"

RESULT_DIR="artifacts/testresults"

echo "=== CI TEST START ==="

mkdir -p "${RESULT_DIR}"

docker build \
  --target test \
  -t "${IMAGE_NAME}" \
  .

docker rm -f "${CONTAINER_NAME}" 2>/dev/null || true

docker create \
  --name "${CONTAINER_NAME}" \
  "${IMAGE_NAME}"

docker cp \
  "${CONTAINER_NAME}:/artifacts/testresults" \
  "${RESULT_DIR}"

docker rm "${CONTAINER_NAME}"

echo "=== CI TEST DONE ==="