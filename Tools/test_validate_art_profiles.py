"""Regression coverage for cross-platform catalog fingerprints."""
import json
from pathlib import Path
import tempfile
import unittest

from validate_art_profiles import validate


class CatalogFingerprintTests(unittest.TestCase):
    def test_line_endings_and_content_changes(self):
        source = Path(__file__).resolve().parents[1]
        manifest = json.loads((source / "Docs/hardware_visual_profiles.json").read_text(encoding="utf-8"))
        catalog = "Assets/Resources/Data/hardware.json"
        domain = "Assets/Scripts/Core/DomainModels.cs"
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for relative in (catalog, domain):
                target = root / relative
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes((source / relative).read_bytes())
            target = root / catalog
            lf = target.read_bytes().replace(b"\r\n", b"\n")
            for data in (lf, lf.replace(b"\n", b"\r\n")):
                with self.subTest(crlf=b"\r\n" in data):
                    target.write_bytes(data)
                    errors, _, _ = validate(root, manifest)
                    self.assertEqual([], errors)
            changed = json.loads(lf)
            changed["parts"][0]["price"] += 1
            target.write_text(json.dumps(changed), encoding="utf-8")
            errors, _, _ = validate(root, manifest)
            self.assertTrue(any("Catalog fingerprint changed" in error for error in errors))


if __name__ == "__main__":
    unittest.main()
