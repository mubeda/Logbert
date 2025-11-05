#!/bin/bash
cd /x/Logbert/src/Logbert

# Remove System.Windows.Forms using statements
find . -name "*.cs" -type f -exec sed -i '/using System\.Windows\.Forms;/d' {} \;

# Remove WeifenLuo imports
find . -name "*.cs" -type f -exec sed -i '/using WeifenLuo\.WinFormsUI/d' {} \;

# Remove Couchcoding.Logbert.Gui imports
find . -name "*.cs" -type f -exec sed -i '/using Couchcoding\.Logbert\.Gui;/d' {} \;

# Remove Couchcoding.Logbert.Theme imports
find . -name "*.cs" -type f -exec sed -i '/using Couchcoding\.Logbert\.Theme;/d' {} \;

echo "Legacy imports removed"
