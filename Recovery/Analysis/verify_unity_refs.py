from collections import Counter
from pathlib import Path
import re


PROJECT = Path(
    r"F:\WorkSpace\Project\Unity\Recovery\RecoveredProject\CenterSystem\ExportedProject"
)
ASSETS = PROJECT / "Assets"

script_guids = set()
for meta in list(ASSETS.rglob("*.cs.meta")) + list(ASSETS.rglob("*.dll.meta")):
    match = re.search(r"^guid:\s*([0-9a-f]+)", meta.read_text(encoding="utf-8"), re.MULTILINE)
    if match:
        script_guids.add(match.group(1))

references = Counter()
for asset in list(ASSETS.rglob("*.unity")) + list(ASSETS.rglob("*.prefab")):
    text = asset.read_text(encoding="utf-8")
    for match in re.finditer(
        r"m_Script:\s*\{fileID:\s*(-?\d+),\s*guid:\s*([0-9a-f]+),\s*type:\s*3\}",
        text,
    ):
        references[match.group(2)] += 1

unknown = {guid: count for guid, count in references.items() if guid not in script_guids}
print(f"ScriptMetaGuids={len(script_guids)}")
print(f"ReferencedScriptGuids={len(references)}")
print(f"UnknownScriptGuids={len(unknown)}")
print(f"UnknownScriptReferences={sum(unknown.values())}")
for guid, count in sorted(unknown.items(), key=lambda item: (-item[1], item[0])):
    print(f"{guid} {count}")
