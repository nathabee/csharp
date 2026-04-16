#!/usr/bin/env bash
set -euo pipefail

TEST_IMAGE="${TEST_IMAGE:?TEST_IMAGE is required}"
TEST_CONTAINER="dialmock-test-${BUILD_NUMBER:-local}"

echo "[ci-test] running tests from image: ${TEST_IMAGE}"

rm -rf TestResults
mkdir -p TestResults

docker rm -f "${TEST_CONTAINER}" >/dev/null 2>&1 || true

docker create --name "${TEST_CONTAINER}" "${TEST_IMAGE}" >/dev/null
docker start -a "${TEST_CONTAINER}"
docker cp "${TEST_CONTAINER}:/artifacts/testresults/." "TestResults/"
docker rm -f "${TEST_CONTAINER}" >/dev/null 2>&1 || true