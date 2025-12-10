#!/bin/bash
# Logbert Cross-Platform Build Script
# Builds self-contained executables for all supported platforms

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
PROJECT_PATH="$ROOT_DIR/src/Logbert/Logbert.csproj"
OUTPUT_DIR="$ROOT_DIR/publish"
VERSION="${VERSION:-2.0.0}"

echo "=========================================="
echo "Logbert Build Script v$VERSION"
echo "=========================================="
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Build configuration
CONFIG="Release"
SELF_CONTAINED="true"
PUBLISH_SINGLE_FILE="true"
PUBLISH_TRIMMED="false"  # Trimming can cause issues with reflection

# Function to publish for a specific runtime
publish_runtime() {
    local RID=$1
    local FRIENDLY_NAME=$2

    echo ""
    echo "Building for $FRIENDLY_NAME ($RID)..."
    echo "----------------------------------------"

    local TARGET_DIR="$OUTPUT_DIR/$RID"

    dotnet publish "$PROJECT_PATH" \
        -c "$CONFIG" \
        -r "$RID" \
        --self-contained "$SELF_CONTAINED" \
        -p:PublishSingleFile="$PUBLISH_SINGLE_FILE" \
        -p:PublishTrimmed="$PUBLISH_TRIMMED" \
        -p:Version="$VERSION" \
        -p:IncludeNativeLibrariesForSelfExtract=true \
        -o "$TARGET_DIR"

    echo "Built: $TARGET_DIR"
}

# Parse command line arguments
BUILD_WINDOWS=false
BUILD_LINUX=false
BUILD_MACOS=false
BUILD_ALL=false

if [ $# -eq 0 ]; then
    BUILD_ALL=true
fi

while [[ $# -gt 0 ]]; do
    case $1 in
        --windows|-w)
            BUILD_WINDOWS=true
            shift
            ;;
        --linux|-l)
            BUILD_LINUX=true
            shift
            ;;
        --macos|-m)
            BUILD_MACOS=true
            shift
            ;;
        --all|-a)
            BUILD_ALL=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  --windows, -w    Build for Windows (x64 and ARM64)"
            echo "  --linux, -l      Build for Linux (x64 and ARM64)"
            echo "  --macos, -m      Build for macOS (ARM64)"
            echo "  --all, -a        Build for all platforms (default)"
            echo "  --help, -h       Show this help message"
            echo ""
            echo "Environment variables:"
            echo "  VERSION          Set the version number (default: 2.0.0)"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

if [ "$BUILD_ALL" = true ]; then
    BUILD_WINDOWS=true
    BUILD_LINUX=true
    BUILD_MACOS=true
fi

# Build for selected platforms
if [ "$BUILD_WINDOWS" = true ]; then
    publish_runtime "win-x64" "Windows x64"
    publish_runtime "win-arm64" "Windows ARM64"
fi

if [ "$BUILD_LINUX" = true ]; then
    publish_runtime "linux-x64" "Linux x64"
    publish_runtime "linux-arm64" "Linux ARM64"
fi

if [ "$BUILD_MACOS" = true ]; then
    publish_runtime "osx-arm64" "macOS ARM64 (Apple Silicon)"
fi

echo ""
echo "=========================================="
echo "Build Complete!"
echo "=========================================="
echo ""
echo "Output directory: $OUTPUT_DIR"
echo ""
ls -la "$OUTPUT_DIR"
