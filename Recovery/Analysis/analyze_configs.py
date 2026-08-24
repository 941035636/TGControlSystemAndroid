import pathlib
import re


ROOT = pathlib.Path(
    r"F:\WorkSpace\Project\Unity\Recovery\Extracted\CenterSystem\assets\softController"
)


def value(block: str, key: str) -> str:
    match = re.search(
        rf'"{re.escape(key)}"\s*:\s*(?:"([^"]*)"|(\d+)|(true|false))',
        block,
        re.IGNORECASE,
    )
    if not match:
        return ""
    return next(item for item in match.groups() if item is not None)


for file_name in ("Equip.json", "SoftWare.json"):
    text = (ROOT / file_name).read_text(encoding="utf-8")
    print(f"---{file_name}---")
    keys = (
        ("GetControlWay", "GetIP", "GetPort", "GetCom")
        if file_name == "Equip.json"
        else (
            "GetModelName",
            "GetIP1",
            "GetPort1",
            "GetIP2",
            "GetPort2",
            "GetHWIP",
            "ISVideo",
        )
    )
    for block in re.findall(r"\{[^{}]+\}", text, re.DOTALL):
        row = [value(block, "GetName")]
        for key in keys:
            raw = value(block, key)
            row.append(raw)
        print(" | ".join(row))
