#!/bin/bash
# Logbert Linux Packaging Script
# Creates tar.gz and .deb packages for Linux x64 and ARM64

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
PUBLISH_DIR="${PUBLISH_DIR:-$ROOT_DIR/publish}"
PACKAGE_DIR="$ROOT_DIR/packages"
VERSION="${VERSION:-2.0.0}"

show_help() {
    cat << EOF
Logbert Linux Packaging Script

Usage: $0 [options]

Options:
  --version VERSION    Version number for the package (default: 2.0.0)
  --publish-dir DIR    Path to the publish directory (default: ../../publish)
  --tar                Create tar.gz packages only
  --deb                Create .deb packages only
  --all                Create all package types (default)
  --help               Show this help message

Prerequisites:
  Run publish-all.sh first to build the executables.
  For .deb packages, dpkg-deb must be installed.
EOF
}

create_tarball() {
    local RID=$1
    local ARCH=$2

    local SOURCE_DIR="$PUBLISH_DIR/$RID"

    if [ ! -d "$SOURCE_DIR" ]; then
        echo "Warning: Source directory not found: $SOURCE_DIR"
        echo "Run publish-all.sh -l first"
        return
    fi

    local TAR_NAME="Logbert-$VERSION-linux-$ARCH.tar.gz"
    local TAR_PATH="$PACKAGE_DIR/$TAR_NAME"

    echo "Creating $TAR_NAME..."

    # Create temporary directory
    local TEMP_DIR=$(mktemp -d)
    local APP_DIR="$TEMP_DIR/logbert-$VERSION"
    mkdir -p "$APP_DIR"

    # Copy files
    cp -r "$SOURCE_DIR"/* "$APP_DIR/"

    # Make executable
    chmod +x "$APP_DIR/Logbert"

    # Add README
    cat > "$APP_DIR/README.txt" << EOREADME
Logbert $VERSION for Linux $ARCH

INSTALLATION
============
1. Extract this archive to a folder of your choice:
   tar -xzf $TAR_NAME

2. Run the application:
   ./Logbert

OPTIONAL: Create a desktop shortcut
====================================
Copy logbert.desktop to ~/.local/share/applications/
Update the Exec and Icon paths in the file.

REQUIREMENTS
============
No additional requirements - this is a self-contained application.

DOCUMENTATION
=============
https://github.com/logbert/logbert/wiki

LICENSE
=======
MIT License - See LICENSE.txt for details
EOREADME

    # Add desktop file
    cat > "$APP_DIR/logbert.desktop" << EODESKTOP
[Desktop Entry]
Version=1.0
Type=Application
Name=Logbert
Comment=Cross-platform log file viewer
Exec=/opt/logbert/Logbert
Icon=/opt/logbert/logbert.png
Terminal=false
Categories=Development;Utility;
EODESKTOP

    # Create tarball
    tar -czf "$TAR_PATH" -C "$TEMP_DIR" "logbert-$VERSION"

    # Cleanup
    rm -rf "$TEMP_DIR"

    echo "Created: $TAR_PATH"
    echo "Size: $(du -h "$TAR_PATH" | cut -f1)"
}

create_deb() {
    local RID=$1
    local ARCH=$2
    local DEB_ARCH=$3

    local SOURCE_DIR="$PUBLISH_DIR/$RID"

    if [ ! -d "$SOURCE_DIR" ]; then
        echo "Warning: Source directory not found: $SOURCE_DIR"
        echo "Run publish-all.sh -l first"
        return
    fi

    if ! command -v dpkg-deb &> /dev/null; then
        echo "Warning: dpkg-deb not found, skipping .deb package for $ARCH"
        return
    fi

    local DEB_NAME="logbert_${VERSION}_${DEB_ARCH}.deb"
    local DEB_PATH="$PACKAGE_DIR/$DEB_NAME"

    echo "Creating $DEB_NAME..."

    # Create temporary directory structure
    local TEMP_DIR=$(mktemp -d)
    local DEB_ROOT="$TEMP_DIR/logbert"

    mkdir -p "$DEB_ROOT/DEBIAN"
    mkdir -p "$DEB_ROOT/opt/logbert"
    mkdir -p "$DEB_ROOT/usr/share/applications"
    mkdir -p "$DEB_ROOT/usr/share/icons/hicolor/256x256/apps"

    # Copy application files
    cp -r "$SOURCE_DIR"/* "$DEB_ROOT/opt/logbert/"
    chmod +x "$DEB_ROOT/opt/logbert/Logbert"

    # Create control file
    cat > "$DEB_ROOT/DEBIAN/control" << EOCONTROL
Package: logbert
Version: $VERSION
Section: utils
Priority: optional
Architecture: $DEB_ARCH
Maintainer: Logbert Contributors <logbert@example.com>
Description: Cross-platform log file viewer
 Logbert is a powerful log file viewer supporting multiple formats
 including Log4Net, NLog, Syslog, and Windows Event Logs.
 Features include real-time monitoring, advanced filtering,
 regex search, Lua scripting, and more.
Homepage: https://github.com/logbert/logbert
EOCONTROL

    # Create desktop file
    cat > "$DEB_ROOT/usr/share/applications/logbert.desktop" << EODESKTOP
[Desktop Entry]
Version=1.0
Type=Application
Name=Logbert
Comment=Cross-platform log file viewer
Exec=/opt/logbert/Logbert
Icon=logbert
Terminal=false
Categories=Development;Utility;
EODESKTOP

    # Create postinst script
    cat > "$DEB_ROOT/DEBIAN/postinst" << 'EOPOSTINST'
#!/bin/bash
# Create symlink for easy command-line access
ln -sf /opt/logbert/Logbert /usr/local/bin/logbert
EOPOSTINST
    chmod 755 "$DEB_ROOT/DEBIAN/postinst"

    # Create postrm script
    cat > "$DEB_ROOT/DEBIAN/postrm" << 'EOPOSTRM'
#!/bin/bash
# Remove symlink
rm -f /usr/local/bin/logbert
EOPOSTRM
    chmod 755 "$DEB_ROOT/DEBIAN/postrm"

    # Build .deb package
    dpkg-deb --build "$DEB_ROOT" "$DEB_PATH"

    # Cleanup
    rm -rf "$TEMP_DIR"

    echo "Created: $DEB_PATH"
    echo "Size: $(du -h "$DEB_PATH" | cut -f1)"
}

# Parse arguments
CREATE_TAR=false
CREATE_DEB=false
CREATE_ALL=false

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
        --tar)
            CREATE_TAR=true
            shift
            ;;
        --deb)
            CREATE_DEB=true
            shift
            ;;
        --all)
            CREATE_ALL=true
            shift
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
if [ "$CREATE_TAR" = false ] && [ "$CREATE_DEB" = false ]; then
    CREATE_ALL=true
fi

if [ "$CREATE_ALL" = true ]; then
    CREATE_TAR=true
    CREATE_DEB=true
fi

echo "=========================================="
echo "Logbert Linux Packaging Script"
echo "=========================================="
echo ""

# Create packages directory
mkdir -p "$PACKAGE_DIR"

# Create packages for each architecture
if [ "$CREATE_TAR" = true ]; then
    echo "Creating tar.gz packages..."
    create_tarball "linux-x64" "x64"
    create_tarball "linux-arm64" "arm64"
fi

if [ "$CREATE_DEB" = true ]; then
    echo ""
    echo "Creating .deb packages..."
    create_deb "linux-x64" "x64" "amd64"
    create_deb "linux-arm64" "arm64" "arm64"
fi

echo ""
echo "=========================================="
echo "Packaging Complete!"
echo "=========================================="
echo ""
echo "Packages created in: $PACKAGE_DIR"
ls -la "$PACKAGE_DIR" | grep -E "\.(tar\.gz|deb)$" | grep linux || true
