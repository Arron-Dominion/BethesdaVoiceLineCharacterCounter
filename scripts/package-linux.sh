#!/usr/bin/env bash
set -euo pipefail

version="${1:-1.0.0}"
repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
project_path="$repository_root/BethesdaVoiceLineCharacterCounter/BethesdaVoiceLineCharacterCounter.csproj"
publish_directory="$repository_root/artifacts/publish/linux-x64"
package_directory="$repository_root/artifacts/packages"
package_root="$repository_root/artifacts/deb-root"
application_name="BethesdaVoiceLineCharacterCounter"
install_directory="$package_root/usr/lib/bethesda-voice-line-character-counter"

rm -rf "$publish_directory" "$package_root"
mkdir -p "$publish_directory" "$package_directory"

dotnet publish "$project_path" \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishProfile=linux-x64 \
  -p:Version="$version" \
  -o "$publish_directory"

chmod +x "$publish_directory/$application_name"

archive_path="$package_directory/$application_name-$version-linux-x64.tar.gz"
tar -C "$publish_directory" -czf "$archive_path" .

mkdir -p \
  "$install_directory" \
  "$package_root/usr/bin" \
  "$package_root/usr/share/applications" \
  "$package_root/usr/share/icons/hicolor/scalable/apps" \
  "$package_root/DEBIAN"

cp -a "$publish_directory/." "$install_directory/"
ln -s "/usr/lib/bethesda-voice-line-character-counter/$application_name" \
  "$package_root/usr/bin/bethesda-voice-line-character-counter"
install -m 644 "$repository_root/packaging/linux/bethesda-voice-line-character-counter.desktop" \
  "$package_root/usr/share/applications/bethesda-voice-line-character-counter.desktop"
install -m 644 "$repository_root/BethesdaVoiceLineCharacterCounter/Resources/AppIcon/appicon.svg" \
  "$package_root/usr/share/icons/hicolor/scalable/apps/bethesda-voice-line-character-counter.svg"

sed "s/@VERSION@/$version/g" "$repository_root/packaging/linux/control" \
  > "$package_root/DEBIAN/control"

deb_path="$package_directory/$application_name-$version-linux-x64.deb"
dpkg-deb --root-owner-group --build "$package_root" "$deb_path"

for artifact in "$archive_path" "$deb_path"; do
  artifact_name="$(basename "$artifact")"
  (
    cd "$package_directory"
    sha256sum "$artifact_name" > "$artifact_name.sha256"
  )
done