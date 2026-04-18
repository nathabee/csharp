#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="dialmock-ci-build"

echo "=== CI BUILD START ==="

docker build \
  --target build \
  -t "${IMAGE_NAME}" \
  .

echo "=== CI BUILD DONE ==="