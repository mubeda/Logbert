#!/bin/bash
cd /x/Logbert/src/Logbert

# Find all lines that reference Couchcoding.Logbert.Theme or Couchcoding.Logbert.Gui
# and comment them out or remove them

# Remove lines that try to use Theme properties/methods
find . -name "*.cs" -type f -exec sed -i '/ThemeManager\./d' {} \;
find . -name "*.cs" -type f -exec sed -i '/IThemable/d' {} \;
find . -name "*.cs" -type f -exec sed -i '/Theme\./d' {} \;
find . -name "*.cs" -type f -exec sed -i '/Gui\./d' {} \;

# Remove base class inheritance from removed classes
find . -name "*.cs" -type f -exec sed -i 's/, IThemable//g' {} \;
find . -name "*.cs" -type f -exec sed -i 's/, ISearchable//g' {} \;

echo "Legacy references cleaned"
