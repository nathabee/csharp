#!/usr/bin/env bash
set -euo pipefail

echo "[ci-test] starting"

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT_DIR}"

IMAGE_NAME="${IMAGE_NAME:-dialmock}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
TEST_IMAGE="${IMAGE_NAME}:ci-test-${IMAGE_TAG}"
RESULTS_DIR="${ROOT_DIR}/TestResults"

rm -rf "${RESULTS_DIR}"
mkdir -p "${RESULTS_DIR}"

# create a container from the already-built test stage image
CONTAINER_ID="$(docker create "${TEST_IMAGE}")"

cleanup() {
  docker rm -f "${CONTAINER_ID}" >/dev/null 2>&1 || true
}
trap cleanup EXIT

docker cp "${CONTAINER_ID}:/artifacts/testresults/." "${RESULTS_DIR}/"

echo "[ci-test] copied test results to ${RESULTS_DIR}"
echo "[ci-test] done"