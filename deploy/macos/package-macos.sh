#!/bin/bash
# Logbert macOS Packaging Script
# Creates .app bundle and .dmg installer for macOS ARM64

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
PUBLISH_DIR="${PUBLISH_DIR:-$ROOT_DIR/publish}"
PACKAGE_DIR="$ROOT_DIR/packages"
VERSION="${VERSION:-2.0.0}"
APP_NAME="Logbert"
BUNDLE_ID="com.logbert.Logbert"

show_help() {
    cat << EOF
Logbert macOS Packaging Script

Usage: $0 [options]

Options:
  --version VERSION    Version number for the package (default: 2.0.0)
  --publish-dir DIR    Path to the publish directory (default: ../../publish)
  --app                Create .app bundle only
  --dmg                Create .dmg installer only
  --all                Create both (default)
  --sign IDENTITY      Code signing identity (optional)
  --help               Show this help message

Prerequisites:
  Run publish-all.sh first to build the executables.
  For .dmg creation, hdiutil must be available (macOS only).

Note: This script should be run on macOS for full functionality.
EOF
}

create_app_bundle() {
    local SOURCE_DIR="$PUBLISH_DIR/osx-arm64"

    if [ ! -d "$SOURCE_DIR" ]; then
        echo "Warning: Source directory not found: $SOURCE_DIR"
        echo "Run publish-all.sh -m first"
        return 1
    fi

    echo "Creating $APP_NAME.app bundle..."

    # Create .app bundle structure
    local APP_BUNDLE="$PACKAGE_DIR/$APP_NAME.app"
    rm -rf "$APP_BUNDLE"

    mkdir -p "$APP_BUNDLE/Contents/MacOS"
    mkdir -p "$APP_BUNDLE/Contents/Resources"

    # Copy application files
    cp -r "$SOURCE_DIR"/* "$APP_BUNDLE/Contents/MacOS/"
    chmod +x "$APP_BUNDLE/Contents/MacOS/Logbert"

    # Create Info.plist
    cat > "$APP_BUNDLE/Contents/Info.plist" << EOPLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>en</string>
    <key>CFBundleExecutable</key>
    <string>Logbert</string>
    <key>CFBundleIconFile</key>
    <string>Logbert.icns</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>LSMinimumSystemVersion</key>
    <string>11.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSHumanReadableCopyright</key>
    <string>Copyright © 2024 Logbert Contributors. MIT License.</string>
    <key>CFBundleDocumentTypes</key>
    <array>
        <dict>
            <key>CFBundleTypeName</key>
            <string>Log File</string>
            <key>CFBundleTypeRole</key>
            <string>Viewer</string>
            <key>LSHandlerRank</key>
            <string>Alternate</string>
            <key>CFBundleTypeExtensions</key>
            <array>
                <string>log</string>
                <string>txt</string>
                <string>xml</string>
            </array>
        </dict>
    </array>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.developer-tools</string>
</dict>
</plist>
EOPLIST

    # Create PkgInfo
    echo "APPL????" > "$APP_BUNDLE/Contents/PkgInfo"

    # Code sign if identity provided
    if [ -n "$SIGN_IDENTITY" ]; then
        echo "Code signing with identity: $SIGN_IDENTITY"
        codesign --force --deep --sign "$SIGN_IDENTITY" "$APP_BUNDLE"
    fi

    echo "Created: $APP_BUNDLE"
    return 0
}

create_dmg() {
    local APP_BUNDLE="$PACKAGE_DIR/$APP_NAME.app"

    if [ ! -d "$APP_BUNDLE" ]; then
        echo "App bundle not found. Creating it first..."
        create_app_bundle || return 1
    fi

    if ! command -v hdiutil &> /dev/null; then
        echo "Warning: hdiutil not found. DMG creation requires macOS."
        echo "Creating tar.gz archive instead..."
        create_tarball
        return
    fi

    local DMG_NAME="Logbert-$VERSION-macos-arm64.dmg"
    local DMG_PATH="$PACKAGE_DIR/$DMG_NAME"
    local DMG_TEMP="$PACKAGE_DIR/dmg_temp"

    echo "Creating $DMG_NAME..."

    # Create temporary DMG directory
    rm -rf "$DMG_TEMP"
    mkdir -p "$DMG_TEMP"

    # Copy .app bundle
    cp -r "$APP_BUNDLE" "$DMG_TEMP/"

    # Create Applications symlink
    ln -s /Applications "$DMG_TEMP/Applications"

    # Create README
    cat > "$DMG_TEMP/README.txt" << EOREADME
Logbert $VERSION for macOS (Apple Silicon)

INSTALLATION
============
Drag Logbert.app to the Applications folder.

FIRST RUN
=========
On first run, you may need to:
1. Right-click the app and select "Open"
2. Click "Open" in the security dialog

This is required because the app is not notarized.

DOCUMENTATION
=============
https://github.com/logbert/logbert/wiki

LICENSE
=======
MIT License
EOREADME

    # Remove any existing DMG
    rm -f "$DMG_PATH"

    # Create DMG
    hdiutil create -volname "$APP_NAME $VERSION" \
        -srcfolder "$DMG_TEMP" \
        -ov -format UDZO \
        "$DMG_PATH"

    # Cleanup
    rm -rf "$DMG_TEMP"

    echo "Created: $DMG_PATH"
    echo "Size: $(du -h "$DMG_PATH" | cut -f1)"
}

create_tarball() {
    local APP_BUNDLE="$PACKAGE_DIR/$APP_NAME.app"

    if [ ! -d "$APP_BUNDLE" ]; then
        echo "App bundle not found. Creating it first..."
        create_app_bundle || return 1
    fi

    local TAR_NAME="Logbert-$VERSION-macos-arm64.tar.gz"
    local TAR_PATH="$PACKAGE_DIR/$TAR_NAME"

    echo "Creating $TAR_NAME..."

    # Create tarball of the .app bundle
    tar -czf "$TAR_PATH" -C "$PACKAGE_DIR" "$APP_NAME.app"

    echo "Created: $TAR_PATH"
    echo "Size: $(du -h "$TAR_PATH" | cut -f1)"
}

# Parse arguments
CREATE_APP=false
CREATE_DMG=false
CREATE_ALL=false
SIGN_IDENTITY=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --version)
            VERSION="$2"
            shift 2
            ;;
        --publish-dir)
            PUBLISH_DIR="$2"
            shift 2
            ;;
        --app)
            CREATE_APP=true
            shift
            ;;
        --dmg)
            CREATE_DMG=true
            shift
            ;;
        --all)
            CREATE_ALL=true
            shift
            ;;
        --sign)
            SIGN_IDENTITY="$2"
            shift 2
            ;;
        --help|-h)
            show_help
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# Default to all if nothing specified
if [ "$CREATE_APP" = false ] && [ "$CREATE_DMG" = false ]; then
    CREATE_ALL=true
fi

if [ "$CREATE_ALL" = true ]; then
    CREATE_APP=true
    CREATE_DMG=true
fi

echo "=========================================="
echo "Logbert macOS Packaging Script"
echo "=========================================="
echo ""

# Create packages directory
mkdir -p "$PACKAGE_DIR"

# Create packages
if [ "$CREATE_APP" = true ]; then
    create_app_bundle
fi

if [ "$CREATE_DMG" = true ]; then
    create_dmg
fi

echo ""
echo "=========================================="
echo "Packaging Complete!"
echo "=========================================="
echo ""
echo "Packages created in: $PACKAGE_DIR"
ls -la "$PACKAGE_DIR" | grep -E "\.(app|dmg|tar\.gz)$" | grep -i macos || ls -la "$PACKAGE_DIR" | grep -E "\.app$" || true
