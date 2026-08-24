import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "F:/WorkSpace/Project/Unity/Recovery/Extracted/CenterSystem/assets/item.xlsx";
const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);

const sheets = await workbook.inspect({
  kind: "sheet",
  include: "id,name",
  maxChars: 4000,
});
console.log("SHEETS");
console.log(sheets.ndjson);

const overview = await workbook.inspect({
  kind: "workbook,sheet,table",
  maxChars: 14000,
  tableMaxRows: 40,
  tableMaxCols: 12,
  tableMaxCellChars: 200,
});
console.log("OVERVIEW");
console.log(overview.ndjson);
