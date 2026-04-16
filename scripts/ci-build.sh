#!/usr/bin/env bash
set -euo pipefail

TEST_IMAGE="${TEST_IMAGE:?TEST_IMAGE is required}"

echo "[ci-build] building test image: ${TEST_IMAGE}"

docker build \
  --target test \
  -t "${TEST_IMAGE}" \
  .